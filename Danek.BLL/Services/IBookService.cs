using System;
using System.Threading.Tasks;
using Danek.BLL.DTOs;
using Danek.BLL.Models;

namespace Danek.BLL.Services
{
    public interface IBookService
    {
        Task<PagedResult<BookDto>> GetPagedAsync(BookFilter filter, string? sort, int page, int pageSize);
        Task<IEnumerable<BookDto>> SearchByTitleAsync(string titlePart);
        Task<BookDto> CreateAsync(BookDto dto);
        Task<BookDto?> UpdateAsync(BookDto dto);
        Task DeleteAsync(Guid id);
        Task<BookDto?> ChangeQuantityAsync(Guid id, int amount);
        Task<BookDto?> GetByIdAsync(Guid id);
    }
}
