using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

namespace e_BookPvt.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Books> Books { get; set; }
        public DbSet<Customers> Customers { get; set; }

        // Override OnModelCreating if needed for custom configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationship between Orders and Customers
            modelBuilder.Entity<Orders>()
             .HasOne(o => o.Customer)
             .WithMany()
             .HasForeignKey(o => o.CustomerID)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orders>()
                .HasOne(o => o.Book)
                .WithMany()
                .HasForeignKey(o => o.BookID)
                .OnDelete(DeleteBehavior.Restrict);


            // Set default value for Status
            modelBuilder.Entity<Orders>()
                .Property(o => o.Status)
                .HasDefaultValue("Processing");

            // Configure precision and scale for the Price property
            modelBuilder.Entity<Books>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)");
        }


        public DbSet<e_BookPvt.Models.Orders> Orders { get; set; } = default!;

    }
}
