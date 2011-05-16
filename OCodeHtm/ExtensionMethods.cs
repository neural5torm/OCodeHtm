using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.NumberTheory;

namespace CnrsUniProv.OCodeHtm
{
    public static class ExtensionMethods
    {
        #region Random
        
        public static bool NextBool(this Random random)
        {
            return random.Next(2) > 0 ? true : false;
        }

        public static int NextSign(this Random random)
        {
            return random.NextBool() ? -1 : 1;
        }

        #endregion


        #region Matrix<double>
        public static double Minimum(this Matrix<double> matrix)
        {
            var minValue = double.MaxValue;

            foreach (var item in matrix.RowEnumerator())
            {
                minValue = Math.Min(item.Item2.Minimum(), minValue);
            }

            return minValue;
        }

        public static double Maximum(this Matrix<double> matrix)
        {
            var maxValue = double.MinValue;

            foreach (var item in matrix.RowEnumerator())
            {
                maxValue = Math.Max(item.Item2.Maximum(), maxValue);
            }

            return maxValue;
        }

        public static double Mean(this Matrix<double> matrix)
        {
            return matrix.Sum() / (matrix.RowCount * matrix.ColumnCount);
        }

        public static double Sum(this Matrix<double> matrix)
        {
            var sum = 0.0;
            foreach (var row in matrix.RowEnumerator())
            {
                sum += row.Item2.Sum();
            }

            return sum;
        }

        
        public static Matrix SubMatrixAround(this Matrix<double> matrix, int centerRow, int rowCount, int centerCol, int colCount)
        {
            var subMatrix = new DenseMatrix(rowCount, colCount);

            var topRow = Math.Max(centerRow - rowCount / 2, 0);
            var topRowPos = topRow - (centerRow - colCount / 2);

            var leftCol = Math.Max(centerCol - rowCount / 2, 0);
            var leftColPos = leftCol - (centerCol - colCount / 2);

            var actualRowCount = Math.Min(rowCount - topRowPos, matrix.RowCount - topRow);
            var actualColCount = Math.Min(colCount - leftColPos, matrix.ColumnCount - leftCol);

            // TODO use VirtualSubMatrix i.of creating new sub-matrices (compare perf)
            subMatrix.SetSubMatrix(topRowPos, actualRowCount, leftColPos, actualColCount, matrix.SubMatrix(topRow, actualRowCount, leftCol, actualColCount));
            
            return subMatrix;
        }

        // TODOlater use in VirtualSubMatrix?
        public static double ValueInAndAround(this Matrix<double> matrix, int row, int col, bool replicateBorders = false)
        {
            if (row < 0 || row >= matrix.RowCount
                || col < 0 || col >= matrix.ColumnCount)
            {
                if (replicateBorders)
                    return matrix[Math.Min(Math.Max(row, 0), matrix.RowCount - 1), Math.Min(Math.Max(col, 0), matrix.ColumnCount - 1)];
                else
                    return 0.0;
            }
            else
                return matrix[row, col];
        }

        // TODOlater unit test Convolve method
        public static Matrix Convolve(this Matrix<double> matrix, Matrix<double> kernel)
        {
            var result = matrix.CreateMatrix(matrix.RowCount, matrix.ColumnCount);

            var rowCount = matrix.RowCount;
            var columnCount = matrix.ColumnCount;
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < columnCount; col++)
                {
                    result[row, col] = matrix.SubMatrixAround(row, kernel.RowCount, col, kernel.ColumnCount).PointwiseMultiply(kernel).Sum();
                }
            }

            return (Matrix)result;
        }

        

        public static Matrix Normalize(this Matrix<double> matrix, double max = 1.0, double min = 0.0)
        {
            var rows = matrix.RowCount;
            var columns = matrix.ColumnCount;

            // Normalize floor
            var floored = matrix - new DenseMatrix(rows, columns, matrix.Minimum());

            // Normalize range
            var maxValue = floored.Maximum();
            var normalized0To1 = maxValue != 0 ? floored.Divide(maxValue) : floored;
            
            // Expand back matrix range and shifting floor according to given min & max params
            return (Matrix)(normalized0To1 * (max - min) + new DenseMatrix(rows, columns, min));
        }


        public static SparseMatrix ShiftAndFilterToZero(this Matrix<double> matrix, double shiftToMean, double filterOutAbsValuesSmallerThan)
        {
            var filtered = new SparseMatrix(matrix.RowCount, matrix.ColumnCount);

            var shifted = matrix.Subtract(new DenseMatrix(matrix.RowCount, matrix.ColumnCount, shiftToMean));
            foreach (var el in shifted.IndexedEnumerator())
            {
                if (Math.Abs(el.Item3) > filterOutAbsValuesSmallerThan)
                    filtered[el.Item1, el.Item2] = el.Item3;
            }

            return filtered;
        }


        public static Matrix PointwisePower(this Matrix<double> matrix, double power)
        {
            if (power == 2.0)
                return (Matrix)(matrix.PointwiseMultiply(matrix));

            foreach (var el in matrix.IndexedEnumerator())
            {
                matrix[el.Item1, el.Item2] = Math.Pow(el.Item3, power);
            }

            return (Matrix)matrix;
        }

        /// <summary>
        /// Warning: avoid using PointwiseApply with sparse matrices since each and every element will be re-computed.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static Matrix PointwiseApply(this Matrix<double> matrix, Func<double, double> function)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    matrix.At(i, j, function(matrix.At(i, j)));
                }
            }

            return (Matrix)matrix;
        }

        public static bool IsBlank(this Matrix<double> matrix)
        {
            return matrix.FrobeniusNorm() == 0.0;
        }

        //TODO bool IsGrey()

        #endregion
    }
}