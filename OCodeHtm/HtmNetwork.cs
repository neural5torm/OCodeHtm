using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CnrsUniProv.OCodeHtm.Interfaces;

namespace CnrsUniProv.OCodeHtm
{
    public class HtmNetwork<TInput, TOutput>
    {
        public List<HtmLayer<TInput, TOutput>> Layers { get; private set; }
    }
}
