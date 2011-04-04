using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CnrsUniProv.OCodeHtm;
using System.Diagnostics;

namespace OCodeHTM_UnitTests
{
    [TestClass]
    public class BitmapPictureSensorTest
    {
        readonly string InputPath = Path.Combine("O:", "clean");
        const int InputSetSize = 26; 

        [TestMethod]
        public void InputFilesCanBeFound()
        {
            var dir = new DirectoryInfo(Path.Combine(InputPath, "a"));

            var nbFiles = dir.GetFiles("*.bmp").Length;

            Assert.AreNotEqual(0, nbFiles);
            Assert.AreEqual(1, nbFiles);
        }

        [TestMethod]
        public void CanGetTestInputsWithNormalOrder()
        {
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, 1, TrainingOrder.Normal);
            sensor.AddTestFolder(InputPath);
            var nbInputs = 0;


            foreach (var input in sensor.GetTestInputs())
            {
                Assert.IsNotNull(input.CurrentFile);
                //Debug.WriteLine(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));

                foreach (var iteration in input)
                {
                    nbInputs++;
                }
            }

            Assert.AreEqual(InputSetSize, nbInputs);
        }

        [TestMethod]
        public void CanGetTestInputsWithRandomOrderSameAsNormal()
        {
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, 1, TrainingOrder.Normal);
            sensor.AddTestFolder(InputPath);
            var nbInputs = 0;
            var categories = new List<string>();

            foreach (var input in sensor.GetTestInputs())
            {
                Assert.IsNotNull(input.CurrentFile);
                //Debug.WriteLine(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));
                categories.Add(input.CategoryName);

                foreach (var iteration in input)
                {
                    nbInputs++;
                }
            }

            Assert.AreEqual(InputSetSize, nbInputs);


            // Compare with normal order sequence
            var normalCategories = new List<string>();
            var normalSensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, 1, TrainingOrder.Normal);
            normalSensor.AddTestFolder(InputPath);
            foreach (var input in normalSensor.GetTestInputs(false))
            {
                normalCategories.Add(input.CategoryName);
            }

            Assert.IsTrue(categories.SequenceEqual(normalCategories));
        }


        [TestMethod]
        public void CanGetTrainingInputsNoTransformationsWithoutFiltersInNormalOrder()
        {
            int nbRepetitions = 2;
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, nbRepetitions, TrainingOrder.Normal);
            sensor.SetTrainingFolder(InputPath);
            var nbInputs = 0;


            foreach (var input in sensor.GetTrainingInputs(false))
            {
                Assert.IsNotNull(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));
                
                foreach (var iteration in input)
                {
                    nbInputs++;
                }
            }

            Assert.AreEqual(nbRepetitions * InputSetSize, nbInputs);
        }


        [TestMethod]
        public void CanGetTrainingInputsNoTransformationsWithoutFiltersInRandomOrder()
        {
            int nbRepetitions = 2;
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, nbRepetitions, TrainingOrder.Random);
            sensor.SetTrainingFolder(InputPath);
            var nbInputs = 0;
            var categories = new List<string>();


            foreach (var input in sensor.GetTrainingInputs(false))
            {
                Assert.IsNotNull(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));
                categories.Add(input.CategoryName);

                foreach (var iteration in input)
                {
                    nbInputs++;
                }
            }

            Assert.AreEqual(nbRepetitions * InputSetSize, nbInputs);


            // Compare with normal order sequence
            var normalCategories = new List<string>();
            var normalSensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, nbRepetitions, TrainingOrder.Normal);
            normalSensor.SetTrainingFolder(sensor.TrainingFolder.FullName);
            foreach (var input in normalSensor.GetTrainingInputs(false))
            {
                normalCategories.Add(input.CategoryName);
            }

            Assert.IsFalse(categories.SequenceEqual(normalCategories));
        }


        [TestMethod]
        public void CanGetTrainingInputsNoTransformationsWithoutFiltersInRandomAllOrder()
        {
            int nbRepetitions = 2;
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, nbRepetitions, TrainingOrder.RandomAll);
            sensor.SetTrainingFolder(InputPath);
            var nbInputs = 0;
            var categories = new List<string>();


            foreach (var input in sensor.GetTrainingInputs(false))
            {
                Assert.IsNotNull(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));
                categories.Add(input.CategoryName);

                foreach (var iteration in input)
                {
                    nbInputs++;
                    Debug.WriteLine("RandomAll order: " + input.CategoryName);
                }
            }

            Assert.AreEqual(nbRepetitions * InputSetSize, nbInputs);


            // Compare with normal order sequence
            var randomCategories = new List<string>();
            var randomSensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, nbRepetitions, TrainingOrder.Random);
            randomSensor.SetTrainingFolder(sensor.TrainingFolder.FullName);
            foreach (var input in randomSensor.GetTrainingInputs(false))
            {
                randomCategories.Add(input.CategoryName);
                foreach (var iteration in input)
                {
                    Debug.WriteLine("Random order: " + input.CategoryName);
                }
            }

            Assert.IsFalse(categories.SequenceEqual(randomCategories));
        }


        [TestMethod]
        public void CanGetTrainingInputsNoTransformationsWithoutFiltersInSameRandomOrderWithSeed()
        {
            int nbRepetitions = 2;
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, Default.RandomizerSeed, nbRepetitions, TrainingOrder.Random);
            sensor.SetTrainingFolder(InputPath);
            var nbInputs = 0;
            var categories = new List<string>();


            foreach (var input in sensor.GetTrainingInputs(false))
            {
                Assert.IsNotNull(input.CurrentFile);
                Assert.IsFalse(string.IsNullOrWhiteSpace(input.CategoryName));
                categories.Add(input.CategoryName);

                foreach (var iteration in input)
                {
                    nbInputs++;
                    Debug.WriteLine("Random order: " + input.CategoryName);
                }
            }

            Assert.AreEqual(nbRepetitions * InputSetSize, nbInputs);


            // Compare with normal order sequence
            var controlCategories = new List<string>();
            var controlSensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, Default.RandomizerSeed, nbRepetitions, TrainingOrder.Random);
            controlSensor.SetTrainingFolder(sensor.TrainingFolder.FullName);
            foreach (var input in controlSensor.GetTrainingInputs(false))
            {
                controlCategories.Add(input.CategoryName);
                foreach (var iteration in input)
                {
                    Debug.WriteLine("Control random order: " + input.CategoryName);
                }
            }

            Assert.IsTrue(categories.SequenceEqual(controlCategories));
        }

        //TODO tests with transformations &| filters
    }
}
