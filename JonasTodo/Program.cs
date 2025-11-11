using Azure.Core;
using Azure.Identity;
using DAL;
using JonasTodoConsole;
using JonasTodoConsole.TuiView;
using JonasTodoConsole.TuiView.Tables;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

// Configure log4net log file path to use LocalApplicationData
var logDirectory = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "JonasTodo",
    "logs");
Directory.CreateDirectory(logDirectory);

// Load log4net configuration and update file path
var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// Update the RollingFileAppender path
foreach (var appender in logRepository.GetAppenders())
{
    if (appender is RollingFileAppender rollingFileAppender)
    {
        var logFilePath = Path.Combine(logDirectory, "JonasTodo.log");
        rollingFileAppender.File = logFilePath;
        rollingFileAppender.ActivateOptions();
    }
}

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        // Clear default console logging providers
        logging.ClearProviders();
        
        // Add log4net with configuration file
        logging.AddLog4Net("log4net.config");
        
        // Set minimum level
        logging.SetMinimumLevel(LogLevel.Information);
        
        // Disable or reduce EF Core logging
        logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
        logging.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        var built = config.Build();
        var kvEndpoint = built["KeyVault:Endpoint"];
        if (!string.IsNullOrWhiteSpace(kvEndpoint))
        {
            TokenCredential credential;
            if (context.HostingEnvironment.IsDevelopment())
                credential = new InteractiveBrowserCredential();
            else 
                credential = new DefaultAzureCredential();
            config.AddAzureKeyVault(new Uri(kvEndpoint), credential);
        }
    })
    .ConfigureServices((context, services) =>
    {
        // Using Azure KeyVault to manage secrets (connection strings)
        services.AddKeyVaultSecretHelper(context.Configuration["KeyVault:Endpoint"]!);
        services.AddDALServices(context.Configuration);

        services.AddTransient<ISecretHelper, SecretHelper>();

        services.AddTransient<ITopicsTable, TopicsTable>();
        services.AddTransient<ISubtopicsTable, SubtopicsTable>();
        services.AddScoped<ConsoleMenu>();
        services.AddHostedService<ConsoleMenuHostedService>();
    })
    .UseConsoleLifetime()
    .RunConsoleAsync();