using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using JonasTodo.Models;
using DAL;
using JonasTodoConsole.TuiView;
using JonasTodoConsole.TuiView.TableViewer;
using Core;
using JonasTodoConsole.TuiView.TableViewer.Spectre.Console;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<LearningDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
        services.Configure<DALSettings>(context.Configuration.GetSection(nameof(DALSettings)));

        services.AddDALServices();
        services.AddCoreServices();

        services.AddSingleton<IConsoleTablePresenter, ConsoleTablePresenter>();
        services.AddSingleton<IAnsiTableChooser, AnsiTableChooser>();
        services.AddTransient<IConsoleTablePresenter, AnsiTablePresenter>();

        services.AddSingleton<ConsoleMenu>();
        services.AddHostedService<ConsoleMenuHostedService>();
    })
    .UseConsoleLifetime()
    .RunConsoleAsync();