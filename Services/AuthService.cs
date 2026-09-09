using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.Auth;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

/// <summary>
/// Сервис авторизации: регистрация, вход, обновление токенов
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly AppDbContext _context;

    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        AppDbContext context)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<(AuthResponse? Response, string? Error)> RegisterAsync(RegisterRequest request, string? ipAddress)
    {
        var existingByUsername = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingByUsername != null)
        {
            return (null, "Пользователь с таким username уже существует");
        }

        var existingByEmail = await _userRepository.GetByEmailAsync(request.Email);
        if (existingByEmail != null)
        {
            return (null, "Пользователь с таким email уже существует");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            City = request.City,
            UserFirstName = request.UserFirstName,
            UserLastName = request.UserLastName,
            UserMiddleName = request.UserMiddleName ?? "",
            Phone = request.Phone,
            IsEmailConfirmed = false,
            FailedLoginAttempts = 0
        };

        var createdUser = _userRepository.Add(user);

        var accessToken = _jwtService.GenerateAccessToken(createdUser);
        var refreshToken = _jwtService.GenerateRefreshToken(createdUser.Id, ipAddress);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = _jwtService.AccessTokenExpirationMinutes * 60,
            User = MapToUserDto(createdUser)
        };

        return (response, null);
    }

    public async Task<(AuthResponse? Response, string? Error)> LoginAsync(LoginRequest request, string? ipAddress)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            return (null, "Неверный username или пароль");
        }

        if (user.LockedUntil != null && user.LockedUntil > DateTime.UtcNow)
        {
            var remainingMinutes = (int)(user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
            return (null, $"Аккаунт заблокирован. Попробуйте через {remainingMinutes} минут");
        }

        if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockedUntil = DateTime.UtcNow.AddMinutes(LockoutMinutes);
                _userRepository.Update(user);
                return (null, $"Слишком много неудачных попыток. Аккаунт заблокирован на {LockoutMinutes} минут");
            }

            _userRepository.Update(user);
            return (null, "Неверный username или пароль");
        }

        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        _userRepository.Update(user);

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken(user.Id, ipAddress);

        await RevokeUserRefreshTokensAsync(user.Id, ipAddress);

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = _jwtService.AccessTokenExpirationMinutes * 60,
            User = MapToUserDto(user)
        };

        return (response, null);
    }

    public async Task<(AuthResponse? Response, string? Error)> RefreshTokenAsync(string accessToken, string refreshToken, string? ipAddress)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
        {
            return (null, "Недействительный access token");
        }

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
        {
            userIdClaim = principal.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        }
        
        if (userIdClaim == null)
        {
            return (null, "Недействительный access token");
        }

        var userId = int.Parse(userIdClaim);

        var storedRefreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.UserId == userId);

        if (storedRefreshToken == null || !storedRefreshToken.IsActive)
        {
            return (null, "Недействительный refresh token");
        }

        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.RevokedAt = DateTime.UtcNow;
        storedRefreshToken.RevokedByIp = ipAddress;

        var user = storedRefreshToken.User!;
        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id, ipAddress);

        newRefreshToken.ReplacedByToken = storedRefreshToken.Token;

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresIn = _jwtService.AccessTokenExpirationMinutes * 60,
            User = MapToUserDto(user)
        };

        return (response, null);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || !token.IsActive)
        {
            return false;
        }

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task RevokeUserRefreshTokensAsync(int userId, string? ipAddress)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
        }
    }

    private static UserDto MapToUserDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        UserFirstName = user.UserFirstName,
        UserLastName = user.UserLastName,
        City = user.City
    };
}
