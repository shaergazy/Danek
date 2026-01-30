using Danek.BLL.DTOs;
using Danek.Web.Models;

namespace Danek.BLL.Services.Contracts
{
    public interface IBookService : IGenericService<ListBookDto, AddBookDto, EditBookDto, GetBookDto, Book, Guid>
    {
    }
}
