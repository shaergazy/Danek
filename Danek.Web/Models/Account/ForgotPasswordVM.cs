namespace Danek.Web.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    namespace Danek.Web.Models.Account
    {
        public class ForgotPasswordVM
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
            [Display(Name = "Email Address")]
            public string Email { get; set; }
        }
    }
}
