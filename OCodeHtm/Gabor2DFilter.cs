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

        Lazy<Matrix[]> filterMatricesReal;
        public Matrix[] FilterMatricesReal 
        { 
            get { return filterMatricesReal.Value; } 
        }

        Lazy<Matrix[]> filterMatricesImaginary;
        public Matrix[] FilterMatricesImaginary 
        { 
            get { return filterMatricesImaginary.Value; } 
        }

        public event OutputEventHandler<Matrix> OnFilterCreated;



        public Gabor2DFilter(uint orientations = Default.GaborOrientations, double aspectRatio = Default.SquareAspectRatio, MultipleOutputFilterMode outputMode = MultipleOutputFilterMode.Concatenated)
        {
            NbOrientations = (int)orientations;
            OutputMode = outputMode;
            AspectRatio = aspectRatio;
            //TODO properties
            uint waveLength = 8;
            double bandwidth = 1.0;

            // Create (lazy) filter matrices
            filterMatricesReal = new Lazy<Matrix[]>(() =>
            {
                var filters = new Matrix[NbOrientations];
                for (int i = 0; i < NbOrientations; i++)
                    filters[i] = CreateFilter(i * Math.PI / NbOrientations, /*phase:*/0, waveLength, AspectRatio, bandwidth);
                
                return filters;
            });

            filterMatricesImaginary = new Lazy<Matrix[]>(() =>
            {
                var filters = new Matrix[NbOrientations];
                for (int i = 0; i < NbOrientations; i++)
                    filters[i] = CreateFilter(i * Math.PI / NbOrientations, /*phase:*/Math.PI / 2, waveLength, AspectRatio, bandwidth);
                
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
        private Matrix CreateFilter(double angle, double phase, uint waveLength, double aspectRatio, double bandwidth)
        {

            var sigmaX = waveLength / Math.PI * Math.Sqrt(Math.Log(2) / 2) * (Math.Pow(2, bandwidth) + 1) / (Math.Pow(2.0, bandwidth) - 1);
            var sigmaY = sigmaX / aspectRatio;

            // Bounding box
            int n = 4;
            int halfSize = (int)Math.Ceiling(n * Math.Max(sigmaX, sigmaY));
            var size = /*symmetrical values:*/2 * halfSize + /*the middle zero:*/1;

            // Filter matrix
            var filterMatrix = new DenseMatrix(size);

            for (int x = -halfSize, xPos = 0; x <= halfSize; x++, xPos++)
            {
                for (int y = -halfSize, yPos = 0; y <= halfSize; y++, yPos++)
                {
                    double xTheta = x * Math.Cos(angle) + y * Math.Sin(angle);
                    double yTheta = -x * Math.Sin(angle) + y * Math.Cos(angle);

                    filterMatrix.At(yPos, xPos, GaborFunction(xTheta, yTheta, sigmaX, sigmaY, waveLength, phase));
                }
            }

            // Notify observers about the Gabor filter created
            if (OnFilterCreated != null)
                OnFilterCreated(this, new OutputEventArgs<Matrix>(filterMatrix, string.Format("{0:G3}rad.angle_{1:G3}rad.phase_filter", angle, phase)));

            return filterMatrix;
        }

        private double GaborFunction(double xTheta, double yTheta, double sigmaX, double sigmaY, double lambda, double psi)
        {
            return Math.Exp(-0.5 * (Math.Pow(xTheta, 2) / Math.Pow(sigmaX, 2) + Math.Pow(yTheta, 2) / Math.Pow(sigmaY, 2)))
                    * Math.Cos(2 * Math.PI * xTheta / lambda + psi);
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
