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
        //MaxProp, //SumProp, //TimeBased //TODOlater
    }

    public enum ExplorationPath
    {
        //2D:
        RandomSweep4Axes, LeftToRightSweep, TopToBottomSweep,
        
        //1D:
        //BeginningToEndSweep, EndToBeginningSweep, ReboundSweep
        
        //3D?
    }

    public enum TrainingOrder
    {
        Random, Normal, Reverse, RandomAll
    }
    
    public enum MultipleOutputFilterMode
    {
        Concatenated, Interleaved, SuperImposed,
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
        public const string InputFilenameMask = "*";
        public const string BitmapInputFilenameMask = "*.bmp";

        public const int AutomaticSize = 0;

        public const int RandomizerSeed = 0;
        public const int NoRandomizerSeed = -1;

        public const int SinglePresentation = 1;

        public const bool PathUseRandomOrigin = true;
        public const int NoMaxIterations = int.MaxValue;
        public const int PathSpeed = 1;

        public const float NoRandomRotationAngle = 0.0f;
        public const float NoRandomRotationSpeed = 0.0f;

        public const float NoRandomScalingFactor = 1.0f;
        public const float NoRandomScalingSpeed = 0.0f;

        public const bool UseTransformationsForTraining = true;
        public const bool DontUseTransformationsForTesting = false;

        public const int GaborOrientations = 4;
        public const double SquareAspectRatio = 1.0;
    }
}
