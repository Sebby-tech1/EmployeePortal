using BCrypt.Net;
using EmployeePortal.API.Models;

namespace EmployeePortal.API.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Employees.Any())
            {
                var employee = new Employee
                {
                    Username = "employee1",
                    PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword("Password@123"),
                    Role = "Employee"
                };

                if (!context.Payments.Any())
                {
                    context.Payments.AddRange(
                        new Payment
                        {
                            CustomerName = "John Smith",
                            Amount = 5000,
                            Currency = "USD",
                            Provider = "SWIFT",
                            PayeeAccount = "987654321",
                            SwiftCode = "SBZAZAJJ",
                            Verified = false,
                            SubmittedToSwift = false,
                        },

                        new Payment
                        {
                            CustomerName = "Sarah Jones",
                            Amount = 12000,
                            Currency = "EUR",
                            Provider = "SWIFT",
                            PayeeAccount = "111222333",
                            SwiftCode = "NEDSZAJJ",
                            Verified = false,
                            SubmittedToSwift = false,
                        }

                        );
                    context.SaveChanges();
                }

                
            }
        }
    }
}
