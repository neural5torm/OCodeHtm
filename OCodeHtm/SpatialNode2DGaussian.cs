using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Exceptions;

namespace CnrsUniProv.OCodeHtm
{
    public class SpatialNode2DGaussian : SpatialNode2D
    {
        public double SquaredSigma { get; private set; }


        public SpatialNode2DGaussian(double maxDistance = Default.MaxDistance, double sigma = Default.NoSigma, int maxOutputSize = Default.MaxNodeOutputSize)
            : base(maxDistance, maxOutputSize)
        { 
            if (sigma == Default.NoSigma || sigma < 0.0)
                SquaredSigma = (maxDistance == 0 ? Default.SquaredSigma : maxDistance);
            else
                SquaredSigma = sigma * sigma;
        }



        /// <summary>
        /// Learn the current input.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="HtmRuleException"></exception>
        public override void Learn(SparseMatrix input)
        {
            // Limitation due to HTM v1.x design (cannot learn after inference without modifying ouput vector structure)
            if (!IsLearning)
                // TODOlater allow learning after training when using FixedMaxSize nodes
                throw new HtmRuleException("Cannot learn after any other mode than learning", this);

            // Ignore blank input 
            //TODOlater? treat any input with identical values for *all* components as blank?
            //TODOlater use DetectBlanks/DetectBlanksMode properties
            if (input.NonZerosCount == 0)
            { return; }

            // Check matrix size
            if (CoincidencesFrequencies.Count > 0 &&
                (CoincidencesFrequencies.Keys.First().RowCount != input.RowCount || CoincidencesFrequencies.Keys.First().ColumnCount != input.ColumnCount))
                throw new HtmRuleException("Cannot learn varying sized inputs", this);

            SparseMatrix existingCoincidence = FindClosestCoincidence(input);

            if (existingCoincidence != null)
                CoincidencesFrequencies[existingCoincidence] += 1;
            else
                if (CoincidencesFrequencies.Count < MaxOutputSize)
                    CoincidencesFrequencies[input] = 1;
        }



        private SparseMatrix FindClosestCoincidence(SparseMatrix input)
        {
            SparseMatrix canonical;
            return FindClosestCoincidence(input, out canonical);
        }
        // TODOlater try to optimize by running computation in a Task
        protected override SparseMatrix FindClosestCoincidence(SparseMatrix input, out SparseMatrix canonicalInput)
        {
            canonicalInput = input; // no Winner-Take-All

            if (CoincidencesFrequencies.ContainsKey(input))
                return input;
            
            // else
            if (MaxDistance <= 0)
                return null;

            // else
            // Look for the 1st coincidence with a distance to the input below MaxDistance 
            // (which SHOULD be the closest since each learned coincidence has to be distant to all others by MaxDistance)
            var closestCoincidence = input;
            closestCoincidence = null;

            

            foreach (var coincidence in CoincidencesFrequencies.Keys)
            {
                // Compute distance between matrices and return coincidence, if found
                var diff = coincidence.Subtract(input);
                var norm = diff.FrobeniusNorm();
                if (norm <= MaxDistance)
                    return coincidence;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do Flash inference for the given input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Vector Infer(SparseMatrix input)
        {
            State = NodeState.FlashInference;

            if (LearnedCoincidences.Length == 0)
            {
                return new SparseVector(1);
            }

            var output = new DenseVector(LearnedCoincidences.Length);
            for (int i = 0; i < LearnedCoincidences.Length; ++i)
            {
                var diff = LearnedCoincidences[i] - input;
                var norm = diff.FrobeniusNorm();

                output[i] = Math.Exp(-(norm * norm)  / (2 * SquaredSigma));
            }

            return output;
        }

        /// <summary>
        /// Do Time-based inference for the given input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Vector TimeInfer(SparseMatrix input)
        {
            State = NodeState.TimeBasedInference;

            if (LearnedCoincidences.Length == 0)
            {
                return new SparseVector(1);
            }

            //TODOlater tbi infer

            return new SparseVector(1);
        }



        public override SpatialNode<SparseMatrix, Vector> GetClone()
        {
            var node = new SpatialNode2DGaussian();
            node.SquaredSigma = this.SquaredSigma;

            return node;
        }
    }
}
