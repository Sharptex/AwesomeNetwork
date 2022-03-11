using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Models.Users
{
    public class UserWithFriendExt : User
    {
        public bool IsFriendWithCurrent { get; set; }
    }
}
