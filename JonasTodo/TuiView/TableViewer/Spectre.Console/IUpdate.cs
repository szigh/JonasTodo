using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonasTodoConsole.TuiView.TableViewer.Spectre.Console
{
    internal interface IUpdate
    {
        bool TryUpdate<T>(int id, Func<T,T> updateFunction);
    }
}
