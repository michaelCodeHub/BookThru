using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookThru.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookThru.Models;

namespace BookThru.Models
{
    public class BookThruContext : IdentityDbContext<BookThruUser>
    {
        public BookThruContext (DbContextOptions<BookThruContext> options)
            : base(options)
        {
        }
        

        public DbSet<BookThru.Models.Book> Book { get; set; }
        public DbSet<BookThru.Models.Category> Category { get; set; }
        public DbSet<BookThru.Models.CourseCode> CourseCode { get; set; }
    }
}
