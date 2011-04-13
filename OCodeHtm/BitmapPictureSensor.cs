﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using D2D = System.Drawing.Drawing2D;

using MathNet.Numerics.LinearAlgebra.Double;
using CnrsUniProv.OCodeHtm.Exceptions;
using CnrsUniProv.OCodeHtm.Interfaces;

namespace CnrsUniProv.OCodeHtm
{
    public class BitmapPictureSensor : Sensor<SparseMatrix>
    {
        public const double MAX_VALUE = 255.0;

        private int height;
        public int Height
        {
            get
            {
                if (height == Default.AutomaticSize)
                {
                    try
                    {
                        if (CurrentInputFile == null || OriginalImage == null)
                            throw new Exception("No current input file");

                        height = OriginalImage.Height;
                        width = OriginalImage.Width;
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
        public string CurrentCategory { get; private set; }

        public Bitmap OriginalImage { get; set; }

        public int CurrentPosV { get; private set; }
        public int CurrentPosH { get; private set; }

        private ExplorationPath CurrentPath { get; set;}

        public int CurrentPathDeltaV { get; private set; }
        public int CurrentPathDeltaH { get; private set; }


        /// <summary>
        /// Determines if the current input is shown outside of the area surrounding the sensor 
        /// (its top left origin should remain between -Width&lt;=X&lt;=Width and -Height&lt;=Y&lt;=Height)
        /// </summary>
        public bool IsOutsideField
        {
            get
            {
                return CurrentPosH < -Width || CurrentPosH > Width
                    || CurrentPosV < -Height || CurrentPosV > Height;
            }
        }
        


        public BitmapPictureSensor(uint height = Default.AutomaticSize, uint width = Default.AutomaticSize, 
            int randomizerSeed = Default.RandomizerSeed, int presentationsPerInput = Default.SinglePresentation, TrainingOrder trainingOrder = TrainingOrder.Normal,
            uint maxIterations = Default.NoMaxIterations, uint pathSpeed = Default.PathSpeed, bool useRandomOrigin = Default.PathUseRandomOrigin, //TODOlater use enum {Center,Left,RandomOutsideSensor?,Random} for origins
            params ExplorationPath[] explorationPaths)
            : base(randomizerSeed, presentationsPerInput, trainingOrder, explorationPaths, maxIterations, pathSpeed, useRandomOrigin)
        {
            Height = (int)height;
            Width = (int)width;


            // More default values, for good measure:
            InputFilenameMask = Default.BitmapInputFilenameMask;

            if (explorationPaths.Length == 0)
                ExplorationPaths.Add(ExplorationPath.RandomSweep4Axes);
        }



        public event OutputEventHandler<Bitmap> OnBitmapOutput;
        public event OutputEventHandler<Matrix> OnMatrixOutput;

        /// <summary>
        /// Set the current input file before starting enumerating exploration iterations
        /// </summary>
        /// <param name="file"></param>
        protected override void SetCurrentInputFile(FileInfo file, string category)
        {
            CurrentInputFile = file;
            CurrentCategory = category;

            OriginalImage = new Bitmap(file.FullName);

            // Initialize origin (by default, leave the input in the center of the sensor)
            if (ExplorationPathUseRandomOrigin) 
            {
                CurrentPosH = RandomGenerator.Next(Width + 1) - Width / 2;
                CurrentPosV = RandomGenerator.Next(Height + 1) - Height / 2;
            }        
        }

       

        protected override IEnumerator<SparseMatrix> GetEnumerator(bool useTransformations)
        {
            if (!useTransformations)
            {
                yield return FilterInput(GetNextTransformedInput());
            }
            else
                foreach (var path in ExplorationPaths)
                {
                    CurrentPath = path;
                    InitPathDirection();

                    SparseMatrix input = new SparseMatrix(1, 1, 1);
                    for (int iteration = 0; iteration < ExplorationPathMaxIterations; iteration++)
                    {
                        if (IsOutsideField || input.FrobeniusNorm() == 0.0)
                            break;

                        input = GetNextTransformedInput();
                        yield return FilterInput(input);
                    }
                }
        }

        

        private SparseMatrix GetNextTransformedInput()
        {
            var image = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(image);
            
            var transformMatrix = new D2D.Matrix();            
            transformMatrix.RotateAt(ExplorationRandomRotationMaxAngle, new Point(Width / 2, Height / 2), D2D.MatrixOrder.Append);
            transformMatrix.Scale(ExplorationScalingMax, ExplorationScalingMax, D2D.MatrixOrder.Append);
            transformMatrix.Translate(CurrentPosH, CurrentPosV, D2D.MatrixOrder.Append);

            var graphicsPath = new D2D.GraphicsPath();
            graphicsPath.AddPolygon(new Point[] { new Point(0, 0), new Point(Width, 0), new Point(0, Height) });
            graphicsPath.Transform(transformMatrix);

            graphics.FillRectangle(new SolidBrush(Color.Black), 0, 0, Width, Height);
            graphics.DrawImage(OriginalImage, graphicsPath.PathPoints);

            // notify observers about the bitmap output just created
            if (OnBitmapOutput != null)
                OnBitmapOutput(this, new OutputEventArgs<Bitmap>(image, CurrentCategory));

            var input = new SparseMatrix(Height, Width);
            for (int i = 0; i < input.ColumnCount; i++)
            {
                for (int j = 0; j < input.RowCount; j++)
                {
                    if (image.GetPixel(i, j).GetBrightness() > 0)
                    { input.At(i, j, MAX_VALUE); }
                }
            }

            // Increment transformation params:
            // Translation
            CurrentPosH += CurrentPathDeltaH;
            CurrentPosV += CurrentPathDeltaV;

            // TODO Rotation
            // Scaling

            // notify observers about the matrix output
            if (OnMatrixOutput != null)
                OnMatrixOutput(this, new OutputEventArgs<Matrix>(input, CurrentCategory));

            return input;
        }

        private SparseMatrix FilterInput(SparseMatrix input)
        {
            //TODOnow add filters
            return input;
        }


        
        private void InitPathDirection()
        {
            // Path direction (translation)
            switch (CurrentPath)
            {
                case ExplorationPath.RandomSweep4Axes:
                    CurrentPathDeltaH = RandomGenerator.Next(3) - 1;
                    CurrentPathDeltaV = RandomGenerator.Next(3) - 1;
                    // Check
                    if (CurrentPathDeltaH == 0 && CurrentPathDeltaV == 0)
                        switch (RandomGenerator.Next(3))
                        {
                            case 0:
                                CurrentPathDeltaH = RandomGenerator.Next(2) > 0 ? -1 : 1;
                                break;
                            case 1:
                                CurrentPathDeltaV = RandomGenerator.Next(2) > 0 ? -1 : 1;
                                break;
                            default:
                                CurrentPathDeltaH = RandomGenerator.Next(2) > 0 ? -1 : 1;
                                CurrentPathDeltaV = RandomGenerator.Next(2) > 0 ? -1 : 1;
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

            // TODO Rotation
            // Scaling
        }
    }
}
