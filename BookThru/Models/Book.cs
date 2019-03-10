using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using BookThru.Data;

namespace BookThru.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        public string Id { get; set; }
        public string ImageURL { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int CourseCodeId { get; set; }
        public string Editon { get; set; }
        public string Description { get; set; }
        public int MinimumBidPrice { get; set; }
        public int BuyNowPrice{ get; set; }
        public DateTime Uploaded { get; set; }


        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("CourseCodeId")]
        public virtual CourseCode CourseCode { get; set; }

        [ForeignKey("Id")]
        public virtual BookThruUser User { get; set; }

        //[NotMapped]
        public string Message { get; set; }
    }
}
