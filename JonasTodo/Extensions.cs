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

        public static bool BooleanPrompt(string prompt, bool defaultValue)
        {
            return AnsiConsole.Prompt(
                            new TextPrompt<bool>(prompt)
                                .AddChoice(true)
                                .AddChoice(false)
                                .DefaultValue(defaultValue)
                                .WithConverter(choice => choice ? "y" : "n"));
        }

        internal static string GetStars(int number)
        {
            string starEmoji = "*";

            //unknown why this doesn't work 
            //starEmoji = Emoji.Known.Star;

            var priorityStars = string.Empty;
            if (number > 0)
            {
                for (int i = 0; i < number; i++)
                {
                    priorityStars += starEmoji;
                }
            }

            return priorityStars;
        }

        internal static Markup MarkupNullableCell(string? content)
        {
            if (content != null)
                return new Markup(content);
            else
                return new Markup("N/A", new Style(Color.LightSlateGrey));
        }
    }
}
