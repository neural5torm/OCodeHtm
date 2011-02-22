using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Exceptions;

namespace CnrsUniProv.OCodeHtm
{
    public class GaussianSpatialNode : SpatialNode2D
    {

        public GaussianSpatialNode(double maxDistance = 0, uint maxOutputSize = 0)
            : this(new uint[] { 1, 1 }, maxDistance, maxOutputSize)
        { }

        public GaussianSpatialNode(uint[] childNodeArrayDims, double maxDistance = 0, uint maxOutputSize = 0)
            : base(childNodeArrayDims, maxDistance * maxDistance, maxOutputSize)
        { }




        public override void Learn(SparseMatrix input)
        {
            // Limitation due to HTM v1.x design
            if (!IsLearning)
                throw new HtmRuleException("Cannot learn after any other mode than learning", this);

            // Ignore blank input 
            //TODOlater treat any input with identical values for *all* components as blank?
            if (input.NonZerosCount == 0)
            { return; }

            SparseMatrix existingCoincidence = FindClosestCoincidence(input);

            if (existingCoincidence != null)
                CoincidencesFrequencies[existingCoincidence] += 1;
            else
                CoincidencesFrequencies[input] = 1;
        }



        private SparseMatrix FindClosestCoincidence(SparseMatrix input)
        {
            SparseMatrix canonical;
            return FindClosestCoincidence(input, out canonical);
        }
        protected override SparseMatrix FindClosestCoincidence(SparseMatrix input, out SparseMatrix canonicalInput)
        {
            canonicalInput = input; // no Winner-Take-All

            if (CoincidencesFrequencies.ContainsKey(input))
                return input;
            
            // else
            if (MaxSquaredDistance <= 0)
                return null;

            // else
            // Look for the 1st coincidence with a distance to the input below MaxDistance 
            // (which SHOULD be the closest since each learned coincidence has to be distant to all others by MaxDistance)
            var closestCoincidence = input;
            closestCoincidence = null;

            

            foreach (var coincidence in CoincidencesFrequencies.Keys)
            {
                // Compute distance between matrices and return coincidence, if found
                try
                {
                    var diff = coincidence.Subtract(input);
                    if (diff.PointwiseMultiply(diff).L1Norm() <= MaxSquaredDistance)
                        return coincidence;
                }
                catch (Exception e)
                {
                    throw new HtmRuleException("Cannot compare differently-sized input and coincidence matrices", this, e);
                }
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do Flash inference for the given input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override SparseVector Infer(SparseMatrix input)
        {
            Mode = NodeMode.FlashInference;

            //TODONOW infer

            return new SparseVector(1);
        }

        /// <summary>
        /// Do Time-based inference for the given input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override SparseVector TimeBasedInfer(SparseMatrix input)
        {
            Mode = NodeMode.TimeBasedInference;

            //TODOlater tbi infer

            return new SparseVector(1);
        }
    }
}
