using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CnrsUniProv.OCodeHtm
{
    public class GaussianSpatialNode : SpatialNode2D
    {
        public GaussianSpatialNode(double maxDistance = 0, uint maxOutputSize = 0)
            : base(maxDistance, maxOutputSize)
        {

        }




        public override void Learn(SparseMatrix input)
        {
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
            if (MaxSqDistance <= 0)
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
                if (diff.PointwiseMultiply(diff).L1Norm() <= MaxSqDistance)
                    return coincidence;
            }

            // not found
            return null;
        }


        public override SparseVector Infer(SparseMatrix input)
        {
            //TODONOW infer
            throw new NotImplementedException();
        }
    }
}
