using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public interface INode<TInput, TOutput> : ICloneable<INode<TInput, TOutput>>, ILearningInferring<TInput, TOutput>
    {
    }
}
