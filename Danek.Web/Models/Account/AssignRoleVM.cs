using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Danek.Web.Models.Account
{
    public class AssignRoleVM
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Current Roles")]
        public List<string> CurrentRoles { get; set; } = new List<string>();

        [Required(ErrorMessage = "At least one role must be selected")]
        [Display(Name = "Select Roles")]
        public List<string> SelectedRoles { get; set; } = new List<string>();

        [Display(Name = "Available Roles")]
        public List<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();

        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
