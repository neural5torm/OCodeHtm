using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra.Double;
using System.IO;
using CnrsUniProv.OCodeHtm.Exceptions;

namespace CnrsUniProv.OCodeHtm
{
    public class BitmapPictureSensor : Sensor<SparseMatrix>
    {
        private int height;
        public int Height
        {
            get
            {
                if (height == Default.AutomaticSize)
                {
                    try
                    {
                        if (CurrentInputFile == null)
                            throw new Exception("No current input file");

                        height = originalImage.Height;
                        width = originalImage.Width;
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

        private Bitmap originalImage;
        public Bitmap OriginalImage
        {
            get { return new Bitmap(originalImage); }
            private set { originalImage = value; }
        }

        public int CurrentPosV { get; private set; }
        public int CurrentPosH { get; private set; }

        public int CurrentPathDeltaV { get; private set; }
        public int CurrentPathDeltaH { get; private set; }
        

        public BitmapPictureSensor(params ExplorationPath[] explorationPaths)
            : this(Default.AutomaticSize, Default.AutomaticSize, Default.RandomizerSeed, Default.SinglePresentation, TrainingOrder.Normal, Default.NoMaxIterations, Default.PathSpeed, Default.PathUseRandomOrigin,
                    explorationPaths)
        { }


        public BitmapPictureSensor(uint height = Default.AutomaticSize, uint width = Default.AutomaticSize, 
            int randomizerSeed = Default.RandomizerSeed, int presentationsPerInput = Default.SinglePresentation, TrainingOrder trainingOrder = TrainingOrder.Normal,
            uint maxIterations = Default.NoMaxIterations, uint pathSpeed = Default.PathSpeed, bool useRandomOrigin = Default.PathUseRandomOrigin, //TODOlater use enum {Center,Random,RandomOutsideField?} for origins
            params ExplorationPath[] explorationPaths)
            : base(randomizerSeed, presentationsPerInput, trainingOrder, explorationPaths, maxIterations, pathSpeed, useRandomOrigin)
        {
            Height = (int)height;
            Width = (int)width;


            // More default values, for good measure:
            InputFilenameFormat = Default.BitmapInputFilenameFormat;

            if (explorationPaths.Length == 0)
                ExplorationPaths.Add(ExplorationPath.RandomSweep4Axes);
        }




        /// <summary>
        /// Set the current input file before starting enumerating exploration iterations
        /// </summary>
        /// <param name="file"></param>
        protected override void SetCurrentInputFile(FileInfo file)
        {
            CurrentInputFile = file;
            OriginalImage = new Bitmap(file.FullName);
            
            //TODOnow
            if (ExplorationPathUseRandomOrigin)
            {
               // ChooseRandomOrigin();
            }
            
            // ChooseRandomPathDelta();


           

        }

        protected override IEnumerator<SparseMatrix> GetEnumerator(bool useTransformations)
        {
            SparseMatrix input = new SparseMatrix(5);//TODO

            if (!useTransformations)
            {
                yield return FilterInput(input);
            }
            else
                while (true)
                {
                    //TODO transformations


                    //input.IndexedEnumerator

                    yield return FilterInput(input);
                }

            yield break;
        }

        protected SparseMatrix FilterInput(SparseMatrix input)
        {
            //TODO (add filters)
            return input;
        }
    }
}
