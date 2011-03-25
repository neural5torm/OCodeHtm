using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        Gaussian, //Product, //TODO
    }

    public enum TemporalLayerType
    {
        //MaxProp, //SumProp, //TimeBased //TODO
    }

    public enum ExplorationPath
    {
        RandomSweep4Axes, LeftRightSweep, TopDownSweep,
    }
    
    
    
    
    public static class Default
    {
        public const int MaxNodeOutputSize = 1000;

        public const double MaxDistance = 0.0;
        public const double NoSigma = 0.0;
        public const double SquaredSigma = 0.1;

        public const string CategoryFolderPattern = "(?<cat>.*)";
        public const string TrainingFolderString = "clean";
        public const string TestFolderString = "test";
        public const string InputFilenameFormat = "*";
        public const string BitmapInputFilenameFormat = "*.bmp";

        public const int RandomizerSeed = 0;
        public const int NoRandomizerSeed = -1;

        public const ExplorationPath Path = ExplorationPath.RandomSweep4Axes;
        public const bool PathUseRandomOrigin = true;
        public const int NoMaxIterations = int.MaxValue;
        public const int PathSpeed = 1;

        public const double NoRandomRotationAngle = 0.0;
        public const double NoRandomRotationSpeed = 0.0;

        public const double NoRandomScalingFactor = 1.0;
        public const double NoRandomScalingSpeed = 0.0;

        public const bool UseTransformationsForTraining = true;
        public const bool DontUseTransformationsForTesting = false;
    }
}
