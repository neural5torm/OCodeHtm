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
        // Random
        public static bool NextBool(this Random random)
        {
            return random.Next(2) > 0 ? true : false;
        }

        public static int NextSign(this Random random)
        {
            return random.NextBool() ? -1 : 1;
        }


        // Matrix<double>
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
            foreach (var el in matrix.RowEnumerator())
            {
                sum += el.Item2.Sum();
            }

            return sum;
        }

        //TODOlater unit test
        public static Matrix SubMatrixAround(this Matrix<double> matrix, int centerRow, int rowLength, int centerCol, int colLength)
        {
            var subMatrix = matrix.CreateMatrix(rowLength, colLength);

            var topRow = Math.Max(centerRow - rowLength / 2, 0);
            var topRowPos = topRow - (centerRow - colLength / 2);
            
            var leftCol = Math.Max(centerCol - rowLength / 2, 0);
            var leftColPos = leftCol - (centerCol - colLength / 2);

            var actualRowLength = Math.Min(rowLength - topRowPos, matrix.RowCount - topRow);
            var actualColLength = Math.Min(colLength - leftColPos, matrix.ColumnCount - leftCol);

            subMatrix.SetSubMatrix(topRowPos, actualRowLength, leftColPos, actualColLength, matrix.SubMatrix(topRow, actualRowLength, leftCol, actualColLength));
            
            return (Matrix)subMatrix;
        }

        //TODOlater unit test
        public static Matrix Convolve(this Matrix<double> matrix, Matrix<double> kernel)
        {
            var result = matrix.CreateMatrix(matrix.RowCount, matrix.ColumnCount);

            for (int row = 0; row < matrix.RowCount; row++)
            {
                for (int col = 0; col < matrix.ColumnCount; col++)
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


        public static SparseMatrix ShiftAndFilterToZero(this Matrix<double> matrix, double mean, double filterOutAbsValuesSmallerThan)
        {
            var filtered = new SparseMatrix(matrix.RowCount, matrix.ColumnCount);

            var shifted = matrix.Subtract(new DenseMatrix(matrix.RowCount, matrix.ColumnCount, mean));
            foreach (var el in shifted.IndexedEnumerator())
            {
                if (Math.Abs(el.Item3) > filterOutAbsValuesSmallerThan)
                    filtered[el.Item1, el.Item2] = el.Item3;
            }

            return filtered;
        }


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
    }
}