using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra.Double;
using System.IO;

namespace CnrsUniProv.OCodeHtm
{
    public class BitmapPictureSensor : Sensor<SparseMatrix>
    {
        public int Height { get; private set; }
        public int Width { get; private set; }

        public FileInfo CurrentInputFile { get; private set; }


        public BitmapPictureSensor(params ExplorationPath[] explorationPaths)
            : this(Default.RandomizerSeed, Default.NoMaxIterations, Default.PathSpeed, Default.PathUseRandomOrigin,
                    explorationPaths)
        { }


        public BitmapPictureSensor(int randomizerSeed = Default.RandomizerSeed, uint maxIterations = Default.NoMaxIterations, uint pathSpeed = Default.PathSpeed, bool useRandomOrigin = Default.PathUseRandomOrigin, //TODOlater use enum {OutsideField,Center,Random} for origins
            params ExplorationPath[] explorationPaths)
            : base(randomizerSeed, explorationPaths, maxIterations, pathSpeed, useRandomOrigin)
        {
            // More default values, for good measure:
            InputFilenameFormat = Default.BitmapInputFilenameFormat;

            if (explorationPaths.Length == 0)
                ExplorationPaths.Add(Default.Path);
        }




        //TODO
        protected override void SetCurrentInputFile(FileInfo file)
        {
            throw new NotImplementedException();
        }

        protected override SparseMatrix GetNextIteration(bool useTransformations)
        {
            throw new NotImplementedException();
        }

        protected override SparseMatrix FilterInput(SparseMatrix input)
        {
            throw new NotImplementedException();
        }
    }
}
