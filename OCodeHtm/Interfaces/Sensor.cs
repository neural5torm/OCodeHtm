using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using CnrsUniProv.OCodeHtm.Exceptions;

namespace CnrsUniProv.OCodeHtm.Interfaces
{
    public abstract class Sensor<TOutput> 
    {
        public class ExplorationIterations : IEnumerable<TOutput>
        {
            public Sensor<TOutput> Sensor { get; private set; }
            
            public bool UseTransformations { get; private set; }
            public FileInfo CurrentFile { get; private set; }
            public string CategoryName { get; private set; }


            public ExplorationIterations(Sensor<TOutput> sensor, FileInfo file, bool useTransformations, string category)
            {
                Sensor = sensor;
                UseTransformations = useTransformations;

                CurrentFile = file;
                CategoryName = category;
            }


            public IEnumerator<TOutput> GetEnumerator()
            {
                Sensor.SetCurrentInputFile(CurrentFile, CategoryName);

                return Sensor.GetIterationsEnumerator(UseTransformations);
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public class Inputs : IEnumerable<ExplorationIterations>
        {
            private Sensor<TOutput> Sensor { get; set; }
            private bool UseTransformations { get; set; }

            private List<DirectoryInfo> TestFolders { get; set; }
            private DirectoryInfo TrainingFolder { get; set; }
            private Regex CategoryFolderPattern { get; set; }

            public bool IsTesting
            {
                get { return TestFolders != null; }
            }


            public Inputs(Sensor<TOutput> sensor, bool useTransformations, List<DirectoryInfo> testFolders)
            {
                Sensor = sensor;
                UseTransformations = useTransformations;
                
                TestFolders = testFolders;
                TrainingFolder = null;
                CategoryFolderPattern = sensor.TestCategoryFolderPattern;
            }
            public Inputs(Sensor<TOutput> sensor, bool useTransformations, DirectoryInfo trainingFolder)
            {
                Sensor = sensor;
                UseTransformations = useTransformations;

                TrainingFolder = trainingFolder;
                TestFolders = null;
                CategoryFolderPattern = sensor.TrainingCategoryFolderPattern;
            }


            public IEnumerator<ExplorationIterations> GetEnumerator()
            {
                if (IsTesting)
                {
                    if (TestFolders.Count == 0)
                        throw new HtmException("No test folder set", this);

                    // Browsing test sets
                    foreach (var testFolder in TestFolders)
                    {
                        // Browsing category folders
                        foreach (var catFolder in testFolder.EnumerateDirectories())
                        {
                            // Browsing input files in each category, ordered by ascending file name
                            foreach (var file in catFolder.GetFiles(Sensor.InputFilenameMask).OrderBy(file => file.Name))
                                yield return new ExplorationIterations(Sensor, file, UseTransformations, GetCategoryFromFolder(catFolder));
                        }
                    }
                }
                else // Training
                {
                    if (TrainingFolder == null)
                        throw new HtmException("No training folder set", this);

                    // Ordering cat. folders by category name in training set
                    var orderedCatFolders = TrainingFolder.EnumerateDirectories();

                    switch (Sensor.TrainingOrder)
                    {
                        case TrainingOrder.Normal:
                            orderedCatFolders = orderedCatFolders.OrderBy(catFolder => GetCategoryFromFolder(catFolder));
                            break;
                        case TrainingOrder.Random:
                            orderedCatFolders = orderedCatFolders.OrderBy(catFolder => Sensor.RandomGenerator.Next());
                            break;
                        case TrainingOrder.Reverse:
                            orderedCatFolders = orderedCatFolders.OrderByDescending(catFolder => GetCategoryFromFolder(catFolder));
                            break;
                        default:
                            break;
                    }

                    IEnumerable<DirectoryInfo> catFolders = orderedCatFolders.ToList();
                    if (Sensor.TrainingPresentationsPerInput > Default.SinglePresentation)
                    {
                        for (int times = 0; times < Sensor.TrainingPresentationsPerInput - 1; times++)
                            catFolders = catFolders.Concat(orderedCatFolders);
                    }

                    if (Sensor.TrainingOrder == TrainingOrder.RandomAll)
                        catFolders = catFolders.OrderBy(dir => Sensor.RandomGenerator.Next());


                    // Browsing ordered category folders
                    foreach (var catFolder in catFolders)
                    {
                        // Ordering input files in each category
                        IEnumerable<FileInfo> inputFiles = catFolder.GetFiles(Sensor.InputFilenameMask);
                        switch (Sensor.TrainingOrder)
                        {
                            case TrainingOrder.Normal:
                                inputFiles = inputFiles.OrderBy(file => file.Name);
                                break;
                            case TrainingOrder.Reverse:
                                inputFiles = inputFiles.OrderByDescending(file => file.Name);
                                break;
                            case TrainingOrder.Random:
                            case TrainingOrder.RandomAll:
                                inputFiles = inputFiles.OrderBy(file => Sensor.RandomGenerator.Next());
                                break;
                            default:
                                break;
                        }

                        // Browsing ordered input files
                        foreach (var file in inputFiles)
                            yield return new ExplorationIterations(Sensor, file, UseTransformations, GetCategoryFromFolder(catFolder));
                    }
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



        public int ExplorationRandomSeed { get; private set; }
        private Random randomGenerator;
        protected Random RandomGenerator
        {
            get
            {
                if (randomGenerator == null)
                    randomGenerator = ExplorationRandomSeed >= 0 ? new Random(ExplorationRandomSeed) : new Random();

                return randomGenerator;
            }
        }


        public List<ExplorationPath> ExplorationPaths { get; set; }
        public int ExplorationPathMaxIterations { get; private set; }
        public bool ExplorationPathUseRandomOrigin { get; private set; }
        /// <summary>
        /// Speed of image translation in pixels per iteration
        /// </summary>
        public int ExplorationPathSpeed { get; set; }

        
        public float ExplorationRandomRotationAngleMaxDegrees { get; private set; }
        /// <summary>
        /// Determines angle delta between two consecutive rotated iterations.
        /// </summary>
        public float ExplorationRandomRotationSpeed { get; private set; }
        public float ExplorationScalingMin { get; private set; }
        public float ExplorationScalingMax { get; private set; }
        /// <summary>
        /// Determines change of scale between two consecutive iterations, based on (ScalingMax - ScalingMin) range.
        /// Value between 0.0 and 1.0.
        /// </summary>
        public float ExplorationScalingSpeed { get; set; }

        public int TrainingPresentationsPerInput { get; private set; }
        public TrainingOrder TrainingOrder { get; private set; }

        
        public DirectoryInfo TrainingFolder { get; private set; }
        public List<DirectoryInfo> TestFolders { get; private set; }

        public Regex TrainingCategoryFolderPattern { get; private set; }
        public Regex TestCategoryFolderPattern { get; private set; }
        public string InputFilenameMask { get; set; }


        public List<IFilter<TOutput>> Filters { get; private set; }


        public Sensor(int randomizerSeed, int trainingPresentations, TrainingOrder trainingOrder,
            ExplorationPath[] explorationPaths, uint maxIterations, uint pathSpeed, bool useRandomOrigin,
            float rotationAngleMaxDegrees, float rotationSpeed, float scalingMin, float scalingMax, float scalingSpeed)
        {
            ExplorationRandomSeed = randomizerSeed;

            TrainingPresentationsPerInput = trainingPresentations;
            TrainingOrder = trainingOrder;

            ExplorationPaths = explorationPaths.ToList();
            ExplorationPathMaxIterations = (int)maxIterations;
            ExplorationPathUseRandomOrigin = useRandomOrigin;
            ExplorationPathSpeed = (int)pathSpeed;

            ExplorationRandomRotationAngleMaxDegrees = Math.Abs(rotationAngleMaxDegrees);
            ExplorationRandomRotationSpeed = rotationSpeed;

            ExplorationScalingMin = Math.Abs(scalingMin);
            ExplorationScalingMax = Math.Abs(scalingMax);
            ExplorationScalingSpeed = scalingSpeed;
            
            // Default null objects:
            Filters = new List<IFilter<TOutput>>();
            TestFolders = new List<DirectoryInfo>(); 
            
            InputFilenameMask = Default.InputFilenameMask;
            SetTrainingCategoryFolderPattern();
            SetTestCategoryFolderPattern();
        }



        public Inputs GetTrainingInputs(bool useTransformations = Default.UseTransformationsForTraining)
        {
            return new Inputs(this, useTransformations, TrainingFolder);
        }

        public Inputs GetTestInputs(bool useTransformations = Default.DontUseTransformationsForTesting)
        {
            return new Inputs(this, useTransformations, TestFolders);
        }


        protected abstract void SetCurrentInputFile(FileInfo file, string category);

        protected abstract IEnumerator<TOutput> GetIterationsEnumerator(bool useTransformations);



        /// <summary>
        /// Resets the random number generator to the ExplorationRandomSeed. Should be called between each layer training if the seed is fixed. 
        /// </summary>
        private void ResetRandomGenerator()
        {
            randomGenerator = null;
        }
        
        public void SetTrainingFolder(string path = Default.TrainingFolderString)
        {
            TrainingFolder = new DirectoryInfo(path);
        }

        public void AddTestFolder(string path = Default.TestFolderString)
        {
            TestFolders.Add(new DirectoryInfo(path));
        }

        public void SetTrainingCategoryFolderPattern(string pattern = Default.CategoryFolderPattern)
        {
            TrainingCategoryFolderPattern = new Regex(pattern);
        }
        public void SetTestCategoryFolderPattern(string pattern = Default.CategoryFolderPattern)
        {
            TestCategoryFolderPattern = new Regex(pattern);
        }

        public void AddFilter(IFilter<TOutput> filter)
        {
            Filters.Add(filter);
        }
        
    }
}
