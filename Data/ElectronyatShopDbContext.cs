using ElectronyatShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectronyatShop.Data;

public class ElectronyatShopDbContext : IdentityDbContext<ApplicationUser>
{
    public ElectronyatShopDbContext(DbContextOptions<ElectronyatShopDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }

    public DbSet<CartItem> CartItems { get; set; }

    public DbSet<Cart> Carts { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.CartItems)
            .WithOne(ci => ci.Product)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .OnDelete(DeleteBehavior.Cascade);

        // Fix Identity key/index length for MySQL
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(127);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(127);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(127);
        });

        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(127);
            entity.Property(e => e.NormalizedName).HasMaxLength(127);
        });

        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.Property(e => e.LoginProvider).HasMaxLength(127);
            entity.Property(e => e.ProviderKey).HasMaxLength(127);
        });

        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.Property(e => e.UserId).HasMaxLength(127);
            entity.Property(e => e.RoleId).HasMaxLength(127);
        });

        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.Property(e => e.UserId).HasMaxLength(127);
            entity.Property(e => e.LoginProvider).HasMaxLength(127);
            entity.Property(e => e.Name).HasMaxLength(127);
        });

        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.Property(e => e.UserId).HasMaxLength(127);
        });

        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.Property(e => e.RoleId).HasMaxLength(127);
        });

        // Your existing setup
        modelBuilder.Entity<Product>()
            .HasMany(p => p.CartItems)
            .WithOne(ci => ci.Product)
            .OnDelete(DeleteBehavior.Cascade);
    
        modelBuilder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}