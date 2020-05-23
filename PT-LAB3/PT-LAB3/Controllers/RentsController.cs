using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PT_LAB3.Data;
using PT_LAB3.Models;

namespace PT_LAB3.Controllers
{
    [Route("api/[controller]")]
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

        /*
         Wpisujemy:
         /api/rents/book/1
         
         Dostajemy wszystkich wypozyczajacych ksiazke 1:
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
            var userssController = new UsersController(_context);

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

        /*
         Wpisujemy:
         /api/rents/user/1
         
         Dostajemy wszystkie ksiazki usera 1:
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
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetUserRent(int id)
        {
            var rentList = await _context.Rent.ToListAsync();
            var bookList = new List<Book>();
            var booksController = new BooksController(_context);

            foreach (var rentRecord in rentList)
            {
                if (rentRecord.UserID == id)
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
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

        // POST: api/Rents
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Rent>> PostRent(Rent rent)
        {
            _context.Rent.Add(rent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRent", new { id = rent.ID }, rent);
        }

        // DELETE: api/Rents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rent>> DeleteRent(int id)
        {
            var rent = await _context.Rent.FindAsync(id);
            if (rent == null)
            {
                return NotFound();
            }

            _context.Rent.Remove(rent);
            await _context.SaveChangesAsync();

            return rent;
        }

        private bool RentExists(int id)
        {
            return _context.Rent.Any(e => e.ID == id);
        }
    }
}
