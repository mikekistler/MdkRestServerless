using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

// This class is used to create a design-time factory for the Entity Framework Core tools.
// Before I added this class I was getting the following error from `dotnet ef migrations add InitialCreate`:
//   Unable to create a 'DbContext' of type ''. The exception 'Unable to resolve service for type
//   'Microsoft.EntityFrameworkCore.DbContextOptions`1[AppDbContext]' while attempting to activate 'AppDbContext'.'
//   was thrown while attempting to create an instance. For the different patterns supported at design time,
//   see https://go.microsoft.com/fwlink/?linkid=851728
//
// You have to set the `DesignTimeConnectionString` environment variable explicitly -- you can't just add it
// to the local.settings.json file, because the Entity Framework Core tools don't read that file.

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("DesignTimeConnectionString");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Could not find a connection string named 'DesignTimeConnectionString'.");
        }
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
