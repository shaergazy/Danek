using System.ComponentModel.DataAnnotations;

namespace Danek.Web.Models.Account
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Password Requirements")]
        public PasswordRequirementsVM PasswordRequirements { get; set; } = new PasswordRequirementsVM();


        public class PasswordRequirementsVM
        {
            public bool RequireDigit { get; set; } = true;
            public bool RequireLowercase { get; set; } = true;
            public bool RequireUppercase { get; set; } = true;
            public bool RequireNonAlphanumeric { get; set; } = true;
            public int RequiredLength { get; set; } = 8;

            public string GetRequirementsText()
            {
                var requirements = new List<string>
            {
                $"Minimum {RequiredLength} characters"
            };

                if (RequireDigit) requirements.Add("At least one digit (0-9)");
                if (RequireLowercase) requirements.Add("At least one lowercase letter (a-z)");
                if (RequireUppercase) requirements.Add("At least one uppercase letter (A-Z)");
                if (RequireNonAlphanumeric) requirements.Add("At least one special character (!@#$%^&*)");

                return string.Join(", ", requirements);
            }
        }
    }
}
