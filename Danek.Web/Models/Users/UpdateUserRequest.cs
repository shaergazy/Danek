namespace Danek.Web.Models.Users
{
    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TelegramUsername { get; set; }
        public bool? IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
