using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace ProjectPRN.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext() { }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            
                var builder = new ConfigurationBuilder().
                    SetBasePath(Directory.GetCurrentDirectory()).
                    AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfiguration configuration = builder.Build();
                optionBuilder.UseSqlServer(configuration.GetConnectionString("DePurete"));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasMany(a => a.Products).WithOne(a=>a.Category);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.HasKey(a => a.ID);

                entity.HasOne(a => a.User).WithMany(a => a.Orders).HasForeignKey(a => a.UserID);

                entity.HasOne(a => a.Status).WithMany(a => a.Orders).HasForeignKey(a => a.StatusID);

                entity.HasMany(a => a.OrderDetails).WithOne(a => a.Order).HasForeignKey(a=>a.OrderID);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.HasKey(a => new { a.OrderID, a.ProductID });

                entity.HasOne(a => a.Order).WithMany(a => a.OrderDetails).HasForeignKey(a => a.OrderID);

                entity.HasOne(a => a.Product).WithMany(a => a.OrderDetails).HasForeignKey(a => a.ProductID);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasKey(a => a.ID);

                entity.HasOne(a => a.Category).WithMany(a => a.Products).HasForeignKey(a => a.CategoryID);

                entity.HasMany(a => a.OrderDetails).WithOne(a => a.Product).HasForeignKey(a => a.ProductID);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.HasKey(a => a.ID);

                entity.HasMany(a => a.Orders).WithOne(a => a.Status).HasForeignKey(a => a.StatusID);

                entity.HasData(
                   new Status { ID = 1, Name = "Cart" },
                   new Status { ID = 2, Name = "Waiting" },
                   new Status { ID = 3, Name = "Approved" },
                   new Status { ID = 4, Name = "Shipping" },
                   new Status { ID = 5, Name = "Finished" }
                 );
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasKey(a => a.ID);

                entity.HasMany(a => a.Orders).WithOne(a => a.User).HasForeignKey(a => a.UserID);
            });
            
            base.OnModelCreating(modelBuilder); 
        }
        public static void InitiateData(AppDBContext context)
        {
            context.Status.AddRange(
                new Models.Status { Name = "waiting" },
                new Models.Status { Name = "approved" },
                new Models.Status { Name = "shipping" },
                new Models.Status { Name = "finish" }
                );
            //context.Category.AddRange(new Category { Name = })
        }
    }
}
