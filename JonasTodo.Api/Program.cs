using Azure.Core;
using Azure.Identity;
using DAL;

var builder = WebApplication.CreateBuilder(args);

// Load Key Vault (same pattern as console app)
IHostBuilder hostBuilder = builder.Host.ConfigureAppConfiguration((context, config) =>
{
    var built = config.Build();
    var kvEndpoint = built["KeyVault:Endpoint"];
    if (!string.IsNullOrWhiteSpace(kvEndpoint))
    {
        TokenCredential credential = context.HostingEnvironment.IsDevelopment() 
            ? new InteractiveBrowserCredential() 
            : new DefaultAzureCredential();
        IConfigurationBuilder configurationBuilder = config.AddAzureKeyVault(new Uri(kvEndpoint), credential);
    }
});

// Register DAL and other services
builder.Services.AddDALServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.RoutePrefix = "swagger");

app.MapControllers();
app.Run();
