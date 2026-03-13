using Cadlix_backend.BusinessLogic.DBModel.User;
using Microsoft.EntityFrameworkCore;


namespace Cadlix_backend.BusinessLogic.DBModel
{
    class UserContext(DbContextOptions<UserContext> options) : DbContext(options)
    {
        public virtual DbSet<UDbContext> Users { get; set; }
    }
}
