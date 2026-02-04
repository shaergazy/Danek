namespace Danek.BLL.Services.Contracts
{
    public interface IRoleService
    {
        Task<List<string>> GetAllRolesAsync();
    }
}
