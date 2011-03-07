using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CnrsUniProv.OCodeHtm.Exceptions;

namespace CnrsUniProv.OCodeHtm
{
    public abstract class SpatialNode<TInput, TOutput> : ILearningInferring<TInput, TOutput>
        where TInput : class
        where TOutput : class
    {

        private TInput[] learnedCoincidences;
        public TInput[] LearnedCoincidences
        {
            get
            {
                if (learnedCoincidences == null && !IsLearning)
                {
                    learnedCoincidences = CoincidencesFrequencies.Keys.ToArray();
                }
                return learnedCoincidences;
            }
        }        
        
        public double MaxDistance { get; private set; }
        public int MaxOutputSize { get; private set; }

        public NodeState State { get; protected set; }
        public bool IsLearning { get { return !IsTrained; } }
        public bool IsTrained { get { return State == NodeState.FlashInference || State == NodeState.TimeBasedInference; } }
        

        // TODOlater change double back to uint?
        public Dictionary<TInput, double> CoincidencesFrequencies { get; protected set; }


        
        public SpatialNode(double maxSquaredDistance, uint maxOutputSize)
        {
            State = NodeState.Learning;

            MaxDistance = maxSquaredDistance;
            MaxOutputSize = (int)maxOutputSize;

            CoincidencesFrequencies = new Dictionary<TInput, double>();
        }


        public abstract void Learn(TInput input);

        protected abstract TInput FindClosestCoincidence(TInput input, out TInput canonicalInput);

        public abstract TOutput Infer(TInput input);

        public abstract TOutput TimeInfer(TInput input);
    }


}
