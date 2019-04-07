using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class MyBook
    {
        public List<Book> Bought { get; set; }
        public List<Book> Sold { get; set; }
    }
}
