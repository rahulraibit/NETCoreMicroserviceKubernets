using System;
using Microsoft.EntityFrameworkCore;

namespace AKSExample
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
    public class StoreContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=localhost;Initial Catalog=CodeFirst; user id = SA; Password=<YourStrong@Passw0rd>");
            optionsBuilder.UseSqlServer("");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Test", Age = 23}
            );
        }
        public DbSet<Employee> Employees { get; set; }
    }

}
