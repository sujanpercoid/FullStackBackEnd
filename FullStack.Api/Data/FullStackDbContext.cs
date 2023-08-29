using FullStack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Data
{
    public class FullStackDbContext : DbContext
    {
        public FullStackDbContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Employee>Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }
        public DbSet<Review> Reviews { get; set; }
       





    }
}
