using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Interfaces;
using MathNet.Numerics.LinearAlgebra.Generic;

namespace CnrsUniProv.OCodeHtm
{
    public class Gabor2DFilter : IFilter<SparseMatrix>
    {
        public MultipleOutputFilterMode OutputMode { get; private set; }

        public int NbOrientations { get; private set; }
        public double AspectRatio { get; private set; }
        public double WaveLength { get; private set; }
        public double Bandwidth { get; private set; }


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



        public Gabor2DFilter(MultipleOutputFilterMode outputMode = MultipleOutputFilterMode.Concatenated, 
            uint orientations = Default.GaborOrientations, double aspectRatio = Default.SquareAspectRatio, double waveLength = Default.GaborWaveLength, double bandwidth = Default.GaborBandwidth)
        {
            OutputMode = outputMode;
            NbOrientations = (int)orientations;
            AspectRatio = aspectRatio;
            WaveLength = waveLength;
            Bandwidth = bandwidth;

            // Create (lazy) filter matrices
            filterMatricesReal = new Lazy<Matrix[]>(() =>
            {
                var filters = new Matrix[NbOrientations];
                for (int i = 0; i < NbOrientations; i++)
                    filters[i] = CreateFilter(i * Math.PI / NbOrientations, /*phase:*/0, WaveLength, AspectRatio, Bandwidth);
                
                return filters;
            });

            filterMatricesImaginary = new Lazy<Matrix[]>(() =>
            {
                var filters = new Matrix[NbOrientations];
                for (int i = 0; i < NbOrientations; i++)
                    filters[i] = CreateFilter(i * Math.PI / NbOrientations, /*phase:*/Math.PI / 2, WaveLength, AspectRatio, Bandwidth);
                
                return filters;
            });
        }

        /// <summary>
        /// Create a 2D Gabor filter like in the example at http://en.wikipedia.org/wiki/Gabor_filter
        /// </summary>
        private Matrix CreateFilter(double angle, double phase, double waveLength, double aspectRatio, double bandwidth)
        {
            int multiplicator = 3;

            // Sigma's
            var sigmaX = waveLength / Math.PI * Math.Sqrt(Math.Log(2) / 2) * (Math.Pow(2, bandwidth) + 1) / (Math.Pow(2.0, bandwidth) - 1);
            var sigmaY = sigmaX / aspectRatio;

            // Bounding box            
            int halfSize = (int)Math.Ceiling(multiplicator * Math.Max(sigmaX, sigmaY));
            var size = /*symmetrical values:*/2 * halfSize + /*the middle zero:*/1;

            // Filter matrix
            Matrix filterMatrix = new DenseMatrix(size);

            for (int x = -halfSize, xPos = 0; x <= halfSize; x++, xPos++)
            {
                for (int y = -halfSize, yPos = 0; y <= halfSize; y++, yPos++)
                {
                    double xTheta = x * Math.Cos(angle) + y * Math.Sin(angle);
                    double yTheta = -x * Math.Sin(angle) + y * Math.Cos(angle);

                    filterMatrix[yPos, xPos] = GaborFunction(xTheta, yTheta, sigmaX, sigmaY, waveLength, phase);
                }
            }

            var normalized = filterMatrix.Normalize(-1, 1);
            filterMatrix = normalized.ShiftAndFilterToZero(normalized.Mean(), 0.05);

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
            var outputs = new Matrix<double>[NbOrientations];

            for (int i = 0; i < NbOrientations; i++)
            {
                var real = input.Convolve(FilterMatricesReal[i]);
                var imaginary = input.Convolve(FilterMatricesImaginary[i]);

                outputs[i] = (real.PointwisePower(2.0) + imaginary.PointwisePower(2.0)).PointwisePower(0.5);

                if (OnFilterCreated != null)
                    OnFilterCreated(this, new OutputEventArgs<Matrix>((Matrix)outputs[i], "output#" + i));
            }

            var output = outputs[0];
            switch (OutputMode)
            {
                case MultipleOutputFilterMode.Concatenated:
                    output = outputs[0];
                    for (int i = 1; i < outputs.Length; i++)
                    {
                        output = (Matrix)output.Stack(outputs[i]);
                    }
                    break;
                case MultipleOutputFilterMode.Interleaved:
                    throw new NotImplementedException();

                case MultipleOutputFilterMode.SuperImposed:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentException();
            }
            return (SparseMatrix)output;
        }


    }
}
