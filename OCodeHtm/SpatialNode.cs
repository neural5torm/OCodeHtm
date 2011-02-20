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
        public double MaxSqDistance { get; private set; }
        public uint MaxOutputSize { get; private set; }

        // TODOlater change double back to uint?
        //public HashSet<TInput> Coincidences { get; protected set; }
        public Dictionary<TInput, double> CoincidencesFrequencies { get; protected set; }


        
        public SpatialNode(double maxDistance, uint maxOutputSize)
        {
            MaxSqDistance = maxDistance * maxDistance;
            MaxOutputSize = maxOutputSize;

            CoincidencesFrequencies = new Dictionary<TInput, double>();
        }


        public abstract void Learn(TInput input);

        protected abstract TInput FindClosestCoincidence(TInput input, out TInput canonicalInput);

        public abstract TOutput Infer(TInput input);
    }


}
