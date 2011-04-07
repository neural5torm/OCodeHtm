using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public interface IOutputWriter<TOutput>
    {
        void OutputWriterHandler(object sender, OutputEventArgs<TOutput> e);
    }
}
