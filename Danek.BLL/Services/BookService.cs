using AutoMapper;
using Danek.BLL.DTOs;
using Danek.BLL.Services.Contracts;
using Danek.DAL.Repositories.Contracts;
using Danek.Web.Models;
using Microsoft.Extensions.Logging;

namespace Danek.BLL.Services
{
    public class BookService : GenericService<ListBookDto, AddBookDto, EditBookDto, GetBookDto, Book, Guid>, IBookService
    {
        public BookService(IMapper mapper, IUnitOfWork<Book, Guid> unitOfWork, ILogger<GenericService<ListBookDto, AddBookDto, EditBookDto, GetBookDto, Book, Guid>> logger) : base(mapper, unitOfWork, logger)
        {
        }
    }
}
