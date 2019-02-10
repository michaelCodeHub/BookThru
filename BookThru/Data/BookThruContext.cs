using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookThru.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookThru.Models
{
    public class BookThruContext : IdentityDbContext<BookThruUser>
    {
        public BookThruContext (DbContextOptions<BookThruContext> options)
            : base(options)
        {
        }

        public DbSet<BookThru.Models.ChatDetails> ChatDetails { get; set; }
    }
}
