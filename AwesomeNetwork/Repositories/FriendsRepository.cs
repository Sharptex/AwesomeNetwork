using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AwesomeNetwork.Repositories
{
    public class FriendsRepository : Repository<Friend>
    {
        public FriendsRepository(ApplicationDbContext db) : base(db)
        {
            
        }

        public async Task AddFriendAsync(User target, User Friend)
        {
            var friends = await Set.FirstOrDefaultAsync(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friends == null)
            {
                var item = new Friend()
                {
                    UserId = target.Id,
                    User = target,
                    CurrentFriend = Friend,
                    CurrentFriendId = Friend.Id,
                };

                await CreateAsync(item);
            }
        }

        public async Task<List<User>> GetFriendsByUserAsync(User target)
        {
            var friends = Set.Include(x => x.CurrentFriend).Where(x => x.UserId == target.Id).Select(x => x.CurrentFriend);

            return await friends.ToListAsync();
        }

        public async Task DeleteFriendAsync(User target, User Friend)
        {
            var friend = await Set.FirstOrDefaultAsync(x => x.UserId == target.Id && x.CurrentFriendId == Friend.Id);

            if (friend != null)
            {
                await DeleteAsync(friend.Id);
            }
        }

    }
}
