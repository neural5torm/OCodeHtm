using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}
