﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CnrsUniProv.OCodeHtm;
using System.IO;


namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string TrainingSetPath = Path.Combine("O:", "clean");


            var filter = new Gabor2DFilter();
            var writer = new MatrixToBitmapFileWriter("");

            var sensor = new BitmapPicture2DSensor(presentationsPerInput: 1, pathSpeed: 2, useRandomOrigin: true);
            sensor.SetTrainingFolder(TrainingSetPath);
            sensor.AddFilter(filter);
            sensor.OnFilteredMatrixOutput += writer.OutputWriterHandler;

            int maxIterations = 2, nbIterations = 0;
            foreach (var input in sensor.GetTrainingInputs(true))
            {
                foreach (var iteration in input)
                {
                    nbIterations++;
                    if (nbIterations >= maxIterations)
                        break;
                }
                if (nbIterations >= maxIterations)
                    break;
            }

        }
    }
}
