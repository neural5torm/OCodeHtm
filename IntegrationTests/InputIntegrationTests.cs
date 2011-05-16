using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CnrsUniProv.OCodeHtm.Interfaces;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Diagnostics;

namespace CnrsUniProv.OCodeHtm.IntegrationTests
{
    [TestClass]
    public class InputIntegrationTests
    {
        static readonly string TrainingSetPath = Path.Combine("O:", "clean");
        const int TrainingSetSize = 26;


        #region BitmapPicture2DSensor inputs
        
        [TestMethod]
        public void InputFilesCanBeFound()
        {
            var dir = new DirectoryInfo(Path.Combine(TrainingSetPath, "a"));

            var nbFiles = dir.GetFiles("*.bmp").Length;

            Assert.AreNotEqual(0, nbFiles);
            Assert.AreEqual(1, nbFiles);
        }

        [TestMethod]
        public void CanOutputBitmapFilesFromSensorGeneratedImagesUsingTwoSensorsAndOneOutputFolder()
        {
            var sensor = new BitmapPicture2DSensor(presentationsPerInput: 1, pathSpeed: 2);
            sensor.SetTrainingFolder(TrainingSetPath);
            var writer = new MatrixToBitmapFileWriter("SameFolder");
            sensor.OnTransformedMatrixOutput += writer.OutputWriterHandler;

            var nbIterations = 0;

            foreach (var input in sensor.GetTrainingInputs(true))
            {
                foreach (var iteration in input)
                {
                    nbIterations++;
                }
            }

            Assert.AreEqual(nbIterations, writer.OutputFolder.GetFiles().Length);


            var sensor2 = new BitmapPicture2DSensor(presentationsPerInput: 1, pathSpeed: 2);
            sensor2.SetTrainingFolder(TrainingSetPath);
            var writer2 = new MatrixToBitmapFileWriter("sameFolder");
            sensor2.OnTransformedMatrixOutput += writer2.OutputWriterHandler;

            var nbIterations2 = 0;

            foreach (var input in sensor2.GetTrainingInputs(true))
            {
                foreach (var iteration in input)
                {
                    nbIterations2++;
                }
            }

            Assert.AreEqual(nbIterations2, writer2.OutputFolder.GetFiles().Length);
        }

        [TestMethod]
        public void GenerateInputsAlwaysInsideSensor()
        {
            var sensor = new BitmapPicture2DSensor(pathSpeed: 100);
            sensor.SetTrainingFolder(TrainingSetPath);
            var writer = new MatrixToBitmapFileWriter();
            sensor.OnTransformedMatrixOutput += writer.OutputWriterHandler;
            var nbInputIterationsOutside = 0;

            foreach (var input in sensor.GetTrainingInputs(true))
            {
                Assert.IsNotNull(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));

                foreach (var iteration in input)
                {
                    if (sensor.IsCurrentInputOutsideField())
                        nbInputIterationsOutside++;
                }
            }

            Assert.AreEqual(0, nbInputIterationsOutside);
        }

        [TestMethod]
        public void CanOutputBitmapFilesFromSensorTransformedInputsWithTranslation()
        {
            var sensor = new BitmapPicture2DSensor(presentationsPerInput: 1, pathSpeed: 2, useRandomOrigin: false);
            sensor.SetTrainingFolder(TrainingSetPath);
            var writer = new MatrixToBitmapFileWriter("");
            sensor.OnTransformedMatrixOutput += writer.OutputWriterHandler;

            var realNbInputs = sensor.TrainingFolder.EnumerateFiles("*.bmp", SearchOption.AllDirectories).Count();

            var nbIterations = 0;
            var nbInputs = 0;

            foreach (var input in sensor.GetTrainingInputs(true))
            {
                bool newInput = true;
                foreach (var iteration in input)
                {
                    if (newInput)
                    {
                        nbInputs++;
                        newInput = false;
                    }
                    nbIterations++;
                }
            }

            Assert.AreEqual(realNbInputs, nbInputs, "nbInputs");
            Assert.AreEqual(nbIterations, writer.OutputFolder.GetFiles().Length, "nbIterations");
        }

        [TestMethod]
        public void CanOutputBitmapFilesFromSensorTransformedInputsWithTranslationRotationAndScaling()
        {
            var sensor = new BitmapPicture2DSensor(presentationsPerInput: 1, pathSpeed: 2, 
                rotationAngleMaxDegrees:180.0f, rotationSpeed:10.0f, scalingMin:0.5f, scalingMax:2.0f, scalingSpeed:0.1f);
            sensor.SetTrainingFolder(TrainingSetPath);
            var writer = new MatrixToBitmapFileWriter("");
            sensor.OnTransformedMatrixOutput += writer.OutputWriterHandler;

            var nbIterations = 0;

            foreach (var input in sensor.GetTrainingInputs(true))
            {
                foreach (var iteration in input)
                {
                    nbIterations++;
                }
            }

            Assert.AreEqual(nbIterations, writer.OutputFolder.GetFiles().Length);
        }

        [TestMethod]
        public void CanOutputFilterFromGabor2DFilter()
        {
            var filter = new Gabor2DFilter();
            var writer = new MatrixToBitmapFileWriter("");
            filter.OnFilterCreated += writer.OutputWriterHandler;

            var r = filter.FilterMatricesReal.Length;
            var i = filter.FilterMatricesImaginary.Length;

            Assert.AreEqual(r + i, writer.OutputFolder.GetFiles().Length);
        }

        [TestMethod]
        public void CanOutputFilteredInputsFromGabor2DFilter()
        {
            var filter = new Gabor2DFilter();
            var writer = new MatrixToBitmapFileWriter("");

            var sensor = new BitmapPicture2DSensor(presentationsPerInput: 1, pathSpeed: 2, useRandomOrigin: true);
            sensor.SetTrainingFolder(TrainingSetPath);
            sensor.AddFilter(filter);
            sensor.OnFilteredMatrixOutput += writer.OutputWriterHandler;
            int maxIterations = 100, nbIterations = 0;

            var begin = DateTime.Now;
            foreach (var input in sensor.GetTrainingInputs(true))
            {
                foreach (var iteration in input)
                {
                    nbIterations++;
                    if (nbIterations >= maxIterations)
                        break;
                }
                if (nbIterations >= maxIterations)
                    break;
            }
            var end = DateTime.Now;

            Debug.WriteLine(begin);
            Debug.WriteLine("{0} (duration: {1:D} ms)", end, (int)(end - begin).TotalMilliseconds);

            Assert.AreEqual(maxIterations, writer.OutputFolder.GetFiles().Length);
        }

        [TestMethod]
        public void CanGetTrainingInputsWithTransformationsNoFiltersInNormalOrder()
        {
            var sensor = new BitmapPicture2DSensor();
            sensor.SetTrainingFolder(TrainingSetPath);

            var nbInputs = 0;


            foreach (var input in sensor.GetTrainingInputs(true))
            {
                Assert.IsNotNull(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));

                foreach (var iteration in input)
                {
                    nbInputs++;
                }
            }

            Assert.IsTrue(nbInputs > TrainingSetSize);
        }

        [TestMethod]
        public void CanGetTrainingInputsWithTransformationsAndGaborFilterInNormalOrder()
        {
            var sensor = new BitmapPicture2DSensor(pathSpeed: 10, useRandomOrigin: true);
            sensor.SetTrainingFolder(TrainingSetPath);
            sensor.AddFilter(new Gabor2DFilter());
            var nbInputs = 0;
            var nbIterations = 0;


            foreach (var input in sensor.GetTrainingInputs(true))
            {
                Assert.IsNotNull(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));

                nbInputs++;

                foreach (var iteration in input)
                {
                    if (!iteration.IsBlank())
                        nbIterations++;
                }

            }

            Assert.AreEqual(TrainingSetSize, nbInputs, "nbInputs");
            Assert.IsTrue(nbIterations >= TrainingSetSize, "nbIterations");
        }

        [TestMethod]
        public void SpatialLayer2DGaussianCanLearnAndInferFromBitmapPicture2DSensor()
        {
            var sensor = new BitmapPicture2DSensor(presentationsPerInput: 1, pathSpeed: 2, useRandomOrigin: false);
            sensor.SetTrainingFolder(TrainingSetPath);

            var layer = new Spatial2DLayer(SpatialLayerType.Gaussian, 16, 16, 0.1, true);

            foreach (var input in sensor.GetTrainingInputs(true))
            {
                foreach (var iteration in input)
                {
                    layer.Learn(iteration);
                }
            }
            
            Assert.IsTrue(((SpatialNode2DGaussian)layer.ClonedNode).CoincidencesFrequencies.Keys.Count > 0);
        }

        #endregion
    } // testc
}
