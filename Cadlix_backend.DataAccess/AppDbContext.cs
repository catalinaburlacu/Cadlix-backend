using Microsoft.EntityFrameworkCore;
using Cadlix_backend.DataAccess.Entities;

namespace Cadlix_backend.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Users> Users { get; set; }

}
