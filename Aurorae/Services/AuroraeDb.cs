using Aurorae.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Aurorae.Services;

public class AuroraeDb(DbContextOptions<AuroraeDb> options) : DbContext(options)
{
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<Thumbnail> Thumbnails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Thumbnail>().HasIndex(t => new { t.FilePath, t.Width, t.Height }).IsUnique();
    }
}
