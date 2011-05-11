using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public abstract class SpatialNode2D : SpatialNode<SparseMatrix, Vector> 
    {
        public SpatialNode2D(double maxSquaredDistance, int maxOutputSize)
            : base(maxSquaredDistance, maxOutputSize)
        { }

    }


}
