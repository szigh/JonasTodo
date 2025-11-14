using Azure.Core;
using Azure.Identity;
using DAL;

var builder = WebApplication.CreateBuilder(args);

// Load Key Vault (same pattern as console app)
var kvEndpoint = builder.Configuration["KeyVault:Endpoint"];
if (!string.IsNullOrWhiteSpace(kvEndpoint))
{
    TokenCredential credential = builder.Environment.IsDevelopment()
        ? new InteractiveBrowserCredential()
        : new DefaultAzureCredential();
    builder.Configuration.AddAzureKeyVault(new Uri(kvEndpoint), credential);
}

// Register DAL and other services
builder.Services.AddDALServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c => c.RoutePrefix = "swagger");

app.MapControllers();
app.Run();
