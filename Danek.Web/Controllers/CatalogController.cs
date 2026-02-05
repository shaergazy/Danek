
using System;
using System.Linq;
using System.Threading.Tasks;
using Danek.DAL;
using Danek.DAL.Models;
using Danek.Web.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Danek.Web.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext _db;

        public CatalogController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            string? q,
            string? author,
            bool? inStock,
            string sortBy = "title",
            int page = 1,
            int pageSize = 12)
        {
            pageSize = pageSize switch
            {
                24 => 24,
                48 => 48,
                _ => 12
            };

            var query = _db.Books.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var pattern = $"%{q}%";
                query = query.Where(b =>
                    EF.Functions.Like(b.Title, pattern) ||
                    EF.Functions.Like(b.Author, pattern));
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                query = query.Where(b => b.Author == author);
            }

            if (inStock.HasValue)
            {
                query = inStock.Value
                    ? query.Where(b => b.Quantity > 0)
                    : query.Where(b => b.Quantity == 0);
            }

            query = sortBy.ToLowerInvariant() switch
            {
                "date" => query.OrderByDescending(b => b.CreatedAt),
                "quantity" => query.OrderByDescending(b => b.Quantity),
                _ => query.OrderBy(b => b.Title)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookCardViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Intro = b.Intro,
                    InStock = b.Quantity > 0
                })
                .ToListAsync();

            var authors = await _db.Books
                .AsNoTracking()
                .Select(b => b.Author)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            var model = new BookListViewModel
            {
                Query = q,
                SelectedAuthor = author,
                InStock = inStock,
                SortBy = sortBy,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Authors = authors,
                Books = items
            };

            return View(model);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var book = await _db.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var model = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Intro = book.Intro,
                Quantity = book.Quantity,
                CreatedAt = book.CreatedAt
            };

            return View(model);
        }
    }
}