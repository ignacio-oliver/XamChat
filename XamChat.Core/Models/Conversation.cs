using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamChat.Core
{
    public class Conversation
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Username { get; set; }

        public string LastMessage { get; set; }
    }
}
