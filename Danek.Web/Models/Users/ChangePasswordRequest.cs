using System.ComponentModel.DataAnnotations;

namespace Danek.Web.Models.Users
{
    public class ChangePasswordRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}
