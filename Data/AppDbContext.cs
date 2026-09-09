using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Альтернативный способ настройки (без атрибутов в модели)
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);

            // Id - автоинкремент
            entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd();

            // Username - NOT NULL, max 50
            entity.Property(e => e.Username)
                  .IsRequired()
                  .HasMaxLength(50);

            // UserLastName - NULL, max 50
            entity.Property(e => e.UserLastName)
                  .HasMaxLength(50);

            // UserMiddleName - NOT NULL, max 50
            entity.Property(e => e.UserMiddleName)
                  .IsRequired()
                  .HasMaxLength(50);

            // User FirstName - колонка с пробелом в имени!
            entity.Property(e => e.UserFirstName)
                  .HasColumnName("User FirstName");  // ВАЖНО!

            // Email - NULL, max 50
            entity.Property(e => e.Email)
                  .HasMaxLength(50);

            // Phone - NULL, max 50
            entity.Property(e => e.Phone)
                  .HasMaxLength(50);

            // City - NOT NULL, max 50
            entity.Property(e => e.City)
                  .IsRequired()
                  .HasMaxLength(50);

            // Password - NULL, max 50
            entity.Property(e => e.Password)
                  .HasMaxLength(50);
        });
    }
}
