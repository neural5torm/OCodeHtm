using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Interfaces;
using System.Threading.Tasks;

namespace CnrsUniProv.OCodeHtm
{
    public class SpatialLayer : HtmLayer2D
    {
        public SpatialLayerType Type { get; private set; }
        
        public double MaxDistance { get; private set; }
        public double Sigma { get; private set; }




        public SpatialLayer(SpatialLayerType type, uint height, uint width, double overlap, bool clone, uint maxOutputSize, 
                            double sigma = Default.Sigma, double maxDistance = Default.MaxDistance)
            : this(type, height, width, overlap, clone, maxOutputSize)
        {
            MaxDistance = maxDistance;
            Sigma = sigma;
        }

        public SpatialLayer(SpatialLayerType type, uint height, uint width, double overlap, bool clone, uint maxOutputSize = Default.MaxNodeOutputSize)
            : base(height, width, overlap, clone, maxOutputSize)
        { 
            Type = type;
        }




        protected override void CreateNodes()
        {
            if (IsTrainedNodeCloned)
                ClonedNode = CreateNode();
            else
            {
                NodeArray = new INode<SparseMatrix, Vector>[Height, Width];
                Parallel.For(0, Height, (row) =>
                {
                    Parallel.For(0, Width, (col) =>
                    {
                        NodeArray[row, col] = CreateNode();
                    });
                });
            }
        }


        protected INode<SparseMatrix, Vector> CreateNode()
        {
            switch (Type)
            {
                case SpatialLayerType.Gaussian:
                    return new SpatialNodeGaussian(MaxDistance, Sigma, MaxNodeOutputSize);
                default:
                    return null;
            }
        }

        protected override void CloneTrainedNode()
        {
            NodeArray = new INode<SparseMatrix, Vector>[Height, Width];
            Parallel.For(0, Height, (row) =>
            {
                Parallel.For(0, Width, (col) =>
                {
                    NodeArray[row, col] = ClonedNode.Clone();
                });
            });
        }
    }
}
