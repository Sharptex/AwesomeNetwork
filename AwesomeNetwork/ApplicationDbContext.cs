using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeNetwork.Models;
using AwesomeNetwork.Models.Configurations;
using AwesomeNetwork.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AwesomeNetwork
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Message> Message { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration<Friend>(new FriendConfiguration());
            builder.ApplyConfiguration(new MessageConfiguration());
        }
    }
}
