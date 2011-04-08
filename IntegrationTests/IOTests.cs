using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CnrsUniProv.OCodeHtm.IntegrationTests
{
    [TestClass]
    public class IOTests
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
        public void CanOutputBitmapFilesFromSensorGeneratedImages()
        {
            //TODOnow! CanOutputBitmapFilesFromSensorGeneratedImages test
        }

        [TestMethod]
        public void CanOutputTextFilesFromSensorTransformedInputs()
        {
            //TODOnow CanOutputTextFilesFromSensorTransformedInputs test
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
