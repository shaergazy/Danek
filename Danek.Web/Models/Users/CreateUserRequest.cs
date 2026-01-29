using System.ComponentModel.DataAnnotations;

namespace Danek.Web.Models.Users
{
    public class CreateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string TelegramUsername { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
