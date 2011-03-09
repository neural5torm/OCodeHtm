using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm
{
    public enum NodeState
    {
        Learning, FlashInference, TimeBasedInference,
    }

    public enum LayerState
    {
        Learning, Trained,
    }

    public enum SpatialLayerType
    {
        Gaussian,
    }

    internal static class Default
    {
        public const int MaxNodeOutputSize = 1000;
        public const double MaxDistance = 0.0;
        public const double Sigma = 1.0;
    }
}
