using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm;
using CnrsUniProv.OCodeHtm.Exceptions;

namespace OCodeHTM_UnitTests
{
    [TestClass]
    public class HtmLayer2DTest
    {
        
        [TestMethod]
        public void ErrorWhenLearningAfterLayerIsTrained()
        {
            var layer = new HtmLayer2D(1, 1, 0.0, false, 1000);

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
            var layer = new HtmLayer2D(size, size, overlap, true, 1000);

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
            var layer = new HtmLayer2D(size, size, overlap, true, 1000);

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
            var layer = new HtmLayer2D(size, size, overlap, true, 1000);

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
            var layer = new HtmLayer2D(size, size, overlap, true, 1000);

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
    }
}
