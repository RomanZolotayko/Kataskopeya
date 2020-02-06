using Kataskopeya.EF.Models;
using System.Data.Entity;

namespace Kataskopeya.EF
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext() : base("KataskopeyaDB")
        {

        }

        public DbSet<User> Users { get; set; }


    }
}
