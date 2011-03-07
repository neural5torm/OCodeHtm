using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MathNet.Numerics.LinearAlgebra.Double;

namespace OCodeHTM_UnitTests
{
    [TestClass]
    public class SomeTests
    {
       
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

    }
}
