using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MathNet.Numerics.LinearAlgebra.Double;

namespace CnrsUniProv.OCodeHtm.UnitTests
{
    [TestClass]
    public class OtherComponentsTests
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


        [TestMethod]
        public void SparseMatrixSubMatrixMethodWorks()
        {
            var matrix = new SparseMatrix(10, 10, 1.0);
            var sub = matrix.SubMatrix(8, 2, 0, 2);

            Assert.AreEqual(2, sub.RowCount);
            Assert.AreEqual(2, sub.ColumnCount);
        }

        [TestMethod]
        public void SparseMatrixIndexedEnumeratorWorks()
        {
            var cols = 7;
            var rows = 7;
            var matrix = new SparseMatrix(rows, cols, new double[] { 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, /**/2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 34.5104995998366, 3.30253638788279, 2.7461515689052E-06, 2.7461515689052E-06, /**/2.7461515689052E-06, 2.7461515689052E-06, 3.30253638788279, 255, 3.30253638788279, 2.7461515689052E-06, 2.37450036851675E-06, /**/2.7461515689052E-06, 2.7461515689052E-06, 24.4026090935093, 255, 24.4026090935093, 2.7461515689052E-06, 2.7461515689052E-06, /**/2.7461515689052E-06, 2.7461515689052E-06, 3.30253638788279, 34.5104995998366, 3.30253638788279, 2.7461515689052E-06, 2.37450036851675E-06, /**/2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, /**/2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06, 6.62979636198309E-06, 2.7461515689052E-06, 2.7461515689052E-06, 2.7461515689052E-06 });
            var normalized = matrix.Normalize(1.0, 2.7461515689052E-06);

            //
            var count = 0;
            foreach (var el in matrix.IndexedEnumerator())
            {
                count++;
            }
            var count2 = 0;
            foreach (var el in normalized.IndexedEnumerator())
            {
                count2++;
            }

            //
            Assert.AreEqual(cols * rows, count);
            Assert.AreEqual(cols * rows, count2);
        }
    }
}
