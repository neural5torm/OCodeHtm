using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CnrsUniProv.OCodeHtm.Exceptions;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public abstract class SpatialNode<TInput, TOutput> : INode<TInput, TOutput>
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


        
        public SpatialNode(double maxSquaredDistance, int maxOutputSize)
        {
            State = NodeState.Learning;

            MaxDistance = maxSquaredDistance;
            MaxOutputSize = maxOutputSize;

            CoincidencesFrequencies = new Dictionary<TInput, double>();
        }


        public abstract void Learn(TInput input);

        protected abstract TInput FindClosestCoincidence(TInput input, out TInput canonicalInput);

        public abstract TOutput Infer(TInput input);

        public abstract TOutput TimeInfer(TInput input);

      
        public INode<TInput, TOutput> Clone()
        {
            var node = GetClone();

            node.MaxDistance = this.MaxDistance;
            node.MaxOutputSize = this.MaxOutputSize;
            node.State = this.State;

            node.CoincidencesFrequencies = new Dictionary<TInput, double>();
            foreach (var coinc in this.CoincidencesFrequencies)
                node.CoincidencesFrequencies[coinc.Key] = coinc.Value;

            if (this.LearnedCoincidences != null)
            {
                node.learnedCoincidences = new TInput[this.LearnedCoincidences.Length];
                for (int i = 0; i < this.LearnedCoincidences.Length; i++)
                {
                    node.learnedCoincidences[i] = this.LearnedCoincidences[i];
                }
            }
            return node;
        }
         
        public abstract SpatialNode<TInput, TOutput> GetClone();
   }


}
