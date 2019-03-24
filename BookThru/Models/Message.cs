using BookThru.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string FromId { get; set; }
        public string ToId { get; set; }
        public string Content { get; set; }
        public DateTime Sent { get; set; }
    }
}
