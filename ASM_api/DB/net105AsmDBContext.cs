using ASM_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_api.DB
{
    public class net105AsmDBContext : DbContext
    {
        public net105AsmDBContext(DbContextOptions<net105AsmDBContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
    }
}
