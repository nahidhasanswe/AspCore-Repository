using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Sample.Service.Model
{
    public class TestDbContext : DbContext
    {
         public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestClass>().ToTable("tbl_Test");
        }

        public virtual DbSet<TestClass> TestClass {get; set;}
    }
}