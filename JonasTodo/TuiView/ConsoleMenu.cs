using DAL.Models;
using JonasTodoConsole.TuiView.ANSI;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace JonasTodoConsole.TuiView
{
    internal class ConsoleMenu
    {
        private readonly IDbContextFactory<ToDoContext> _dbFactory;

        public ConsoleMenu(IDbContextFactory<ToDoContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        internal async Task<int> ShowAsync(CancellationToken stoppingToken)
        {
            try
            {
                TableEnum t;
                do
                {
                    t = await TableChooser.ChooseTable(stoppingToken);

                    switch (t)
                    {
                        case TableEnum.Topics:
                            await new TopicsTable(_dbFactory).RunAsync(stoppingToken);
                            break;
                        case TableEnum.Subtopics:
                            await new SubtopicsTable(_dbFactory).RunAsync(stoppingToken);
                            break;
                        default:
                            return -1;
                    }
                } while (t != TableEnum.Exit); // Loop until user chooses to exit
                return 0;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //SUB(11) add logger
                return -1;
            }
        }
    }
}
