using Microsoft.EntityFrameworkCore;
using EmployeePortal.API.Models;

namespace EmployeePortal.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Payment> Payments { get; set; }

        
    }
}
