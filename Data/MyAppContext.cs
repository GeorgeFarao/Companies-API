using Microsoft.EntityFrameworkCore;
using TodoApi2.Data;

namespace TodoApi2.Data
{
    public class MyAppContext : DbContext
    {
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<WorkStation> WorkStations { get; set; }


        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>().ToTable("Branch")/*.HasOne(c => c.CompanyId).WithOne().OnDelete(DeleteBehavior.Cascade)*/;
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<WorkStation>().ToTable("WorkStation");

        }
    }
}