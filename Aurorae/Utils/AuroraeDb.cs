using Aurorae.Models;
using Microsoft.EntityFrameworkCore;

namespace Aurorae.Utils;

public class AuroraeDb : DbContext
{
    public AuroraeDb(DbContextOptions<AuroraeDb> options) : base(options) { }
    // Add DbSet properties for your models
    public DbSet<AccessToken> AccessTokens { get; set; }
}
