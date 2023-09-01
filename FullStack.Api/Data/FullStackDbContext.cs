using FullStack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Data
{
    public class FullStackDbContext : DbContext
    {
        public FullStackDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product) // Cart has one Product
                .WithMany() // Product has many Carts (optional if your Product entity has a collection of related Carts)
                .HasForeignKey(c => c.ProductId); // Foreign key relationship

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User) // Cart has one Contact
                .WithMany() // Contact has many Carts (optional if your Contact entity has a collection of related Carts)
                .HasForeignKey(c => c.ContactId); // Foreign key relationship
        }


        public DbSet<Employee>Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
       





    }
}
