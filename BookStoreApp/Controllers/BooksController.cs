using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Data;
using BookStoreApp.Models;

namespace BookStoreApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStoreDbContext _context;

        public BooksController(BookStoreDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.AsNoTracking().ToListAsync();
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create() => View();

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Price")] Book book)
        {
            if (!ModelState.IsValid) return View(book);

            // Uniqueness: same Title + Author not allowed
            bool exists = await _context.Books
                .AnyAsync(b => b.Title == book.Title && b.Author == book.Author);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This book already exists.");
                return View(book);
            }

            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Price")] Book book)
        {
            if (id != book.Id) return NotFound();
            if (!ModelState.IsValid) return View(book);

            // Prevent editing into a duplicate of another record
            bool duplicate = await _context.Books
                .AnyAsync(b => b.Id != book.Id && b.Title == book.Title && b.Author == book.Author);
            if (duplicate)
            {
                ModelState.AddModelError(string.Empty, "Another book with the same Title and Author already exists.");
                return View(book);
            }

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                // Friendly Week-6 messages (no tech leak)
                bool stillExists = await _context.Books.AnyAsync(e => e.Id == book.Id);
                if (!stillExists)
                    ModelState.AddModelError(string.Empty, "This record was deleted by another user.");
                else
                    ModelState.AddModelError(string.Empty, "This record was modified by another user. Please refresh and try again.");

                return View(book);
            }
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id) => _context.Books.Any(e => e.Id == id);
    }
}
