using System.Drawing;
using IdentityChatEmail.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityChatEmail.Context
{
    public class EmailContext : IdentityDbContext<AppUser>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = FURKAN; initial Catalog = EmailChatDb; integrated security = true; trust server certificate = true; ");
        }
        public DbSet<Message> Messages { get; set; }
    }
}
