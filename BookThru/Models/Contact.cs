using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        public string FromId { get; set; }
        public string ToId { get; set; }
        
        public string FromName { get; set; }
        public string ToName { get; set; }
    }
}
