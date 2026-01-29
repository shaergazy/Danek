namespace Danek.Web.Models.Users
{
    public class UserListResponse
    {
        public List<UserVM> Users { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
