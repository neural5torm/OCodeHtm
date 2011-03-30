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
        public void CanGetTestInputs()
        {
            var sensor = new BitmapPictureSensor();
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
    }
}
