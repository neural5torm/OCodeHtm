using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;


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
            sensor.OnBitmapOutput += writer.OutputWriterHandler;

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
            sensor2.OnBitmapOutput += writer2.OutputWriterHandler;

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
        public void CanOutputBitmapFilesFromSensorTransformedInputs()
        {
            var sensor = new BitmapPictureSensor(presentationsPerInput: 2, pathSpeed: 2);
            sensor.SetTrainingFolder(TrainingSetPath);
            var writer = new MatrixToBitmapFileWriter("");
            sensor.OnMatrixOutput += writer.OutputWriterHandler;

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
