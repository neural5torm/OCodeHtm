using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm
{
    public delegate void OutputEventHandler<TOutput>(object sender, OutputEventArgs<TOutput> e);


    public class OutputEventArgs<TOutput> : EventArgs
    {
        public TOutput Output { get; private set; }
        public string Label { get; private set; }

        public OutputEventArgs(TOutput output, string category)
        {
            Output = output;
            Label = category;
        }
    }
}
