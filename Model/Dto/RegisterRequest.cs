using System.ComponentModel.DataAnnotations;

namespace Authentication.Api.Model.Dto
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 7)]
        public required string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 7)]
        public required string ConfirmPassword { get; set; }
    }
}
