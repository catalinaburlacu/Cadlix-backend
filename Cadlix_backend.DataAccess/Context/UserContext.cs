using Cadlix_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cadlix_backend.DataAccess.Context
{
    public class UserContext : DbContext
    {
        public DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(DbSession.ConnectionString);
        }
    }
}