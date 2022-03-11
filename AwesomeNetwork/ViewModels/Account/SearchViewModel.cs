using AwesomeNetwork.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.ViewModels.Account
{
    public class SearchViewModel
    {
        public IEnumerable<User> UserList { get; set; }
        
    }
}
