using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public abstract class HtmLayer<TInput, TOutput> : ILearningInferring<TInput, TInput>
    {
        public double NodeInputCoverageRatio { get; private set; }
        public bool IsTrainedNodeCloned { get; private set; }
        public int MaxNodeOutputSize { get; private set; }
        public LayerState State { get; protected set; }

        
        public INode<TInput, TOutput> ClonedNode { get; protected set; }


        public HtmLayer(double coverage, bool clone, uint maxOutputSize)
        {
            NodeInputCoverageRatio = coverage;
            IsTrainedNodeCloned = clone;
            MaxNodeOutputSize = (int)maxOutputSize;

            State = LayerState.Learning;
        }


        public abstract void Learn(TInput input);

        public abstract TInput Infer(TInput input);

        
    }
}
