using System.ComponentModel.DataAnnotations;

namespace Authentication.Api.Model.Dto
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public required string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 7)]
        public required string Password { get; set; }
    }
}
