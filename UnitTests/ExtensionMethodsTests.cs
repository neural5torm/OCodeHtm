using CnrsUniProv.OCodeHtm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace CnrsUniProv.OCodeHtm.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for ExtensionMethodsTest and is intended
    ///to contain all ExtensionMethodsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExtensionMethodsTests
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        
        [TestMethod()]
        public void NextSignTest()
        {
            const int expected = 1;

            Random random = new Random(1);
            for (int i = 0; i < 10; i++)
            {
                var actual = random.NextSign();
                Assert.IsTrue(actual == -expected || actual == expected);
            }
        }

        
        [TestMethod()]
        public void NormalizeMatrixTest()
        {
            //
            Matrix matrix = new DenseMatrix(3, 3, -15);
            Random random = new Random(2);
            foreach (var elem in matrix.IndexedEnumerator())
            {
                matrix.At(elem.Item1, elem.Item2, elem.Item3 + 5 * random.NextDouble());
            }
            double min = 1.0, max = 2.5;

            //
            var normalizedMatrix = matrix.Normalize(max, min);

            //
            foreach (var row in normalizedMatrix.RowEnumerator())
            {
                Assert.IsTrue(row.Item2.Minimum() >= min);
                Assert.IsTrue(row.Item2.Maximum() <= max);
            }
        }

        [TestMethod()]
        public void NormalizeZeroMatrixTest()
        {
            //
            Matrix matrix = new DenseMatrix(3, 3);
            double min = 1.0, max = 2.5;

            //
            var normalizedMatrix = matrix.Normalize(max, min);

            //
            foreach (var row in normalizedMatrix.RowEnumerator())
            {
                Assert.IsTrue(row.Item2.Minimum() >= min);
                Assert.IsTrue(row.Item2.Maximum() <= max);
            }
        }

       

    }
}
