using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookThru.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        List<ChatDetails> Chats { get; set; }
    }
}
