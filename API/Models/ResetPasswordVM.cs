using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ResetPasswordVM
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}