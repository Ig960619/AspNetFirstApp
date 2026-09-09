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

    public User Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;  // Id будет заполнен после SaveChanges
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
        existing.Password = user.Password;

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
