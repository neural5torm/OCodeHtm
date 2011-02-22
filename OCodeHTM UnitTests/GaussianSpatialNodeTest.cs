using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CnrsUniProv.OCodeHtm;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Exceptions;


namespace OCodeHTM_UnitTests
{
    [TestClass]
    public class GaussianSpatialNodeTest 
    {
        
        [TestMethod]
        public void ManyRandomInputsYieldAsManyCoincidencesWithMaxDistance0()
        {
            // Arrange
            double maxDistance = 0.0;
            GaussianSpatialNode node = new GaussianSpatialNode(maxDistance);
            int nbInputs = 1000;
            int width = 4;
            int height = 4;
            var random = new Random(10);
            
            // Act
            for (int i = 0; i < nbInputs; ++i)
                node.Learn(new SparseMatrix(width, height, random.NextDouble()));


            // Assert
            Assert.AreEqual(nbInputs, node.CoincidencesFrequencies.Count);
        }

        [TestMethod]
        public void TwoIdenticalInputsYieldOneCoincidenceOfFrequency2WithMaxDistance0()
        {
            // Arrange
            double maxDistance = 0.0;
            GaussianSpatialNode node = new GaussianSpatialNode(maxDistance);
            int nbInputs = 2;
            int width = 4;
            int height = 4;

            // Act
            for (int i = 0; i < nbInputs; ++i)
                node.Learn(new SparseMatrix(width, height, 1.0));


            // Assert
            Assert.AreEqual(1, node.CoincidencesFrequencies.Count);
            Assert.AreEqual(nbInputs, node.CoincidencesFrequencies[new SparseMatrix(width, height, 1.0)]);
        }

        [TestMethod]
        public void RandomInputsYieldOnlyOneCoincidenceWithLargeMaxDistance()
        {
            // Arrange
            double maxDistance = 10000.0;
            GaussianSpatialNode node = new GaussianSpatialNode(maxDistance);
            int nbInputs = 100;
            int width = 4;
            int height = 4;
            var random = new Random(10);

            // Act
            for (int i = 0; i < nbInputs; ++i)
                node.Learn(new SparseMatrix(width, height, random.NextDouble()));


            // Assert
            Assert.AreEqual(1, node.CoincidencesFrequencies.Count);
            Assert.AreNotEqual(nbInputs, node.CoincidencesFrequencies.Count);
        }

        [TestMethod]
        public void Several1DistantInputsYieldOneCoincidenceWithMaxDistance1()
        {
            // Arrange
            double maxDistance = 1.0;
            GaussianSpatialNode node = new GaussianSpatialNode(maxDistance);

            var matrices = new List<SparseMatrix>();
            matrices.Add(new SparseMatrix(new double[,] { { 3.0, 2.0, 1.0, 0.0 } }));
            matrices.Add(new SparseMatrix(new double[,] { { 3.0, 2.0, 0.0, 0.0 } }));
            matrices.Add(new SparseMatrix(new double[,] { { 3.0, 2.0, 2.0, 0.0 } }));
            matrices.Add(new SparseMatrix(new double[,] { { 3.0, 1.0, 1.0, 0.0 } }));
            matrices.Add(new SparseMatrix(new double[,] { { 2.0, 2.0, 1.0, 0.0 } }));
            int nbInputs = matrices.Count ;

            // Act
            //node.Learn();
            foreach (var matrix in matrices)
                node.Learn(matrix);

            // Assert
            Assert.AreEqual(1, node.CoincidencesFrequencies.Count);
            Assert.AreNotEqual(nbInputs, node.CoincidencesFrequencies.Count);
        }

        [TestMethod]
        public void Several1DistantInputsYieldMultipleCoincidencesWithMaxDistance0_999()
        {
            // Arrange
            double maxDistance = 0.999;
            GaussianSpatialNode node = new GaussianSpatialNode(maxDistance);

            var matrices = new List<SparseMatrix>();
            matrices.Add(new SparseMatrix(new double[,] { { 3.0, 2.0, 1.0, 0.0 } }));
            matrices.Add(new SparseMatrix(new double[,] { { 3.0, 2.0, 0.0, 0.0 } }));
            matrices.Add(new SparseMatrix(new double[,] { { 3.0, 2.0, 2.0, 0.0 } }));
            matrices.Add(new SparseMatrix(new double[,] { { 3.0, 1.0, 1.0, 0.0 } }));
            matrices.Add(new SparseMatrix(new double[,] { { 2.0, 2.0, 1.0, 0.0 } }));
            int nbInputs = matrices.Count;

            // Act
            foreach (var matrix in matrices)
                node.Learn(matrix);

            // Assert
            Assert.AreEqual(nbInputs, node.CoincidencesFrequencies.Count);
            Assert.AreNotEqual(1, node.CoincidencesFrequencies.Count);
        }

        [TestMethod]
        public void SparseMatrixAddsCorrectlyToZeroComponents()
        {
            var m1 = new SparseMatrix(1, 3);
            var m2 = new SparseMatrix(new double[,] { { 0, 1, 1 } });

            var sum1 = m1 + m2;
            var sum2 = m2 + m1;

            Assert.AreEqual(m2, sum2);
            Assert.AreEqual(m2, sum1);

        }

        [TestMethod]
        public void SparseMatrixSubtractsCorrectlyFromZeroComponents()
        {

            var m1 = new SparseMatrix(new double[,] { { 1, 0, 0, 1 }, { 1, 1, 0, 1 } });
            var m2 = new SparseMatrix(new double[,] { { 1, 1, 1, 0 }, { 1, 0, 0, 1 } });

            var diff1 = new SparseMatrix(new double[,] { { 0, -1, -1, 1 }, { 0, 1, 0, 0 } });
            var diff2 = new SparseMatrix(new double[,] { { 0, 1, 1, -1 }, { 0, -1, 0, 0 } });

            Assert.AreEqual(diff2, m2 - m1);
            Assert.AreEqual(diff1, m1 - m2);

        }

        [TestMethod]
        public void ThrowExceptionIfLearningAfterInferenceMode()
        {
            var node = new GaussianSpatialNode();
            var mat = new SparseMatrix(4);
            var learnAfterInferFails = false;
            var learnAfterTBInferFails = false;

            node.Infer(mat);
            try
            {
                node.Learn(mat);
            }
            catch (HtmRuleException e)
            {
                learnAfterInferFails = true;
                Debug.WriteLine(e.Message);
            }
            node.TimeBasedInfer(mat);
            try
            {
                node.Learn(mat);
            }
            catch (HtmRuleException e)
            {
                learnAfterTBInferFails = true;
                Debug.WriteLine(e.Message);
            }

            Assert.IsTrue(learnAfterInferFails);
            Assert.IsTrue(learnAfterTBInferFails);
        }

        [TestMethod]
        public void DontLearnBlankInputs()
        {
            var node = new GaussianSpatialNode();
            
            node.Learn(new SparseMatrix(4));

            Assert.AreEqual(0, node.CoincidencesFrequencies.Count);
        }

        [TestMethod]
        public void ThrowHtmRuleExceptionWhenLearningSizeDifferingInputs()
        {
            var node = new GaussianSpatialNode(1.0);
            node.Learn(new SparseMatrix(5, 5, 3.0));

            try
            {
                node.Learn(new SparseMatrix(4, 4, 2.0));
                Assert.Inconclusive("Should have fired an exception instead");
            }
            catch (Exception e)
            {
                
                Debug.WriteLine(e.Message);
                Assert.AreEqual(typeof(HtmRuleException), e.GetType());
            }

            
        }

        [TestMethod]
        public void OtherTests()
        {
            var m = new SparseVector(2, 1);
            var m2 = new SparseVector(2, 2);
            var row = m.ToArray().Concat(m2.ToArray()).ToArray();

            var m4 = new SparseMatrix(2, row.Length);
            m4.SetRow(0, row);
            m4.SetRow(1, row);
            
            //Assert.AreEqual(new double[,] {{ 1, 1, 2, 2 }, { 1, 1, 2, 2 }}, m4.ToArray());
        }
    }
}
