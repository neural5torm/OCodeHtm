using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public delegate void OutputEventHandler<TOutput>(object sender, OutputEventArgs<TOutput> e);


    public interface IObservableOutput<TOutput>
        where TOutput : class
    {
        event OutputEventHandler<TOutput> OnOutput;
    }


    public class OutputEventArgs<TOutput> : EventArgs
    {
        public TOutput Output { get; private set; }

        public OutputEventArgs(TOutput output)
        {
            Output = output;
        }
    }
}
