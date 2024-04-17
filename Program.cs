using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults();

builder.ConfigureServices(services =>
{
    var keyVaultUrl = Environment.GetEnvironmentVariable("KeyVaultUrl");
    if (keyVaultUrl == null)
    {
        throw new Exception("keyVaultUrl environment variable not set.");
    }
    var secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
    var connectionString = secretClient.GetSecret("ProductsDbConnectionString").Value.Value;
    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });
});

var host = builder.Build();

host.Run();

