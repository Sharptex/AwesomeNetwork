using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.ViewModels
{
    public class ChatViewModel
    {
        public MessageViewModel NewMessage { get; set; }

        public List<Message> History { get; set; }

        public User Sender { get; set; }

        public User Recipient { get; set; }

        public ChatViewModel()
        {
            NewMessage = new MessageViewModel();
        }
    }
}
