using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CnrsUniProv.OCodeHtm
{
    public abstract class Sensor<TOutput> 
    {
        public class InputIterations : IEnumerable<TOutput>
        {
            public Sensor<TOutput> Sensor { get; private set; }
            
            public bool UseTransformations { get; private set; }
            public FileInfo CurrentFile { get; private set; }
            public string CategoryName { get; private set; }


            public InputIterations(Sensor<TOutput> sensor, FileInfo file, bool useTransformations, string category)
            {
                Sensor = sensor;
                UseTransformations = useTransformations;

                CurrentFile = file;
                CategoryName = category;
            }

            public IEnumerator<TOutput> GetEnumerator()
            {
                //TODOnow
                yield return default(TOutput);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        public class Inputs : IEnumerable<InputIterations>
        {
            private Sensor<TOutput> Sensor { get; set; }
            private bool UseTransformations { get; set; }

            private List<DirectoryInfo> TestFolders { get; set; }
            private DirectoryInfo TrainingFolder { get; set; }
            private Regex CategoryFolderPattern { get; set; }

            public bool IsTest
            {
                get { return TestFolders != null; }
            }


            public Inputs(Sensor<TOutput> sensor, bool useTransformations, List<DirectoryInfo> testFolders)
            {
                Sensor = sensor;
                UseTransformations = useTransformations;
                
                TestFolders = testFolders;
                TrainingFolder = null;
                CategoryFolderPattern = sensor.CategoryFolderPattern;
            }
            public Inputs(Sensor<TOutput> sensor, bool useTransformations, DirectoryInfo trainingFolder)
            {
                Sensor = sensor;
                UseTransformations = useTransformations;

                TrainingFolder = trainingFolder;
                TestFolders = null;
                CategoryFolderPattern = sensor.CategoryFolderPattern;
            }


            public IEnumerator<InputIterations> GetEnumerator()
            {
                if (IsTest)
                {
                    foreach (var testFolder in TestFolders)
                    {
                        foreach (var catFolder in testFolder.EnumerateDirectories())
                        {
                            foreach (var file in catFolder.GetFiles(Sensor.InputFilenameFormat))
                                yield return new InputIterations(Sensor, file, UseTransformations, GetCategoryFromFolder(catFolder));
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }



            private string GetCategoryFromFolder(DirectoryInfo dir)
            {
                return CategoryFolderPattern.Match(dir.Name).Groups["cat"].Value;
            }
        }


        public int ExplorationRandomizerSeed { get; private set; }
        private Random randomizer;
        private Random Randomizer
        {
            get
            {
                if (randomizer == null)
                    randomizer = ExplorationRandomizerSeed >= 0 ? new Random(ExplorationRandomizerSeed) : new Random();

                return randomizer;
            }
        }


        public List<ExplorationPath> ExplorationPaths { get; set; }
        public int ExplorationPathMaxIterations { get; private set; }
        public bool ExplorationPathUseRandomOrigin { get; private set; }
        /// <summary>
        /// Speed of image translation in pixels per iteration
        /// </summary>
        public int ExplorationPathSpeed { get; set; }

        
        public double ExplorationRandomRotationMaxAngle { get; private set; }
        /// <summary>
        /// Determines angle delta between two consecutive rotated iterations.
        /// </summary>
        public double ExplorationRandomRotationSpeed { get; private set; }
        public double ExplorationScalingMin { get; private set; }
        public double ExplorationScalingMax { get; private set; }
        /// <summary>
        /// Determines change of scale between two consecutive iterations, based on (ScalingMax - ScalingMin) range.
        /// Value between 0.0 and 1.0.
        /// </summary>
        public double ExplorationScalingSpeed { get; set; } 


        public DirectoryInfo TrainingFolder { get; private set; }
        public List<DirectoryInfo> TestFolders { get; private set; }

        public Regex CategoryFolderPattern { get; private set; }
        public string InputFilenameFormat { get; set; }




        public Sensor(int randomizerSeed, ExplorationPath[] explorationModes, uint maxIterations, uint pathSpeed, bool useRandomOrigin,
            double rotationAngleMaxDegrees = Default.NoRandomRotationAngle, double rotationSpeed = Default.NoRandomRotationSpeed,
            double scalingMin = Default.NoRandomScalingFactor, double scalingMax = Default.NoRandomScalingFactor, double scalingSpeed = Default.NoRandomScalingSpeed)
        {
            ExplorationRandomizerSeed = randomizerSeed;
            
            ExplorationPaths = explorationModes.ToList();
            ExplorationPathMaxIterations = (int)maxIterations;
            ExplorationPathUseRandomOrigin = useRandomOrigin;
            ExplorationPathSpeed = (int)pathSpeed;

            //TODOlater use rotation and scaling transformations
            ExplorationRandomRotationMaxAngle = Math.Abs(rotationAngleMaxDegrees);
            ExplorationRandomRotationSpeed = rotationSpeed;

            ExplorationScalingMin = Math.Abs(scalingMin);
            ExplorationScalingMax = Math.Abs(scalingMax);
            ExplorationScalingSpeed = scalingSpeed;
            
            // Default null objects:
            TestFolders = new List<DirectoryInfo>(); 
            InputFilenameFormat = Default.InputFilenameFormat;
            SetCategoryFolderPattern();
        }


        public Inputs GetTrainingInputs(bool useTransformations = Default.UseTransformationsForTraining)
        {
            //TODOnow test (with/without transformations/filters)
            return new Inputs(this, useTransformations, TrainingFolder);
        }

        public Inputs GetTestInputs(bool useTransformations = Default.DontUseTransformationsForTesting)
        {
            return new Inputs(this, useTransformations, TestFolders);
        }


        public void SetTrainingFolder(string path = Default.TrainingFolderString)
        {
            TrainingFolder = new DirectoryInfo(path);
        }
        public void AddTestFolder(string path = Default.TestFolderString)
        {
            TestFolders.Add(new DirectoryInfo(path));
        }
        public void SetCategoryFolderPattern(string pattern = Default.CategoryFolderPattern)
        {
            CategoryFolderPattern = new Regex(pattern);
        }
        

        private void ResetRandomizer()
        {
            randomizer = null; 
        }

        protected abstract void SetCurrentInputFile(FileInfo file);
        protected abstract TOutput GetNextIteration(bool useTransformations);
        protected abstract TOutput FilterInput(TOutput input);
    }
}
