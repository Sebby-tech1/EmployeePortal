using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.API.DTOs
{
    public class LoginDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{3,20}$")]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
