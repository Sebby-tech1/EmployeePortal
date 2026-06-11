using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.API.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{3,20}$")]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; } = "Employee";
}
}
