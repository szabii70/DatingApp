using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
    public virtual DbSet<AppUser> Users { get; set; }

    public DataContext(DbContextOptions options) : base(options)
    {
    }
}
