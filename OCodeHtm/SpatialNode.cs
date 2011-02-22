using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm
{
    public abstract class SpatialNode<TInput, TOutput> 
        where TInput : class
        where TOutput : class
    {
        public uint[] ChildNodeArrayDims { get; private set; }
        
        public double MaxSquaredDistance { get; private set; }
        public uint MaxOutputSize { get; private set; }

        public NodeMode Mode { get; protected set; }
        public bool IsLearning { get { return Mode == NodeMode.Learning; } }
        

        // TODOlater change double back to uint?
        //public HashSet<TInput> Coincidences { get; protected set; }
        public Dictionary<TInput, double> CoincidencesFrequencies { get; protected set; }


        
        public SpatialNode(uint[] childNodeArrayDims, double maxSquaredDistance, uint maxOutputSize)
        {
            Mode = NodeMode.Learning;

            ChildNodeArrayDims = childNodeArrayDims;

            MaxSquaredDistance = maxSquaredDistance;
            MaxOutputSize = maxOutputSize;

            CoincidencesFrequencies = new Dictionary<TInput, double>();
        }


        public abstract void Learn(TInput input);

        protected abstract TInput FindClosestCoincidence(TInput input, out TInput canonicalInput);

        public abstract TOutput Infer(TInput input);

        public abstract TOutput TimeBasedInfer(TInput input);
    }


    public enum NodeMode
    {
        Learning, FlashInference, TimeBasedInference
    }

}
