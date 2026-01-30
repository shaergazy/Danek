using System;
using System.Threading.Tasks;
using Danek.BLL.DTOs;
using Danek.BLL.Models;
using Danek.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Danek.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _svc;

        public BooksController(IBookService svc)
        {
            _svc = svc;
        }

        // GET: /Books
        public async Task<IActionResult> Index(string? title, string? author, bool? inStock, string? sort, int page = 1, int pageSize = 10)
        {
            var filter = new BookFilter
            {
                TitlePart = title,
                Author = author,
                InStock = inStock
            };

            var result = await _svc.GetPagedAsync(filter, sort, page < 1 ? 1 : page, pageSize);
            ViewData["CurrentSort"] = sort;
            ViewData["PageSize"] = pageSize;
            ViewData["TitleFilter"] = title;
            ViewData["AuthorFilter"] = author;
            ViewData["InStockFilter"] = inStock;
            return View(result);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new BookDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BookDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _svc.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _svc.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(BookDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var updated = await _svc.UpdateAsync(dto);
            if (updated == null) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _svc.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _svc.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeQuantity(Guid id, int amount)
        {
            var dto = await _svc.ChangeQuantityAsync(id, amount);
            if (dto == null) return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }
}
