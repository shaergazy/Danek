using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Danek.Web.Models.Account
{
    public class UserListVM
    {
        [Display(Name = "Search")]
        public string SearchTerm { get; set; }

        [Display(Name = "Role")]
        public string RoleFilter { get; set; }

        [Display(Name = "Status")]
        public string StatusFilter { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool? EmailConfirmedFilter { get; set; }

        [Display(Name = "Created From")]
        [DataType(DataType.Date)]
        public DateTime? CreatedFrom { get; set; }

        [Display(Name = "Created To")]
        [DataType(DataType.Date)]
        public DateTime? CreatedTo { get; set; }

        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "CreatedAt";

        [Display(Name = "Sort Order")]
        public string SortOrder { get; set; } = "desc";

        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 20;

        public List<UserListItemVM> Users { get; set; } = new List<UserListItemVM>();

        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public List<SelectListItem> RoleOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> StatusOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> SortByOptions { get; set; } = new List<SelectListItem>();
    }

    public class UserListItemVM
    {
        public string Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Telegram")]
        public string TelegramUsername { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Login")]
        [DataType(DataType.DateTime)]
        public DateTime? LastLoginAt { get; set; }

        [Display(Name = "Actions")]
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanAssignRoles { get; set; }
    }
}
