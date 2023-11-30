using Microsoft.EntityFrameworkCore;
using SolforbOrdersTest.Domain;

namespace SolforbOrdersTest.Infrastructure.DB
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Provider> Providers { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
            // Использовано исключительно в условиях тестового задания
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .Property(p => p.Quantity).HasPrecision(18, 3);
            
            // Данные заполнены только для тестового задания
            modelBuilder.Entity<Provider>(entity =>
            {
                entity.HasData(
                    new Provider() { Id = 1, Name = "Тестовый поставщик Альфа" },
                    new Provider() { Id = 2, Name = "Тестовый поставщик Бетта" },
                    new Provider() { Id = 3, Name = "Тестовый поставщик Гамма" });
            });
        }
    }

}

