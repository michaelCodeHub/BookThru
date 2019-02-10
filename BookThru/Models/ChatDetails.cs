using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class ChatDetails
    {
        public int ChatDetailsId { get; set; }
        public string UserEmail { get; set; }
        public DateTime ChatTime { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}
