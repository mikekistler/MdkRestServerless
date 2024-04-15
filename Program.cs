using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults();

builder.ConfigureServices(services =>
{
    var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("keyVaultUrl") ?? "");
    var secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
    var connectionString = secretClient.GetSecret("ConnectionStrings.ProductsDb").Value.Value;
    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });
});

var host = builder.Build();

host.Run();
