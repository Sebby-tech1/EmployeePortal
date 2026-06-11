using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeePortal.API.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Provider { get; set; }

        public string PayeeAccount { get; set; }

        [RegularExpression(@"^[A-Z]{6}[A-Z0-9]{2}([A-Z0-9]{3})?$")]
       
        public string SwiftCode { get; set; }

        public bool SubmittedToSwift { get; set; }

        public bool Verified { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }
}
