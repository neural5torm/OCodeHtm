using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm
{
    public abstract class Sensor<TInput>
    {
        public int ExplorationMaxIterations { get; private set; }

        public List<SensorExplorationMode> ExplorationModes { get; set; }

        public double ExplorationScalingMin { get; private set; }
        public double ExplorationScalingMax { get; private set; }

        public int ExplorationRandomizerSeed { get; private set; }
        private Lazy<Random> lazyrandomizer;
        private Random Randomizer
        {
            get
            {
                return lazyrandomizer.Value;
            }
            set
            {
                lazyrandomizer = new Lazy<Random>(() => value);
            }
        }

        public bool ExplorationRandomStartPosition { get; private set; }
        


        public Sensor(uint maxIterations, int seed, double scalingMin, double scalingMax, bool randomStartPos, params SensorExplorationMode[] modes)
        {
            ExplorationMaxIterations = (int)maxIterations;

            ExplorationScalingMin = scalingMin;
            ExplorationScalingMax = scalingMax;
            ExplorationRandomStartPosition = randomStartPos;

            ExplorationRandomizerSeed = seed;
            Randomizer = new Random(ExplorationRandomizerSeed);

            ExplorationModes = modes.ToList();

            //TODOnow
        }
        
        

        
    }
}
