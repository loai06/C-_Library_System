using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
namespace LibraryManagementSystem.Data

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //Soft Delete
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Book>().HasQueryFilter(b => !b.IsDeleted);

            //Max characters
            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            //Price>0
            modelBuilder.Entity<Book>()
           .Property(b => b.Price)
           .HasColumnType("decimal(18,2)");

            //Relationships
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId);

        }
    }
}