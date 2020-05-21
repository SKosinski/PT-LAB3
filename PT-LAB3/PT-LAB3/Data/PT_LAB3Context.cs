using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PT_LAB3.Models;

namespace PT_LAB3.Data
{
    public class PT_LAB3Context : DbContext
    {
        public PT_LAB3Context (DbContextOptions<PT_LAB3Context> options)
            : base(options)
        {
        }

        public DbSet<PT_LAB3.Models.Book> Book { get; set; }

        public DbSet<PT_LAB3.Models.User> User { get; set; }

        public DbSet<PT_LAB3.Models.Rent> Rent { get; set; }
    }
}
