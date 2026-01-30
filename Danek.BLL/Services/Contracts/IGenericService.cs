namespace Danek.BLL.Services.Contracts
{
    public interface IGenericService<TListDto, TAddDto, TEditDto, TGetDto, TEntity, TKey>
        where TAddDto : class
        where TEditDto : class
        where TListDto : class
        where TGetDto : class
    {
        IEnumerable<TListDto> GetAll();
        Task<TGetDto> GetByIdAsync(TKey id);
        Task<TEntity> CreateAsync(TAddDto dto);
        Task UpdateAsync(TEditDto dto);
        Task DeleteAsync(TKey id);
        Task DeleteAsync(TEntity obj);
    }
}
