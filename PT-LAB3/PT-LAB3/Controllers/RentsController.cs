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
    public class RentsController : ControllerBase
    {
        private readonly PT_LAB3Context _context;

        public RentsController(PT_LAB3Context context)
        {
            _context = context;
        }

        // GET: api/Rents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rent>>> GetRent()
        {
            return await _context.Rent.ToListAsync();
        }

        // GET: api/Rents/1
        /*
         Dostajemy:
         {
             "id": 1,
             "bookID": 3,
             "userID": 1
         }
         */
        [HttpGet("{id}")]
        public async Task<ActionResult<Rent>> GetRent(int id)
        {
            var rent = await _context.Rent.FindAsync(id);

            if (rent == null)
            {
                return NotFound();
            }

            return rent;
        }

        // PODP3.3
        /*
         Wpisujemy:
         /api/rents/book/1
         
         Dostajemy wszystkich wypozyczajacych ksiazke 1, ponieważ inaczej ta funkcja nie ma sensu, zawsze wyświetli tylko jednego użytkownika:
         [
             {
                 "id": 2,
                 "name": "Zbigniew",
                 "surname": "Boniek",
                 "eMail": "zibi.bon@o2.pl"
             },
             {
                 "id": 3,
                 "name": "Adam",
                 "surname": "Kasztan",
                 "eMail": "adam.kasz@wp.pl"
             }
         ]
         */
        [HttpGet("book/{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetBookRent(int id)
        {
            var rentList = await _context.Rent.ToListAsync();
            var userList = new List<User>();
            //var usersController = new UsersController(_context);
            foreach (var rentRecord in rentList)
            {
                if (rentRecord.BookID == id)
                {
                    userList.Add(_context.User.Find(rentRecord.UserID));
                }

            }

            if (userList.Count == 0)
            {
                return NotFound();
            }

            return userList;
        }

        // PODP3.2
        // PODP4.3
        /*
         Wpisujemy:
         /api/rents/user
         
         Dostajemy wszystkie ksiazki zalogowanego usera:
         [
             {
                 "id": 2,
                 "title": "Wielka śmierć",
                 "author": "Bonda",
                 "isRented": false
             },
             {
                 "id": 1,
                 "title": "Harry Potter",
                 "author": "JK Rowling",
                 "isRented": false
             }
         ]
         */
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Book>>> GetUserRent()
        {
            var rentList = await _context.Rent.ToListAsync();
            var bookList = new List<Book>();
            var me = User.Claims.ToList();
            //var booksController = new BooksController(_context);
            var knownUser = _context.User
                    .Where(b => b.Name == me[2].Value.ToString())
                    .Where(b => b.Surname == me[3].Value.ToString())
                    .Where(b => b.EMail == me[4].Value.ToString())
                    .FirstOrDefault();
            
            foreach (var rentRecord in rentList)
            {
                if (rentRecord.UserID == knownUser.ID)
                {
                    bookList.Add(_context.Book.Find(rentRecord.BookID));
                }

            }

            if (bookList.Count == 0)
            {
                return NotFound();
            }

            return bookList;
        }

        // PUT: api/Rents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRent(int id, Rent rent)
        {
            if (id != rent.ID)
            {
                return BadRequest();
            }

            _context.Entry(rent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RentExists(id))
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

        // PODP3.1
        // POST: api/Rents
        // Wypożycz danemu uzytkownikowi daną książkę
        /*
         {
         	"BookID": 3,
         	"UserID": 1
         }
         
         {
         	"BookID": 2,
         	"UserID": 2
         }
         
         {
         	"BookID": 1,
         	"UserID": 2
         }
         
         {
	         "BookID": 1,
	         "UserID": 3
         }
         */
        [HttpPost]
        public async Task<ActionResult<Rent>> PostRent(Rent rent)
        {
            _context.Rent.Add(rent);
            await _context.SaveChangesAsync();

            var book = _context.Book.Where(p => p.ID == rent.BookID).FirstOrDefault();
            if (book != null)
                book.IsRented = true;
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRent", new { id = rent.ID }, rent);
        }

        // DELETE: api/Rents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rent>> DeleteRent(int id)
        {
            var rent = await _context.Rent.FindAsync(id);
            var bookID = rent.BookID;
            if (rent == null)
            {
                return NotFound();
            }

            _context.Rent.Remove(rent);
            await _context.SaveChangesAsync();

            var isRented = _context.Rent.Where(r => r.BookID == bookID).FirstOrDefault();
            if (isRented==null)
            {
                _context.Book.Where(b => b.ID == bookID).FirstOrDefault().IsRented = false;
            }
            await _context.SaveChangesAsync();

            return rent;
        }

        private bool RentExists(int id)
        {
            return _context.Rent.Any(e => e.ID == id);
        }
    }
}
