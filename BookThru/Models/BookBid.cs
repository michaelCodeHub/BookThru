using BookThru.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class BookBid
    {
        [Key]
        public int BookBidId { get; set; }
        public int BookId { get; set; }
        public string Id { get; set; }
        public int BidPrice { get; set; }
        public DateTime DateOfBid { get; set; }

        [ForeignKey("Id")]
        public virtual BookThruUser User { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}
