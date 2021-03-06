﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using D2D = System.Drawing.Drawing2D;

using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Exceptions;
using System.Diagnostics;
using CnrsUniProv.OCodeHtm.Interfaces;

namespace CnrsUniProv.OCodeHtm
{
    public class BitmapPicture2DSensor : Sensor<SparseMatrix>
    {
        public const double MAX_VALUE = 1.0;

        private int height;
        public int Height
        {
            get
            {
                if (height == Default.AutomaticSize)
                {
                    try
                    {
                        if (CurrentInputFile == null || CurrentInputOriginalImage == null)
                            throw new HtmException("No current input file: wait for the first input to be loaded");

                        height = CurrentInputOriginalImage.Height;
                        width = CurrentInputOriginalImage.Width;
                    }
                    catch (Exception e)
                    {
                        throw new HtmException("Cannot automatically determine the size of the sensor from the current input file", this, e);
                    }
                }
                return height;
            }
            set
            {
                if (height > 0)
                    throw new HtmRuleException("Cannot change the size of the sensor dynamically", this);
                height = value;
            }
        }

        private int width;
        public int Width
        {
            get
            {
                if (width == Default.AutomaticSize)
                {
                    var temp = Height; // delegating the loading of sizes to the Height property getter
                }
                return width;
            }
            set
            {
                if (width > 0)
                    throw new HtmRuleException("Cannot change the size of the sensor dynamically", this);
                width = value;
            }
        }


        public FileInfo CurrentInputFile { get; private set; }
        public Bitmap CurrentInputOriginalImage { get; set; }
        public string CurrentCategory { get; private set; }


        private ExplorationPath CurrentPath { get; set; }

        public int CurrentPosV { get; private set; }
        public int CurrentPosH { get; private set; }

        

        public int CurrentPathDeltaV { get; private set; }
        public int CurrentPathDeltaH { get; private set; }


        public float CurrentRotationAngleDegrees { get; private set; }
        public float CurrentScale { get; private set; }

        public float CurrentRotationAngleDelta { get; private set; }
        public float CurrentScaleDelta { get; private set; }



        public BitmapPicture2DSensor(uint height = Default.AutomaticSize, uint width = Default.AutomaticSize, 
            int randomizerSeed = Default.RandomizerSeed, int presentationsPerInput = Default.SinglePresentation, TrainingOrder trainingOrder = TrainingOrder.Normal,
            uint maxIterations = Default.NoMaxIterations, uint pathSpeed = Default.PathSpeed, bool useRandomOrigin = Default.PathUseRandomOrigin, // TODOlater use enum {Center,Left,RandomOutsideSensor?,Random} for origins
            float rotationAngleMaxDegrees = Default.NoRandomRotationAngle, float rotationSpeed = Default.NoRandomRotationSpeed,
            float scalingMin = Default.NoRandomScalingFactor, float scalingMax = Default.NoRandomScalingFactor, float scalingSpeed = Default.NoRandomScalingSpeed,
            params ExplorationPath[] explorationPaths)

            : base(randomizerSeed, presentationsPerInput, trainingOrder, explorationPaths, maxIterations, pathSpeed, useRandomOrigin, 
                    rotationAngleMaxDegrees, rotationSpeed, scalingMin, scalingMax, scalingSpeed)
        {
            Height = (int)height;
            Width = (int)width;


            // More default values:
            InputFilenameMask = Default.BitmapInputFilenameMask;

            if (explorationPaths.Length == 0)
            {
                Trace.WriteLine("Warning: " + ExplorationPath.RandomSweep4Axes.GetType().Name + " added to " + GetType().Name + " since none was specified.");
                ExplorationPaths.Add(ExplorationPath.RandomSweep4Axes);
            }
        }


        /// <summary>
        /// Warning: This is a low-level event that is fired even for blank outputs that get ignored further in the sensor logic
        /// </summary>
        public event OutputEventHandler<Bitmap> OnTransformedBitmapOutput;
        public event OutputEventHandler<Matrix> OnTransformedMatrixOutput;
        public event OutputEventHandler<Matrix> OnFilteredMatrixOutput;

        /// <summary>
        /// Set the current input file before starting enumerating exploration iterations
        /// </summary>
        /// <param name="file"></param>
        protected override void SetCurrentInputFile(FileInfo file, string category)
        {
            CurrentInputFile = file;
            CurrentCategory = category;

            CurrentInputOriginalImage = new Bitmap(file.FullName);

            // Initialize origin (by default, leave the input in the center of the sensor)
            if (ExplorationPathUseRandomOrigin)
            {
                CurrentPosH = RandomGenerator.Next(-Width / 2, Width / 2 + 1);
                CurrentPosV = RandomGenerator.Next(-Height / 2, Height / 2 + 1);
            }
            else
            {
                CurrentPosH = 0;
                CurrentPosV = 0;
            }
        }

       

        protected override IEnumerator<SparseMatrix> GetIterationsEnumerator(bool useTransformations)
        {
            if (!useTransformations)
            {
                yield return Filter(TransformNextInput());
            }
            else
                foreach (var path in ExplorationPaths)
                {
                    CurrentPath = path;

                    InitPathTranslation();
                    InitRotation();
                    InitScaling();
                    
                    var output = SparseMatrix.Identity(2); // initialize with non-"grey" output

                    for (int iteration = 0; iteration < ExplorationPathMaxIterations; iteration++)
                    {
                        output = TransformNextInput();
                        if (output.IsBlank())
                            break;
                        // notify observers about the matrix output
                        if (OnTransformedMatrixOutput != null)
                            OnTransformedMatrixOutput(this, new OutputEventArgs<Matrix>(output, CurrentCategory));


                        output = Filter(output);
                        // notify observers about the filtered output
                        if (OnFilteredMatrixOutput != null)
                            OnFilteredMatrixOutput(this, new OutputEventArgs<Matrix>(output, CurrentCategory));

                        yield return output;
                        IncrementTransformationParameters();
                    }
                }
        }

               

        private SparseMatrix TransformNextInput()
        {
            var image = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(image);
            
            var transformMatrix = new D2D.Matrix();            
            transformMatrix.RotateAt(CurrentRotationAngleDegrees, new Point(Width / 2, Height / 2), D2D.MatrixOrder.Append);
            transformMatrix.Scale(CurrentScale, CurrentScale, D2D.MatrixOrder.Append);
            transformMatrix.Translate(CurrentPosH, CurrentPosV, D2D.MatrixOrder.Append);

            var graphicsPath = new D2D.GraphicsPath();
            graphicsPath.AddPolygon(new Point[] { new Point(0, 0), new Point(Width, 0), new Point(0, Height) });
            graphicsPath.Transform(transformMatrix);

            graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, Width, Height);
            graphics.DrawImage(CurrentInputOriginalImage, graphicsPath.PathPoints);

            // notify observers about the bitmap output just created
            if (OnTransformedBitmapOutput != null)
                OnTransformedBitmapOutput(this, new OutputEventArgs<Bitmap>(image, CurrentCategory));

            var output = new SparseMatrix(Height, Width);
            for (int col = 0; col < output.ColumnCount; col++)
            {
                for (int row = 0; row < output.RowCount; row++)
                {
                    if (image.GetPixel(col, row).GetBrightness() > 0)
                    { output.At(row, col, MAX_VALUE); }
                }
            }

            return output;
        }

        

        private SparseMatrix Filter(SparseMatrix input)
        {
            var output = input;

            foreach (var filter in Filters)
            {
                output = filter.Filter(output);
            }

            return output;
        }



        private void InitPathTranslation()
        {
            // Path direction (translation)
            switch (CurrentPath)
            {
                case ExplorationPath.RandomSweep4Axes:
                    CurrentPathDeltaH = RandomGenerator.Next(-1, 2);
                    CurrentPathDeltaV = RandomGenerator.Next(-1, 2);
                    // Check
                    if (CurrentPathDeltaH == 0 && CurrentPathDeltaV == 0)
                        switch (RandomGenerator.Next(3))
                        {
                            case 0:
                                CurrentPathDeltaH = RandomGenerator.NextSign();
                                break;
                            case 1:
                                CurrentPathDeltaV = RandomGenerator.NextSign();
                                break;
                            default:
                                CurrentPathDeltaH = RandomGenerator.NextSign();
                                CurrentPathDeltaV = RandomGenerator.NextSign();
                                break;
                        }
                    break;
                case ExplorationPath.LeftToRightSweep:
                    CurrentPathDeltaH = 1;
                    CurrentPathDeltaV = 0;
                    break;
                case ExplorationPath.TopToBottomSweep:
                    CurrentPathDeltaH = 0;
                    CurrentPathDeltaV = 1;
                    break;
                default:
                    break;
            }
            
            CurrentPathDeltaH *= ExplorationPathSpeed;
            CurrentPathDeltaV *= ExplorationPathSpeed;
        }

        private void InitRotation()
        {
            CurrentRotationAngleDegrees = Convert.ToSingle(RandomGenerator.NextDouble() * 2 * ExplorationRandomRotationAngleMaxDegrees - ExplorationRandomRotationAngleMaxDegrees);
            
            CurrentRotationAngleDelta = RandomGenerator.NextSign() * ExplorationRandomRotationSpeed;
        }

        private void InitScaling()
        {
            CurrentScale = Convert.ToSingle(RandomGenerator.NextDouble() * (ExplorationScalingMax - ExplorationScalingMin) + ExplorationScalingMin);

            CurrentScaleDelta = RandomGenerator.NextSign() * ExplorationScalingSpeed;
        }


        private void IncrementTransformationParameters()
        {
            // Increment transformation params:
            // Translation
            CurrentPosH += CurrentPathDeltaH;
            CurrentPosV += CurrentPathDeltaV;

            // Rotation
            if (Math.Abs(CurrentRotationAngleDegrees + CurrentRotationAngleDelta) <= ExplorationRandomRotationAngleMaxDegrees)
                CurrentRotationAngleDegrees += CurrentRotationAngleDelta;

            // Scaling
            if (CurrentScale + CurrentScaleDelta >= ExplorationScalingMin
                && CurrentScale + CurrentScaleDelta <= ExplorationScalingMax)
                CurrentScale += CurrentScaleDelta;
        }


        /// <summary>
        /// Determines if the current input is shown outside of the area surrounding the sensor 
        /// (its top left origin coordinates CurrentPosH/V should remain between: 
        /// -Width&lt;=x&lt;=Width and -Height&lt;=y&lt;=Height)
        /// </summary>
        public bool IsCurrentInputOutsideField()
        {
            return CurrentPosH < -Width || CurrentPosH > Width
                    || CurrentPosV < -Height || CurrentPosV > Height;
        }
    }
}
