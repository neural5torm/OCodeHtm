using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CnrsUniProv.OCodeHtm;
using System.Diagnostics;
using CnrsUniProv.OCodeHtm.Exceptions;
using System.Drawing;

namespace CnrsUniProv.OCodeHtm.UnitTests
{
    [TestClass]
    public class BitmapPictureSensorTest
    {
        static readonly string TrainingSetPath = Path.Combine("O:", "clean");
        const int TrainingSetSize = 26;


        [TestMethod]
        public void InputFilesCanBeFound()
        {
            var dir = new DirectoryInfo(Path.Combine(TrainingSetPath, "a"));

            var nbFiles = dir.GetFiles("*.bmp").Length;

            Assert.AreNotEqual(0, nbFiles);
            Assert.AreEqual(1, nbFiles);
        }

        [TestMethod]
        public void ErrorWhenTrainingFolderNotFound()
        {
            var sensor = new BitmapPictureSensor();
            sensor.SetTrainingFolder("stuff.xyz");
            try
            {
                foreach (var input in sensor.GetTrainingInputs())
                {

                }
                Assert.Fail("No exception thrown while parsing training inputs with inexistent training folder");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(DirectoryNotFoundException), e.ToString());
            }

        }

        [TestMethod]
        public void NoErrorWhenTrainingFolderEmpty()
        {
            var sensor = new BitmapPictureSensor();
            sensor.SetTrainingFolder(".");
            try
            {
                foreach (var input in sensor.GetTrainingInputs())
                {
                    Assert.Fail("This folder should be empty for testing purposes: " + sensor.TrainingFolder);
                }
            }
            catch (Exception e)
            {
                Assert.Fail("Should not throw an exception when an empty folder is used for training: " + e.ToString());
            }
        }

        [TestMethod]
        public void ErrorWhenTrainingFolderNotSet()
        {
            var sensor = new BitmapPictureSensor();

            try
            {
                foreach (var input in sensor.GetTrainingInputs())
                {

                }
                Assert.Fail("No exception thrown while parsing training inputs with no training folder set");
            }
            catch (Exception e)
            {

                Assert.IsInstanceOfType(e, typeof(HtmException), e.Message);
            }
            
        }


        [TestMethod]
        public void ErrorWhenTestFoldersNotSet()
        {
            var sensor = new BitmapPictureSensor();

            try
            {
                foreach (var input in sensor.GetTestInputs())
                {

                }
                Assert.Fail("No exception thrown while parsing test inputs with no test folders set");
            }
            catch (Exception e)
            {

                Assert.IsInstanceOfType(e, typeof(HtmException), e.Message);
            }

        }


        [TestMethod]
        public void CanGetTestInputsWithNormalOrder()
        {
            var sensor = new BitmapPictureSensor(trainingOrder: TrainingOrder.Normal);
            sensor.AddTestFolder(TrainingSetPath);
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

            Assert.AreEqual(TrainingSetSize, nbInputs);
        }

        [TestMethod]
        public void CanGetTestInputsWithRandomOrderSameAsNormal()
        {
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, 1, TrainingOrder.Normal);
            sensor.AddTestFolder(TrainingSetPath);
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

            Assert.AreEqual(TrainingSetSize, nbInputs);


            // Compare with normal order sequence
            var normalCategories = new List<string>();
            var normalSensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, 1, TrainingOrder.Normal);
            normalSensor.AddTestFolder(TrainingSetPath);
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
            sensor.SetTrainingFolder(TrainingSetPath);
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

            Assert.AreEqual(nbRepetitions * TrainingSetSize, nbInputs);
        }


        [TestMethod]
        public void CanGetTrainingInputsNoTransformationsWithoutFiltersInRandomOrder()
        {
            int nbRepetitions = 2;
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, 0, nbRepetitions, TrainingOrder.Random);
            sensor.SetTrainingFolder(TrainingSetPath);
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

            Assert.AreEqual(nbRepetitions * TrainingSetSize, nbInputs);


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
            sensor.SetTrainingFolder(TrainingSetPath);
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

            Assert.AreEqual(nbRepetitions * TrainingSetSize, nbInputs);


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
        public void CanGetTrainingInputsNoTransformationsWithoutFiltersInReverseOrder()
        {
            //TODO CanGetTrainingInputsNoTransformationsWithoutFiltersInReverseOrder test
        }

        [TestMethod]
        public void CanGetTrainingInputsNoTransformationsWithoutFiltersInSameRandomOrderWithSeed()
        {
            int nbRepetitions = 2;
            var sensor = new BitmapPictureSensor(Default.AutomaticSize, Default.AutomaticSize, Default.RandomizerSeed, nbRepetitions, TrainingOrder.Random);
            sensor.SetTrainingFolder(TrainingSetPath);
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

            Assert.AreEqual(nbRepetitions * TrainingSetSize, nbInputs);


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

        [TestMethod]
        public void CanGetTrainingInputsWithTransformationsNoFiltersInNormalOrder()
        { 
            var sensor = new BitmapPictureSensor();
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

        //TODO tests with filters(&transformations)
    }
}
