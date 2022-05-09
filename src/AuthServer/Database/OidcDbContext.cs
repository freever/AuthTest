using Microsoft.EntityFrameworkCore;

namespace AuthServer.Database;

public class OidcDbContext : DbContext
{
    public OidcDbContext()
    {
    }

    public OidcDbContext(DbContextOptions options): base(options)
    {
    }
}