using Azure.Identity;
using Core;
using DAL;
using DAL.Models;
using JonasTodoConsole.TuiView;
using JonasTodoConsole.TuiView.ANSI.TableViewer.Spectre.Console;
using JonasTodoConsole.TuiView.Console;
using JonasTodoConsole.TuiView.TableViewer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Host.CreateDefaultBuilder(args)
    //.ConfigureAppConfiguration((context, config) =>
    //{
    //    var built = config.Build();
    //    var kvEndpoint = built["https://a5b2cc5b.vault.azure.net/:Endpoint"]; // e.g. https://myvault.vault.azure.net/
    //    if (!string.IsNullOrEmpty(kvEndpoint))
    //    {
    //        config.AddAzureKeyVault(new Uri(kvEndpoint), new DefaultAzureCredential());
    //    }
    //})
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<LearningDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("ConnectionStrings:DefaultConnection")));
        services.Configure<DALSettings>(context.Configuration.GetSection(nameof(DALSettings)));

        services.AddDALServices();
        services.AddCoreServices();

        services.AddSingleton<IConsoleTablePresenter, ConsoleTablePresenter>();
        services.AddTransient<IConsoleTablePresenter, AnsiTablePresenter>();

        services.AddSingleton<ConsoleMenu>();
        services.AddHostedService<ConsoleMenuHostedService>();
    })
    .UseConsoleLifetime()
    .RunConsoleAsync();