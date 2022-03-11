using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Repositories
{
    public class MessageRepository : Repository<Message>
    {
        public MessageRepository(ApplicationDbContext db) : base(db)
        {

        }

        public async Task<List<Message>> GetMessagesAsync(User sender, User recipient)
        {
            Set.Include(x => x.Recipient);
            Set.Include(x => x.Sender);

            var from = await Set.Where(x => x.SenderId == sender.Id && x.RecipientId == recipient.Id).ToListAsync();
            var to = await Set.Where(x => x.SenderId == recipient.Id && x.RecipientId == sender.Id).ToListAsync();

            var itog = new List<Message>();
            itog.AddRange(from);
            itog.AddRange(to);
            itog.OrderBy(x => x.Id);
            return itog;
        }
    }
}
