using Danek.DAL;
using Danek.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _db;

        public BookController(AppDbContext db)
        {
            _db = db;
        }

        // ===================== INDEX =====================
        public IActionResult Index()
        {
            List<Book> bookList = _db.Books.ToList();
            return View(bookList);
        }

        // ===================== Add =====================
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Book obj)
        {
            if (ModelState.IsValid)
            {
                _db.Books.Add(obj);
                _db.SaveChanges();
                TempData["success"] = "Book created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // ===================== EDIT =====================
        public IActionResult Edit(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }

            Book? bookFromDb = _db.Books.Find(id);

            if (bookFromDb == null)
            {
                return NotFound();
            }

            return View(bookFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book obj)
        {
            if (ModelState.IsValid)
            {
                _db.Books.Update(obj);
                _db.SaveChanges();
                TempData["success"] = "Book updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // ===================== DELETE =====================
        public IActionResult Delete(Guid? id)
        {
            if (id == null || id == Guid.Empty)
            {
                return NotFound();
            }

            Book? bookFromDb = _db.Books.Find(id);

            if (bookFromDb == null)
            {
                return NotFound();
            }

            return View(bookFromDb);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(Guid? id)
        {
            Book? obj = _db.Books.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            _db.Books.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Book deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
