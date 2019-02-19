using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int CourseCodeId { get; set; }
        public string Editon { get; set; }
        public string Description { get; set; }
        public int MinimumBidPrice { get; set; }
        public int BuyNowPrice{ get; set; }
        public DateTime Uploaded { get; set; }
    }
}
