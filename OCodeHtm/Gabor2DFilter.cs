using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Interfaces;

namespace CnrsUniProv.OCodeHtm
{
    public class Gabor2DFilter : IFilter<SparseMatrix>
    {
        public int NbOrientations { get; private set; }
        public MultipleOutputFilterMode OutputMode { get; private set; }
        public double AspectRatio { get; private set; }

        Lazy<SparseMatrix[]> filterMatricesReal;
        public SparseMatrix[] FilterMatricesReal 
        { 
            get { return filterMatricesReal.Value; } 
        }

        Lazy<SparseMatrix[]> filterMatricesImaginary;
        public SparseMatrix[] FilterMatricesImaginary 
        { 
            get { return filterMatricesImaginary.Value; } 
        }

        public event OutputEventHandler<Matrix> OnFilterCreated;



        public Gabor2DFilter(uint orientations = Default.GaborOrientations, double aspectRatio = Default.SquareAspectRatio, MultipleOutputFilterMode outputMode = MultipleOutputFilterMode.Concatenated)
        {
            NbOrientations = (int)orientations;
            OutputMode = outputMode;
            AspectRatio = aspectRatio;

            // Create (lazy) filter matrices
            var sigma = 5.0;

            filterMatricesReal = new Lazy<SparseMatrix[]>(() =>
            {
                var filters = new SparseMatrix[NbOrientations];
                for (int i = 0; i < NbOrientations; i++)
                    filters[i] = CreateFilter(i * Math.PI / NbOrientations, /*phase:*/ 0, 8, aspectRatio, sigma);
                
                return filters;
            });

            filterMatricesImaginary = new Lazy<SparseMatrix[]>(() =>
            {
                var filters = new SparseMatrix[NbOrientations];
                for (int i = 0; i < NbOrientations; i++)
                    filters[i] = CreateFilter(i * Math.PI / NbOrientations, /*phase:*/ Math.PI / 2, 8, aspectRatio, sigma);
                
                return filters;
            });
        }

        /// <summary>
        /// Create a 2D Gabor filter like in the example at http://en.wikipedia.org/wiki/Gabor_filter
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="phase"></param>
        /// <param name="waveLength"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="sigma"></param>
        /// <returns></returns>
        private SparseMatrix CreateFilter(double angle, double phase, uint waveLength, double aspectRatio, double sigma)
        {
            var sigmaX = sigma;
            var sigmaY = sigma / aspectRatio;

            // Bounding box
            var n = 5;
            var xMax = (int)Math.Ceiling(Math.Max(Math.Abs(n * sigmaX * Math.Cos(angle)), Math.Abs(n * sigmaY * Math.Sin(angle))));
            var yMax = (int)Math.Ceiling(Math.Max(Math.Abs(n * sigmaX * Math.Sin(angle)), Math.Abs(n * sigmaY * Math.Cos(angle))));

            xMax = Math.Max(1, xMax);
            var xMin = -xMax;
            yMax = Math.Max(1, yMax);
            var yMin = -yMax;

            var filterMatrix = new SparseMatrix(yMax - yMin + 1, xMax - xMin + 1);
            for (int xPos = 0; xPos < filterMatrix.ColumnCount; xPos++)
            {
                for (int yPos = 0; yPos < filterMatrix.RowCount; yPos++)
                {
                    double x = xMin + xPos;
                    double y = yMin + yPos;

                    filterMatrix.At(yPos, xPos, GaborFunction(x * Math.Cos(angle) + y * Math.Sin(angle), -x * Math.Sin(angle) + y * Math.Cos(angle), sigmaX, sigmaY, (double)waveLength, phase));
                }
            }


            // Notify observers about the Gabor filter
            if (OnFilterCreated != null)
                OnFilterCreated(this, new OutputEventArgs<Matrix>(filterMatrix, string.Format("{0:G3}rad.angle_{1:G3}rad.phase_filter", angle, phase)));

            return filterMatrix;
        }

        private double GaborFunction(double xTheta, double yTheta, double sigmaX, double sigmaY, double lambda, double psi)
        {
            return 1 / (2 * Math.PI * sigmaX * sigmaY) 
                * Math.Exp(-0.5 * (Math.Pow(xTheta, 2) / Math.Pow(sigmaX, 2) 
                                    + Math.Pow(yTheta, 2) / Math.Pow(sigmaY, 2))) 
                * Math.Cos(2 * Math.PI / lambda * xTheta + psi);
        }
        
        public SparseMatrix Filter(SparseMatrix input)
        {
            var outputs = new SparseMatrix[NbOrientations];

            //TODOnow 
            //TODO create convolve extension method
            if (OutputMode == MultipleOutputFilterMode.Concatenated)
            { 
                // Concatenate outputs
            }

            return input;
        }


    }
}
