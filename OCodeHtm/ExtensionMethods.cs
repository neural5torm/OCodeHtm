using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

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


        // Double.Matrix
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

        public static Matrix<double> Normalize(this Matrix<double> matrix, double max = 1.0, double min = 0.0)
        {
            var rows = matrix.RowCount;
            var columns = matrix.ColumnCount;

            // Normalize floor
            var floored = matrix - new DenseMatrix(rows, columns, matrix.Minimum());

            // Normalize range
            var maxValue = floored.Maximum();
            var normalized0To1 = maxValue != 0 ? floored.Divide(maxValue) : floored;
            
            // Expand back matrix range and shifting floor according to given min & max params
            return normalized0To1 * (max - min) + new DenseMatrix(rows, columns, min);
        }


        public static Matrix<double> PointwiseApply(this Matrix<double> matrix, Func<double, double> function)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    matrix.At(i, j, function(matrix.At(i, j)));
                }
            }

            return matrix;
        }
    }
}