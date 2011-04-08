using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CnrsUniProv.OCodeHtm;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Exceptions;


namespace CnrsUniProv.OCodeHtm.UnitTests
{
    [TestClass]
    public class SpatialNodeGaussianTest 
    {
        
        [TestMethod]
        public void ManyRandomInputsYieldAsManyCoincidencesWithMaxDistance0()
        {
            // Arrange
            double maxDistance = 0.0;
            SpatialNodeGaussian node = new SpatialNodeGaussian(maxDistance);
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
            SpatialNodeGaussian node = new SpatialNodeGaussian(maxDistance, 0, 1);
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
            SpatialNodeGaussian node = new SpatialNodeGaussian(maxDistance);
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
            SpatialNodeGaussian node = new SpatialNodeGaussian(maxDistance);

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
            SpatialNodeGaussian node = new SpatialNodeGaussian(maxDistance);

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
        public void Several5By5InputsYieldOneCoincidenceWithMaxDistance5()
        {
            var node = new SpatialNodeGaussian(5);
            var ones = new SparseMatrix(5, 5, 1.0);
            var twos = new SparseMatrix(5, 5, 2.0);
           
            node.Learn(ones);
            node.Learn(twos);

            Assert.AreEqual(1, node.CoincidencesFrequencies.Count);
        }

        [TestMethod]
        public void Several5By5InputsYieldSeveralCoincidenceWithMaxDistance4_999()
        {
            var node = new SpatialNodeGaussian(4.999);
            var ones = new SparseMatrix(5, 5, 1.0);
            var twos = new SparseMatrix(5, 5, 2.0);

            node.Learn(ones);
            node.Learn(twos);

            Assert.AreNotEqual(1, node.CoincidencesFrequencies.Count);
            Assert.AreEqual(2, node.CoincidencesFrequencies.Count);
        }
      
        [TestMethod]
        public void ErrorWhenLearningAfterInferenceMode()
        {
            var node = new SpatialNodeGaussian();
            var mat = new SparseMatrix(4, 4, 4.0);
            var learnAfterInferFails = false;
            var learnAfterTBInferFails = false;
            node.Learn(mat);


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

            node.TimeInfer(mat);
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
            var node = new SpatialNodeGaussian();
            
            node.Learn(new SparseMatrix(4));

            Assert.AreEqual(0, node.CoincidencesFrequencies.Count);
        }

        [TestMethod]
        public void ErrorWhenLearningVaryingSizeInputs()
        {
            var node = new SpatialNodeGaussian();
            node.Learn(new SparseMatrix(5, 5, 3.0));

            try
            {
                node.Learn(new SparseMatrix(4, 4, 2.0));
                Assert.Inconclusive("Should have fired an exception");
            }
            catch (Exception e)
            {
                
                Debug.WriteLine(e.Message);
                Assert.IsInstanceOfType(e, typeof(HtmRuleException));
            }
        }

        [TestMethod]
        public void InferEmptyVectorIfNoCoincidencesLearned()
        {
            var node = new SpatialNodeGaussian(1.0);

            var output = node.Infer(new SparseMatrix(5, 5, 2.0));

            Assert.AreEqual(new SparseVector(1), output);

            output = node.TimeInfer(new SparseMatrix(5, 5, 2.0));

            Assert.AreEqual(new SparseVector(1), output);
        }

        [TestMethod]
        public void InferCorrectlyOnIdenticalMatricesWithMaxDistance0()
        {
            var node = new SpatialNodeGaussian();
            var ones = new SparseMatrix(4, 5, 1.0);
            var twos = new SparseMatrix(4, 5, 2.0);
            var expectedLower = Math.Exp(-(ones.RowCount * ones.ColumnCount) / (2 * node.SquaredSigma)) - 0.0001;
            var expectedUpper = Math.Exp(-(ones.RowCount * ones.ColumnCount) / (2 * node.SquaredSigma)) + 0.0001;
            node.Learn(ones);
            node.Learn(twos);

            var output = node.Infer(ones);

            Assert.IsTrue(output[0] == 1.0 && output[1] > expectedLower && output[1] < expectedUpper);
        }

        [TestMethod]
        public void StopLearningWhenMaxOutputSizeReached()
        {
            int max = 1;
            var node = new SpatialNodeGaussian(0.0, 0.0, max);
            var ones = SparseMatrix.Identity(5);
            var twos = SparseMatrix.Identity(5) * 2.0;
            
            node.Learn(ones);
            node.Learn(twos);

            Assert.AreEqual(max, node.CoincidencesFrequencies.Count);
        }
    }
}
