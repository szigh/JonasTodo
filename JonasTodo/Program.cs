using Azure.Core;
using Azure.Identity;
using DAL;
using JonasTodoConsole;
using JonasTodoConsole.TuiView;
using JonasTodoConsole.TuiView.Tables;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.AddSimpleConsole(options =>
        {
            options.TimestampFormat = "[HH:mm:ss] ";
            options.ColorBehavior = LoggerColorBehavior.Enabled;
            options.IncludeScopes = true;
        });
        logging.SetMinimumLevel(LogLevel.Debug);
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