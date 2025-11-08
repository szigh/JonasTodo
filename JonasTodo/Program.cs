using Azure.Core;
using Azure.Identity;
using DAL;
using JonasTodoConsole;
using JonasTodoConsole.TuiView;
using JonasTodoConsole.TuiView.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
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

        services.AddSingleton<IConsoleTablePresenter, ConsoleTablePresenter>();
        services.AddTransient<ISecretHelper, SecretHelper>();

        services.AddSingleton<ConsoleMenu>();
        services.AddHostedService<ConsoleMenuHostedService>();
    })
    .UseConsoleLifetime()
    .RunConsoleAsync();