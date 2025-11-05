using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JonasTodoConsole.TuiView.TableViewer.Spectre.Console
{
    internal interface IRead
    {
        bool TryGet<T>(out T data);
    }
}
