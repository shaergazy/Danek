using Danek.BLL.DTOs;
using Danek.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _service;

        public BookController(IBookService service)
        {
            _service = service;
        }

        // ===================== INDEX =====================
        public IActionResult Index()
        {
            var bookList = _service.GetAll();
            return View(bookList);
        }

        // ===================== Add =====================
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(AddBookDto obj)
        {
            if (ModelState.IsValid)
            {
                _service.CreateAsync(obj);
                TempData["success"] = "Book created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // ===================== EDIT =====================
        public async Task<IActionResult> EditAsync(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }

            GetBookDto bookFromDb = await _service.GetByIdAsync(id);

            if (bookFromDb == null)
            {
                return NotFound();
            }

            return View(bookFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(EditBookDto obj)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(obj);
                TempData["success"] = "Book updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // ===================== DELETE =====================
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }

            GetBookDto? bookFromDb = await _service.GetByIdAsync(id);

            if (bookFromDb == null)
            {
                return NotFound();
            }

            return View(bookFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOSTAsync(Guid id)
        {
            GetBookDto? obj = await _service.GetByIdAsync(id);

            if (obj == null)
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            TempData["success"] = "Book deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
