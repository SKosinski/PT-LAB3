using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PT_LAB3.Data;
using PT_LAB3.Models;

namespace PT_LAB3.Controllers
{
    [Route("api/[controller]")]
    // PODP6
    [EnableCors("CorsPolicy")]
    // PODP4.2
    [Authorize]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly PT_LAB3Context _context;

        public BooksController(PT_LAB3Context context)
        {
            _context = context;
        }

        // PODP2.8
        // GET: api/Books
        // Wyswietla wszystkie ksiazki
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Book>>> GetBook()
        {
            return await _context.Book.ToListAsync();
        }

        // PODP2.7
        // GET: api/Books/1
        // Wyświetla pojedynczą książkę
        /*
         Dostajemy:
         {
           "Title":"Mikołajek",
           "Author": "Gościnny",
           "IsRented": false
         }
         */
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Book.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PODP2.9
        // PUT: api/Books/3
        // Aktualizuje 3 książkę 
        /*
        {
            "ID": 3,
            "Title": "Barry Trotter",
            "Author": "JK Jumpling",
            "isRented": false
        }
        */
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.ID)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PODP2.6
        // POST: api/Books
        // Dodaje 3 książki
        /*
         {
           "Title":"Mikołajek",
           "Author": "Gościnny",
           "IsRented": false
         }
         
         {
           "Title":"Wielka śmierć",
           "Author": "Bonda",
           "IsRented": false
         }
         
         {
           "Title":"Harry Potter",
           "Author": "JK Rowling",
           "IsRented": false
         }
         */
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            _context.Book.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.ID }, book);
        }

        // PODP2.10
        // DELETE: api/Books/2
        // Usuwa książkę "Wielka Śmierć"
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return book;
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.ID == id);
        }
    }
}
