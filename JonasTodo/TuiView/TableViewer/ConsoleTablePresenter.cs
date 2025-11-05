using System.Reflection;
using System.Text;

namespace JonasTodoConsole.TuiView.TableViewer
{
    public class ConsoleTablePresenter : IConsoleTablePresenter
    {
        private const char TopLeft = '╔';
        private const char TopRight = '╗';
        private const char BottomLeft = '╚';
        private const char BottomRight = '╝';
        private const char TopSep = '╤';
        private const char MidLeft = '╟';
        private const char MidRight = '╢';
        private const char MidSep = '┼';
        private const char BottomSep = '╧';
        private const char Vertical = '║';
        private const char VerticalSep = '│';
        private const char Horizontal = '═';
        private const char HorizontalMid = '─';

        public void PresentTable<T>(IEnumerable<T> tableData)
        {
            var rows = MaterializeRows(tableData);
            var props = GetProperties<T>();

            if (props.Length == 0)
            {
                Console.WriteLine("(no public properties to display)");
                return;
            }

            if (rows.Count == 0)
            {
                Console.WriteLine("(no rows)");
                return;
            }

            var consoleWidth = GetConsoleWidth();

            var columnWidths = ComputeColumnWidths(rows, props);
            ClampColumnWidths(columnWidths, props, consoleWidth);

            var cols = props.Length;
            var totalContentWidth = columnWidths.Values.Sum();
            var totalTableWidth = 2 + totalContentWidth + (cols - 1);

            var sb = new StringBuilder(totalTableWidth * (rows.Count + 6));

            AppendTopBorder(sb, props, columnWidths);
            AppendHeaderRow(sb, props, columnWidths);
            AppendHeaderSeparator(sb, props, columnWidths);
            AppendDataRows(sb, props, columnWidths, rows);
            AppendBottomBorder(sb, props, columnWidths);

            Console.Write(sb.ToString());
        }

        private static List<T> MaterializeRows<T>(IEnumerable<T> tableData)
        {
            // Materialize so we can iterate multiple times without re-evaluating an enumerable
            return tableData?.ToList() ?? new List<T>();
        }

        private static PropertyInfo[] GetProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();
        }

        private static int GetConsoleWidth()
        {
            try
            {
                return Console.WindowWidth;
            }
            catch
            {
                // In environments where Console.WindowWidth is not available, use a sensible default.
                return 120;
            }
        }

        private static Dictionary<PropertyInfo, int> ComputeColumnWidths<T>(IList<T> rows, PropertyInfo[] props)
        {
            var columnWidths = new Dictionary<PropertyInfo, int>(props.Length);
            foreach (var p in props)
            {
                var headerLen = p.Name.Length;
                var maxValueLen = rows
                    .Select(r => p.GetValue(r)?.ToString() ?? "null")
                    .Max(s => s.Length);

                var colWidth = Math.Max(headerLen, maxValueLen) + 2; // one leading + one trailing space
                columnWidths[p] = colWidth;
            }
            return columnWidths;
        }

        private static void ClampColumnWidths(Dictionary<PropertyInfo, int> columnWidths, PropertyInfo[] props, int consoleWidth)
        {
            var cols = props.Length;
            var totalContentWidth = columnWidths.Values.Sum();
            var totalTableWidth = 2 + totalContentWidth + (cols - 1);

            if (totalTableWidth > consoleWidth)
            {
                // Reserve space for borders and separators, then divide remaining across columns.
                var availableForColumns = Math.Max(10, consoleWidth - 2 - (cols - 1));
                // Give each column at least 8 chars (including padding)
                var perColumnMax = Math.Max(8, availableForColumns / cols);

                foreach (var p in props)
                {
                    columnWidths[p] = Math.Min(columnWidths[p], perColumnMax);
                }
            }
        }

        private static void AppendTopBorder(StringBuilder sb, PropertyInfo[] props, Dictionary<PropertyInfo, int> columnWidths)
        {
            var cols = props.Length;
            sb.Append(TopLeft);
            for (int i = 0; i < cols; i++)
            {
                var w = columnWidths[props[i]];
                sb.Append(new string(Horizontal, w))
                    .Append(i < cols - 1 ? TopSep : TopRight);
            }
            sb.AppendLine();
        }

        private static void AppendHeaderRow(StringBuilder sb, PropertyInfo[] props, Dictionary<PropertyInfo, int> columnWidths)
        {
            var cols = props.Length;
            sb.Append(Vertical);
            for (int i = 0; i < cols; i++)
            {
                var propertyInfo = props[i];
                var width = columnWidths[propertyInfo];
                var cell = " " + PadRightTruncate(propertyInfo.Name, width - 1);
                sb.Append(cell)
                    .Append(i < cols - 1 ? VerticalSep : Vertical);
            }
            sb.AppendLine();
        }

        private static void AppendHeaderSeparator(StringBuilder sb, PropertyInfo[] props, Dictionary<PropertyInfo, int> columnWidths)
        {
            var cols = props.Length;
            sb.Append(MidLeft);
            for (int i = 0; i < cols; i++)
            {
                var width = columnWidths[props[i]];
                sb.Append(new string(HorizontalMid, width));
                sb.Append(i < cols - 1 ? MidSep : MidRight);
            }
            sb.AppendLine();
        }

        private static void AppendDataRows<T>(StringBuilder sb, PropertyInfo[] props, Dictionary<PropertyInfo, int> columnWidths, IList<T> rows)
        {
            var cols = props.Length;
            foreach (var row in rows)
            {
                sb.Append(Vertical);
                for (int i = 0; i < cols; i++)
                {
                    var propertyInfo = props[i];
                    var width = columnWidths[propertyInfo];
                    var stringVal = propertyInfo.GetValue(row)?.ToString() ?? "null";
                    var cell = " " + PadRightTruncate(stringVal, width - 1);
                    sb.Append(cell)
                        .Append(i < cols - 1 ? VerticalSep : Vertical);
                }
                sb.AppendLine();
            }
        }

        private static void AppendBottomBorder(StringBuilder sb, PropertyInfo[] props, Dictionary<PropertyInfo, int> columnWidths)
        {
            var cols = props.Length;
            sb.Append(BottomLeft);
            for (int i = 0; i < cols; i++)
            {
                var width = columnWidths[props[i]];
                sb.Append(new string(Horizontal, width));
                sb.Append(i < cols - 1 ? BottomSep : BottomRight);
            }
            sb.AppendLine();
        }

        private static string PadRightTruncate(string input, int width)
        {
            if (width <= 0) return string.Empty;
            if (input.Length == width) return input;
            if (input.Length < width) return input + new string(' ', width - input.Length);

            // Truncate and append ellipsis if there's room for it
            if (width <= 2) return input.Substring(0, width);
            // leave one character for the ellipsis '…'
            return input.Substring(0, width - 1) + '…';
        }
    }
}
