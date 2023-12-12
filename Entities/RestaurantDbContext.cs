using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Entities;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
    {
        
    }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Address> Address { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>()
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(25);

        modelBuilder.Entity<Dish>()
            .Property(d => d.Name)
            .IsRequired();

        modelBuilder.Entity<Address>()
            .Property(a => a.City)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Address>()
           .Property(a => a.Street)
           .IsRequired()
           .HasMaxLength(50);

        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();
    }

    // add-migration Init - tworzenie migracji na podstawie tego pliku
    // update-database - tworzenie db
    // remove-migration - usuwanie ostatniej migracji
}