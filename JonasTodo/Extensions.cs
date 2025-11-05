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
    }
}
