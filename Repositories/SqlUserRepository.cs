// Repositories/SqlUserRepository.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class SqlUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public SqlUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users
                       .AsNoTracking()
                       .ToList();
    }

    public User? GetById(int id)
    {
        return _context.Users
                       .AsNoTracking()
                       .FirstOrDefault(u => u.Id == id);
    }

    // === НОВЫЕ МЕТОДЫ ===

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
                             .AsNoTracking()
                             .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
                             .AsNoTracking()
                             .FirstOrDefaultAsync(u => u.Email == email);
    }

    public User Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public bool Update(User user)
    {
        var existing = _context.Users.Find(user.Id);
        if (existing == null) return false;

        existing.Username = user.Username;
        existing.UserLastName = user.UserLastName;
        existing.UserMiddleName = user.UserMiddleName;
        existing.UserFirstName = user.UserFirstName;
        existing.Email = user.Email;
        existing.Phone = user.Phone;
        existing.City = user.City;

        // Обновляем поля безопасности
        existing.PasswordHash = user.PasswordHash;
        existing.IsEmailConfirmed = user.IsEmailConfirmed;
        existing.LockedUntil = user.LockedUntil;
        existing.FailedLoginAttempts = user.FailedLoginAttempts;
        existing.LastLoginAt = user.LastLoginAt;

        _context.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        _context.SaveChanges();
        return true;
    }
}