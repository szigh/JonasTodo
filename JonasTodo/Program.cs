using Azure.Identity;
using Core;
using DAL;
using DAL.Models;
using JonasTodoConsole;
using JonasTodoConsole.TuiView;
using JonasTodoConsole.TuiView.Console;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
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
            config.AddAzureKeyVault(new Uri(kvEndpoint), new DefaultAzureCredential());
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.AddKeyVaultSecretHelper(context.Configuration["KeyVault:Endpoint"]!);
        var conn = context.Configuration.GetConnectionString("ToDoApp");
        Console.WriteLine(conn);
        services.AddDbContextFactory<LearningDbContext>(options => options.UseSqlServer(conn));
        services.Configure<DALSettings>(context.Configuration.GetSection(nameof(DALSettings)));

        services.AddDALServices();
        services.AddCoreServices();

        services.AddSingleton<IConsoleTablePresenter, ConsoleTablePresenter>();
        services.AddTransient<ISecretHelper, SecretHelper>();

        services.AddSingleton<ConsoleMenu>();
        services.AddHostedService<ConsoleMenuHostedService>();
    })
    .UseConsoleLifetime()
    .RunConsoleAsync();