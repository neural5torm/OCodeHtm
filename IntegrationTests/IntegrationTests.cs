using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CnrsUniProv.OCodeHtm.Interfaces;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CnrsUniProv.OCodeHtm.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        static readonly string TrainingSetPath = Path.Combine("O:", "clean");


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
            var sensor = new BitmapPictureSensor(presentationsPerInput:2, pathSpeed: 2);
            sensor.SetTrainingFolder(TrainingSetPath);
            var writer = new BitmapFileWriter("");
            sensor.OnTransformedBitmapOutput += writer.OutputWriterHandler;

            var nbIterations = 0;

            foreach (var input in sensor.GetTrainingInputs(true))
            {
                foreach (var iteration in input)
                {
                    nbIterations++;
                }
            }

            Assert.AreEqual(nbIterations, writer.OutputFolder.GetFiles().Length);


            var sensor2 = new BitmapPictureSensor(presentationsPerInput: 1, pathSpeed: 4);
            sensor2.SetTrainingFolder(TrainingSetPath);
            var writer2 = new BitmapFileWriter("");
            sensor2.OnTransformedBitmapOutput += writer2.OutputWriterHandler;

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
        public void CanOutputBitmapFilesFromSensorTransformedInputsWithTranslation()
        {
            var sensor = new BitmapPictureSensor(presentationsPerInput: 1, pathSpeed: 2);
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
        public void CanOutputBitmapFilesFromSensorTransformedInputsWithTranslationRotationAndScaling()
        {
            var sensor = new BitmapPictureSensor(presentationsPerInput: 1, pathSpeed: 2, 
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
            //TODOnow! check correctness
            var filter = new Gabor2DFilter(4, 1.0);
            var writer = new MatrixToBitmapFileWriter("");
            filter.OnFilterCreated += writer.OutputWriterHandler;


            for (int i = 0; i < filter.FilterMatricesReal.Length; i++)
            {
                var real = filter.FilterMatricesReal[i];
                var imag = filter.FilterMatricesImaginary[i];

                var filterNorm = (Matrix)real.PointwiseApply((d) => Math.Pow(d, 2.0)).Add(imag.PointwiseApply((d) => Math.Pow(d, 2.0)));
                filterNorm = (Matrix)filterNorm.PointwiseApply((d) => Math.Pow(d, 0.5));

                writer.OutputWriterHandler(this, new OutputEventArgs<Matrix>(filterNorm, "normedFilter"));
            }
        }

        [TestMethod]
        public void GaussianSpatialLayerCanLearnAndInferFromPictureSensor()
        {
            //TODO GaussianSpatialLayerCanLearnAndInferFromPictureSensor
        }

        [TestMethod]
        public void NetworkCanLearnAndBeTestedAgainstInputs()
        {
            //TODOlater
            /*
            void main()
            {
                var net = new HtmNetwork();

                net.AddSensor(new BitmapPictureSensor(32, 32, "ocode/clean", "ocode/test"));
                net.AddLayer(new SpatialLayer<SpatialNodeGaussian>(20, 20, 0.0));
                net.AddLayer(new TemporalLayer(20, 20, 0.0));
                net.AddClassifier(new ProductMatchClassifierNode());

                net.Sensor.Exploration = "RandomSweep";

                Train(net);

            }

            void Train(object net)
            {
                // Train layers
                for (int i = 0; i < net.Layers.Count; ++i)
                {
                    foreach (var input in net.Sensor.GetTrainingInputs())
                    {
                        foreach (var sensorSweepIteration in input.GetIterations())
                        {
                            // Get output from all previously trained layers
                            var output = sensorSweepIteration.GetOutput();
                            for (int iTrained = 0; j < i; ++j)
                            {
                                output = net.Layers[iTrained].Infer(output);
                            }

                            net.Layers[i].Learn(output);
                        }
                    }
                }

                // Train classifier
                foreach (var input in net.Sensor.GetTrainingInputs())
                {
                    var cat = input.GetCategory();

                    foreach (var sensorSweepIteration in input.GetIterations())
                    {
                        var output = sensorSweepIteration.GetOutput();
                        for (int i = 0; i < net.Layers.Count; ++i)
                        {
                            output = net.Layers[i].Infer(output);
                        }

                        net.Classifier.Learn(output, cat);
                    }
                }
            }*/
    
        }

    } // testc
}
