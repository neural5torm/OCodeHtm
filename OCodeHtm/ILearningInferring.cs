using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm
{
    public interface ILearningInferring<TInput, TOutput>
    {
        void Learn(TInput input);

        TOutput Infer(TInput input);
    }
}
