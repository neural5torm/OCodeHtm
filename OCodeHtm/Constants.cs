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
        Gaussian, //Product,
    }

    public enum TemporalLayerType
    {
        //MaxProp, SumProp, TimeBased
    }

    public enum SensorExplorationMode
    {
        RandomSweep4, RandomSweep8, LeftRightSweep, TopDownSweep,
    }
    
    
    
    
    internal static class Default
    {
        public const int MaxNodeOutputSize = 1000;
        public const double MaxDistance = 0.0;
        public const double NoSigma = 0.0;
        public const double SquaredSigma = 0.1;
    }
}
