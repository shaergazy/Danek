using System;
using System.Linq;
using System.Threading.Tasks;
using Danek.BLL.DTOs;
using Danek.BLL.Models;
using Danek.DAL.Models;
using Danek.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Danek.BLL.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork<Book, Guid> _uow;

        public BookService(IUnitOfWork<Book, Guid> uow)
        {
            _uow = uow;
        }

        public async Task<BookDto> CreateAsync(BookDto dto)
        {
            var entity = new Book
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Author = dto.Author,
                Intro = dto.Intro,
                Description = dto.Description,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository.AddAsync(entity);
            await _uow.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _uow.Repository.DeleteByIdAsync(id);
            await _uow.SaveChangesAsync();
        }

        public async Task<BookDto?> GetByIdAsync(Guid id)
        {
            var entity = await _uow.Repository.GetByIdAsync(id);
            if (entity == null) return null;
            return Map(entity);
        }

        public async Task<PagedResult<BookDto>> GetPagedAsync(BookFilter filter, string? sort, int page, int pageSize)
        {
            var query = _uow.Repository.GetAll().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter?.TitlePart))
                query = query.Where(b => b.Title.Contains(filter.TitlePart!));

            if (!string.IsNullOrWhiteSpace(filter?.Author))
                query = query.Where(b => b.Author == filter.Author);

            if (filter?.InStock == true)
                query = query.Where(b => b.Quantity > 0);
            else if (filter?.InStock == false)
                query = query.Where(b => b.Quantity == 0);

            // Sorting
            query = sort switch
            {
                "title_desc" => query.OrderByDescending(b => b.Title),
                "title" => query.OrderBy(b => b.Title),
                "author_desc" => query.OrderByDescending(b => b.Author),
                "author" => query.OrderBy(b => b.Author),
                "quantity_desc" => query.OrderByDescending(b => b.Quantity),
                "quantity" => query.OrderBy(b => b.Quantity),
                "created_desc" => query.OrderByDescending(b => b.CreatedAt),
                "created" => query.OrderBy(b => b.CreatedAt),
                _ => query.OrderBy(b => b.Title)
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Intro = b.Intro,
                    Description = b.Description,
                    Quantity = b.Quantity
                })
                .ToListAsync();

            return new PagedResult<BookDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<BookDto?> ChangeQuantityAsync(Guid id, int amount)
        {
            var entity = await _uow.Repository.GetByIdAsync(id);
            if (entity == null) return null;

            var newQuantity = entity.Quantity + amount;
            if (newQuantity < 0)
                newQuantity = 0;

            entity.Quantity = newQuantity;

            await _uow.Repository.UpdateAsync(entity);
            await _uow.SaveChangesAsync();

            return Map(entity);
        }

        public async Task<IEnumerable<BookDto>> SearchByTitleAsync(string titlePart)
        {
            var q = _uow.Repository.Where(b => b.Title.Contains(titlePart));
            var list = await q.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Intro = b.Intro,
                Description = b.Description,
                Quantity = b.Quantity
            }).ToListAsync();

            return list;
        }

        public async Task<BookDto?> UpdateAsync(BookDto dto)
        {
            var entity = await _uow.Repository.GetByIdAsync(dto.Id);
            if (entity == null) return null;

            entity.Title = dto.Title;
            entity.Author = dto.Author;
            entity.Intro = dto.Intro;
            entity.Description = dto.Description;
            entity.Quantity = dto.Quantity;

            await _uow.Repository.UpdateAsync(entity);
            await _uow.SaveChangesAsync();

            return Map(entity);
        }

        private static BookDto Map(Book b) => new BookDto
        {
            Id = b.Id,
            Title = b.Title,
            Author = b.Author,
            Intro = b.Intro,
            Description = b.Description,
            Quantity = b.Quantity
        };
    }
}
