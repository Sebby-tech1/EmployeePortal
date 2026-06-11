using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeePortal.API.Data;
using EmployeePortal.API.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.RateLimiting;

namespace EmployeePortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Employee")]
    [EnableRateLimiting("fixed")]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsValidInput(string input, string pattern)
        {
            return input != null && Regex.IsMatch(input, pattern);
        }

        private bool IsValidId(string id)
        {
            return Regex.IsMatch(id, @"^\d+$");
        }

        private string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#x27;")
                .Replace("/", "&#X2F;");
        }

        private bool ContainsSqlInjection(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            string[] sqlKeywords = {
                "SELECT", "INSERT", "UPDATE", "DELETE", "DROP", "UNION", "EXEC", "EXECUTE",
                "CONVERT", "CHAR", "VARCHAR", "ALTER", "CREATE"
            };

            string upperInput = input.ToUpper();
            return sqlKeywords.Any(keyword => upperInput.Contains(keyword));
        }

        private string ValidatePayment(Payment payment)
        {
            payment.CustomerName = SanitizeInput(payment.CustomerName);
            payment.Provider = SanitizeInput(payment.Provider);
            payment.SwiftCode = SanitizeInput(payment.SwiftCode);
            payment.Currency = SanitizeInput(payment.Currency);
            payment.PayeeAccount = SanitizeInput(payment.PayeeAccount);

            if (ContainsSqlInjection(payment.CustomerName) || ContainsSqlInjection(payment.Provider) || ContainsSqlInjection(payment.SwiftCode))
                return "Invalid input detected.";

            if (!IsValidInput(payment.CustomerName, @"^[a-zA-Z\s]{2,50}$"))
                return "Invalid customer name. Only letters and spaces allowed (2-50 chars).";

            if (!IsValidInput(payment.Amount.ToString(), @"^\d+(\.\d{1,2})?$"))
                return "Invalid amount. Must be a positive number with up to 2 decimal places.";

            if (!IsValidInput(payment.Currency, @"^[A-Z]{3}$"))
                return "Invalid currency. Must be a 3-letter code e.g. USD, EUR.";

            if (!IsValidInput(payment.Provider, @"^[a-zA-Z0-9 ]{2,50}$"))
                return "Invalid provider name.";

            if (!IsValidInput(payment.PayeeAccount, @"^\d{8,20}"))
                return "Invalid account number. Must be 8-20 digits";

            if (!IsValidInput(payment.SwiftCode, @"^[A-Z0-9]{8,11}$"))
                return "Invalid SWIFT code format";

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] Payment payment)
        {
            var error = ValidatePayment(payment);
            if (error != null)
                return BadRequest(error);

            payment.CreatedDate = DateTime.UtcNow;
            payment.Verified = false;
            payment.SubmittedToSwift = false;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        [HttpGet("pending")]
        public IActionResult GetPendingPayments()
        {
            var payment = _context.Payments.Where(x => !x.Verified).ToList();
            return Ok(payment);
        }

        [HttpPut("verify/{id}")]
        public IActionResult VerifyPayment(int id)
        {
            if (!IsValidId(id.ToString()))
                return BadRequest("Invalid payment ID format");

            var payment = _context.Payments.FirstOrDefault(x => x.Id == id);

            if (payment == null)
                return NotFound();

            payment.Verified = true;
            _context.SaveChanges();

            return Ok(new { message = "Payment verified successfully" });
        }

        [HttpGet("verified")]
        public IActionResult GetVerifiedPayments()
        {
            var payments = _context.Payments.Where(x => x.Verified && !x.SubmittedToSwift).ToList();
            return Ok(payments);
        }

        [HttpPost("submit-to-swift/{id}")]
        public IActionResult SubmitToSwift(int id)
        {
            var payment = _context.Payments.FirstOrDefault(x => x.Id == id);

            if (payment == null)
                return NotFound();

            if (!payment.Verified)
            {
                return BadRequest(new { message = "Payment must be verified first" });
            }

            payment.SubmittedToSwift = true;
            _context.SaveChanges();

            return Ok(new { message = "Payment submitted to SWIFT" });
        }

        [HttpGet("submitted")]
        public IActionResult GetActionResult()
        {
            var payments = _context.Payments.Where(x => x.SubmittedToSwift).ToList();
            return Ok(payments);
        }
    }
}