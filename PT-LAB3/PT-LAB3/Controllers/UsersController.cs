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
    public class UsersController : ControllerBase
    {
        private readonly PT_LAB3Context _context;

        public UsersController(PT_LAB3Context context)
        {
            _context = context;
        }

        // PODP2.3
        // GET: api/Users
        // Pokazuje wszystkich użytkowników
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            var me = User.Claims.ToList();
            return await _context.User.ToListAsync();
        }

        // PODP2.2
        // GET: api/Users/2
        // Wyswietli Zbigniewa (2 użytkownik)
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PODP2.4
        // PUT: api/Users/1
        // Zaktualizuje Adama (1 użytkownika)
        /*
         {
           "ID":2,	
           "Name":"Arnold",
           "Surname": "Kasztelan",
           "EMail": "arnold.kasz@wp.pl"
         }
         */
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.ID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // PODP2.1
        // POST: api/Users/Post
        // doda następujących użytkowników
        //
        /*
         {
           "Name":"Adam",
           "Surname": "Kasztan",
           "EMail": "adam.kasz@wp.pl"
         }

         {
           "Name":"Zbigniew",
           "Surname": "Boniek",
           "EMail": "zibi.bon@o2.pl"
         }

         {
           "Name":"Michał",
           "Surname": "Materla",
           "EMail": "m.m@gmail.com"
         }
        */

        [HttpPost]
        [Route("Post")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.ID }, user);
        }

        // PODP5
        // POST: api/Users/me
        // Doda obecnie zalogowanego użytkownika
        [HttpPost]
        [Route("Me")]
        public async Task<ActionResult<User>> PostMe()
        {
            var me = User.Claims.ToList();

            var user = new Models.User { Name = me[2].Value.ToString(), Surname = me[3].Value.ToString(), EMail = me[4].Value.ToString() };
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUser", new { id = user.ID }, user);
        }

        // PODP2.5
        // DELETE: api/Users/3
        // Usunie Michała (3 użytkownika)
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    }
}
