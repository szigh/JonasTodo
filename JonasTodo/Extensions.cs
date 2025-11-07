using DAL.Models;
using Spectre.Console;

namespace JonasTodoConsole
{
    internal static class Extensions
    {
        public static void H3(string h, bool clearConsole = true)
        {
            if(clearConsole) AnsiConsole.Clear();
            AnsiConsole.Write(new Markup(h + Environment.NewLine + Environment.NewLine,
                new Style(foreground: Color.Green, decoration: Decoration.Italic)));
        }

        internal static string GetStars(int number)
        {
            var priorityStars = string.Empty;
            if (number > 0)
            {
                for (int i = 0; i < number; i++)
                {
                    priorityStars += Emoji.Known.Star;
                }
            }

            return priorityStars;
        }

        internal static Markup MarkupCell(string? content)
        {
            if (content != null)
                return new Markup(content);
            else
                return new Markup("N/A", new Style(Color.LightSlateGrey));
        }
    }
}
