using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm;
using CnrsUniProv.OCodeHtm.Exceptions;

namespace CnrsUniProv.OCodeHtm.UnitTests
{
    [TestClass]
    public class SpatialLayerTest 
    {
        #region Gaussian Spatial Layer

        [TestMethod]
        public void ErrorWhenLearningAfterLayerIsTrained()
        {
            var layer = new SpatialLayer(SpatialLayerType.Gaussian, 1, 1, 0.0, true, 1000);

            layer.Learn(new SparseMatrix(5));
            layer.Infer(new SparseMatrix(5));
            try
            {
                layer.Learn(new SparseMatrix(5));
                Assert.Inconclusive("Should have fired an exception");
            }
            catch (Exception e)
            {

                Debug.WriteLine(e.Message);
                Assert.IsInstanceOfType(e, typeof(HtmRuleException));
            }
        }

        [TestMethod]
        public void SeesCorrectSubmatrixWhenCloneTrainingWith0Overlap()
        {
            var inputsize = 10;
            var size = (uint)inputsize / 2;
            var overlap = 0.0;
            var layer = new SpatialLayer(SpatialLayerType.Gaussian, size, size, overlap, true, 1000);

            var list = from i in Enumerable.Range(0, inputsize)
                       select (double)i;

            var matrix = new SparseMatrix(inputsize, inputsize);
            for (int i = 0; i < matrix.RowCount; ++i)
            {
                matrix.SetRow(i, list.ToArray());
            }

            // Act
            var subMatrix = layer.GetSubMatrixForNodeAt(layer.ClonedNodeRow, layer.ClonedNodeCol, matrix);
            
            //
            var width = inputsize / size + overlap * (inputsize - inputsize / size);
            var delta = (inputsize - width) / (size - 1);
            
            var centerRow = layer.Height / 2;
            var centerCol = layer.Width / 2;

            for (int i = 0; i < (int)width; i++)
            {
                for (int j = 0; j < (int)width; j++)
                {
                    Assert.AreEqual(centerCol * delta + j, subMatrix[i, j]);
                }
            }
        }

        [TestMethod]
        public void SeesCorrectSubmatrixWhenCloneTrainingWith0_5Overlap()
        {
            var inputsize = 10;
            var size = (uint)inputsize / 2;
            var overlap = 0.5;
            var layer = new SpatialLayer(SpatialLayerType.Gaussian, size, size, overlap, true, 1000);

            var list = from i in Enumerable.Range(0, inputsize)
                       select (double)i;

            var matrix = new SparseMatrix(inputsize, inputsize);
            for (int i = 0; i < matrix.RowCount; ++i)
            {
                matrix.SetRow(i, list.ToArray());
            }

            // Act
            var subMatrix = layer.GetSubMatrixForNodeAt(layer.ClonedNodeRow, layer.ClonedNodeCol, matrix);

            //
            var width = inputsize / size + overlap * (inputsize - inputsize / size);
            var delta = (inputsize - width) / (size - 1);

            var centerRow = layer.Height / 2;
            var centerCol = layer.Width / 2;

            for (int i = 0; i < (int)width; i++)
            {
                for (int j = 0; j < (int)width; j++)
                {
                    Assert.AreEqual(centerCol * delta + j, subMatrix[i, j]);
                }
            }
        }

        [TestMethod]
        public void SeesCorrectSubmatrixWhenTrainingWith0Overlap()
        {
            var inputsize = 10;
            var size = (uint)inputsize / 2;
            var overlap = 0;
            var layer = new SpatialLayer(SpatialLayerType.Gaussian, size, size, overlap, true, 1000);

            var list = from i in Enumerable.Range(0, inputsize)
                       select (double)i;

            var matrix = new SparseMatrix(inputsize, inputsize);
            for (int i = 0; i < matrix.RowCount; ++i)
            {
                matrix.SetRow(i, list.ToArray());
            }


            // for each node in the 2D layer
            for (int nodeRow = 0; nodeRow < layer.Height; nodeRow++)
            {
                for (int nodeCol = 0; nodeCol < layer.Width; nodeCol++)
                {
                    // Act
                    var subMatrix = layer.GetSubMatrixForNodeAt(nodeRow, nodeCol, matrix);

                    // check if its 2D input is ok
                    var width = inputsize / size + overlap * (inputsize - inputsize / size);
                    var delta = (inputsize - width) / (size - 1);

                    for (int i = 0; i < (int)width; i++)
                    {
                        for (int j = 0; j < (int)width; j++)
                        {
                           
                            Assert.AreEqual(nodeCol * delta + j, subMatrix[i, j]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void SeesCorrectSubmatrixWhenTrainingWith0_5Overlap()
        {
            var inputsize = 10;
            var size = (uint)inputsize / 2;
            var overlap = 0.5;
            var layer = new SpatialLayer(SpatialLayerType.Gaussian, size, size, overlap, true, 1000);

            var list = from i in Enumerable.Range(0, inputsize)
                       select (double)i;

            var matrix = new SparseMatrix(inputsize, inputsize);
            for (int i = 0; i < matrix.RowCount; ++i)
            {
                matrix.SetRow(i, list.ToArray());
            }


            // for each node in the 2D layer
            for (int nodeRow = 0; nodeRow < layer.Height; nodeRow++)
            {
                for (int nodeCol = 0; nodeCol < layer.Width; nodeCol++)
                {
                    // Act
                    var subMatrix = layer.GetSubMatrixForNodeAt(nodeRow, nodeCol, matrix);

                    // check if its 2D input is ok
                    var width = inputsize / size + overlap * (inputsize - inputsize / size);
                    var delta = (inputsize - width) / (size - 1);

                    for (int i = 0; i < (int)width; i++)
                    {
                        for (int j = 0; j < (int)width; j++)
                        {

                            Assert.AreEqual(nodeCol * delta + j, subMatrix[i, j]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void CanLearnNewInputs_5By4Layer0Overlap()
        {
            uint maxoutputsize = 5;
            var overlap = 0.0;
            var clone = false;
            uint height = 5;
            uint width = 4;

            var layer = new SpatialLayer(SpatialLayerType.Gaussian, height, width, overlap, clone, maxoutputsize);
            var input1 = (SparseMatrix) SparseMatrix.Identity(8).Stack(new SparseMatrix(2, 8));
            var input2 = 2*input1;
            
            //
            layer.Learn(input1);
            layer.Learn(input2);

            //
            for (int i = 0; i < height; i++)
			{
			    for (int j = 0; j < width; j++)
			    {
			        var node = (SpatialNodeGaussian)layer.NodeArray[i, j];
                    foreach (var coinc in node.CoincidencesFrequencies)
                        Assert.AreEqual(1, coinc.Value);
                }
			}            
        }

        [TestMethod]
        public void CanLearnAndInfer1OnRecognizedSubInputs_5By4Layer0Overlap()
        {
            uint maxoutputsize = 5;
            var overlap = 0.0;
            var clone = false;
            uint layerheight = 5;
            uint layerwidth = 4;
            int inputtolayerratio = 2;
            
            var layer = new SpatialLayer(SpatialLayerType.Gaussian, layerheight, layerwidth, overlap, clone, maxoutputsize);
            var input1 = new SparseMatrix((int)layerheight * inputtolayerratio, (int)layerwidth * inputtolayerratio, 1.0);
            var input2 = 2 * input1;
            
            
            //
            layer.Learn(input1);
            layer.Learn(input2);

            var infer2 = layer.Infer(input2);
            var infer1 = layer.Infer(input1);
            var input3 = (SparseMatrix)new SparseMatrix((int)layerwidth * inputtolayerratio, (int)layerwidth * inputtolayerratio, 1.0)
                .Stack(new SparseMatrix((int)(layerheight - layerwidth) * inputtolayerratio, (int)layerwidth * inputtolayerratio, 3.0));
            var infer3 = layer.Infer(input3);

            //
            foreach (var infer in new SparseMatrix[] { infer1, infer2, infer3 })
            {
                for (int row = 0; row < layerheight; row++)
                {
                    for (int col = 0; col < layerwidth * layer.MaxNodeOutputSize; col += layer.MaxNodeOutputSize)
                    {
                        var subInput = infer.SubMatrix(row, 1, col, layer.MaxNodeOutputSize);
                        if (infer.Equals(infer3) && row >= layerheight - 1 && row < layerheight)
                            for (int i = 0; i < subInput.ColumnCount; i++)
                                Assert.IsTrue(subInput[0, i] < 0.1);
                        else                       
                            Assert.IsTrue(subInput[0, 0] == 1.0 || subInput[0, 1] == 1.0);                                 
                    }
                }
            }
        }

        [TestMethod]
        public void CanLearnAndInfer1OnRecognizedSubInputs_5By4Layer0_5Overlap()
        {
            uint maxoutputsize = 5;
            var overlap = 0.5;
            var clone = false;
            uint layerheight = 5;
            uint layerwidth = 4;
            int inputtolayerratio = 2;

            var layer = new SpatialLayer(SpatialLayerType.Gaussian, layerheight, layerwidth, overlap, clone, maxoutputsize);
            var input1 = new SparseMatrix((int)layerheight * inputtolayerratio, (int)layerwidth * inputtolayerratio, 1.0);
            var input2 = 2 * input1;


            //
            layer.Learn(input1);
            layer.Learn(input2);

            var infer2 = layer.Infer(input2);
            var infer1 = layer.Infer(input1);
            var input3 = (SparseMatrix)new SparseMatrix((int)layerwidth * inputtolayerratio, (int)layerwidth * inputtolayerratio, 1.0)
                .Stack(new SparseMatrix((int)(layerheight - layerwidth) * inputtolayerratio, (int)layerwidth * inputtolayerratio, 3.0));
            var infer3 = layer.Infer(input3);

            //
            foreach (var infer in new SparseMatrix[] { infer1, infer2, infer3 })
            {
                for (int row = 0; row < layerheight; row++)
                {
                    for (int col = 0; col < layerwidth * layer.MaxNodeOutputSize; col += layer.MaxNodeOutputSize)
                    {
                        var subInput = infer.SubMatrix(row, 1, col, layer.MaxNodeOutputSize);
                        if (infer.Equals(infer3) && row >= layerheight - 2 && row < layerheight)
                            for (int i = 0; i < subInput.ColumnCount; i++)
                                Assert.IsTrue(subInput[0, i] < 0.1);
                        else
                            Assert.IsTrue(subInput[0, 0] == 1.0 || subInput[0, 1] == 1.0);
                    }
                }
            }
        }


        [TestMethod]
        public void CanLearnAndInfer1OnRecognizedSubInput_5By4Layer0OverlapWithCloning()
        {
            uint maxoutputsize = 5;
            var overlap = 0.0;
            var clone = false;
            uint layerheight = 5;
            uint layerwidth = 4;
            int inputtolayerratio = 2;

            var layer = new SpatialLayer(SpatialLayerType.Gaussian, layerheight, layerwidth, overlap, clone, maxoutputsize);
            var input1 = new SparseMatrix((int)layerheight * inputtolayerratio, (int)layerwidth * inputtolayerratio, 1.0);
            var input2 = 2 * input1;


            //
            layer.Learn(input1);
            layer.Learn(input2);

            var infer2 = layer.Infer(input2);
            var infer1 = layer.Infer(input1);
            var input3 = (SparseMatrix)new SparseMatrix((int)layerwidth * inputtolayerratio, (int)layerwidth * inputtolayerratio, 1.0)
                .Stack(new SparseMatrix((int)(layerheight - layerwidth) * inputtolayerratio, (int)layerwidth * inputtolayerratio, 3.0));
            var infer3 = layer.Infer(input3);

            //
            foreach (var infer in new SparseMatrix[] { infer1, infer2, infer3 })
            {
                for (int row = 0; row < layerheight; row++)
                {
                    for (int col = 0; col < layerwidth * layer.MaxNodeOutputSize; col += layer.MaxNodeOutputSize)
                    {
                        var subInput = infer.SubMatrix(row, 1, col, layer.MaxNodeOutputSize);
                        if (infer.Equals(infer3) && row >= layerheight - 1 && row < layerheight)
                            for (int i = 0; i < subInput.ColumnCount; i++)
                                Assert.IsTrue(subInput[0, i] < 0.1);
                        else
                            Assert.IsTrue(subInput[0, 0] == 1.0 || subInput[0, 1] == 1.0);
                    }
                }
            }
        }

        [TestMethod]
        public void CanLearnAndInfer1OnRecognizedSubInput_5By4Layer0_5OverlapWithCloning()
        {
            uint maxoutputsize = 5;
            var overlap = 0.5;
            var clone = false;
            uint layerheight = 5;
            uint layerwidth = 4;
            int inputtolayerratio = 2;

            var layer = new SpatialLayer(SpatialLayerType.Gaussian, layerheight, layerwidth, overlap, clone, maxoutputsize);
            var input1 = new SparseMatrix((int)layerheight * inputtolayerratio, (int)layerwidth * inputtolayerratio, 1.0);
            var input2 = 2 * input1;


            //
            layer.Learn(input1);
            layer.Learn(input2);

            var infer2 = layer.Infer(input2);
            var infer1 = layer.Infer(input1);
            var input3 = (SparseMatrix)new SparseMatrix((int)layerwidth * inputtolayerratio, (int)layerwidth * inputtolayerratio, 1.0)
                .Stack(new SparseMatrix((int)(layerheight - layerwidth) * inputtolayerratio, (int)layerwidth * inputtolayerratio, 3.0));
            var infer3 = layer.Infer(input3);

            //
            foreach (var infer in new SparseMatrix[] { infer1, infer2, infer3 })
            {
                for (int row = 0; row < layerheight; row++)
                {
                    for (int col = 0; col < layerwidth * layer.MaxNodeOutputSize; col += layer.MaxNodeOutputSize)
                    {
                        var subInput = infer.SubMatrix(row, 1, col, layer.MaxNodeOutputSize);
                        if (infer.Equals(infer3) && row >= layerheight - 2 && row < layerheight)
                            for (int i = 0; i < subInput.ColumnCount; i++)
                                Assert.IsTrue(subInput[0, i] < 0.1);
                        else
                            Assert.IsTrue(subInput[0, 0] == 1.0 || subInput[0, 1] == 1.0);
                    }
                }
            }
        }

        #endregion
    }
}
