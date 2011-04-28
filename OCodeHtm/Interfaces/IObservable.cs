using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    // TODO use for OutputEventHandler's sender type
    public interface IObservable
    {
        string Name { get; set; }
    }
}
