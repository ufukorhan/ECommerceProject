using ECommerceProject.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProject.API.DataAccess;

public class DatabaseContext: DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartProduct> CartProducts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    
}