using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.LinearAlgebra.Double;

namespace CnrsUniProv.OCodeHtm
{
    public abstract class SpatialNode2D : SpatialNode<SparseMatrix, SparseVector> 
    {
        public SpatialNode2D(double maxDistance, uint maxOutputSize)
            : base(maxDistance, maxOutputSize)
        { }

    }


}
