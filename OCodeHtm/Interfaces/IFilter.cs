using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public interface IFilter<TOutput>
    {
        TOutput Filter(TOutput input);
    }
}
