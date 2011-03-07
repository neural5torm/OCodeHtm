using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CnrsUniProv.OCodeHtm
{/*
    class Temp
    {
        void main()
        {
            var net = new HtmNetwork();

            net.AddSensor(new BitmapPictureSensor(32, 32, "ocode/clean", "ocode/test"));
            net.AddLayer(new SpatialLayer<SpatialNodeGaussian>(20, 20, 0.0));
            net.AddLayer(new TemporalLayer(20, 20, 0.0));
            net.AddClassifier(new ProductMatchClassifierNode());

            net.Sensor.Exploration = "RandomSweep";

            Train(net);

        }

        void Train(object net)
        {
            // Train layers
            for (int i = 0; i < net.Layers.Count; ++i)
            {
                foreach (var input in net.Sensor.GetTrainingInputs())
                {
                    foreach (var sensorSweepIteration in input.GetIterations())
                    {
                        // Get output from all previously trained layers
                        var output = sensorSweepIteration.GetOutput();
                        for (int iTrained = 0; j < i; ++j)
                        {
                            output = net.Layers[iTrained].Infer(output);
                        }

                        net.Layers[i].Learn(output);
                    }
                }
            }

            // Train classifier
            foreach (var input in net.Sensor.GetTrainingInputs())
            {
                var cat = input.GetCategory();

                foreach (var sensorSweepIteration in input.GetIterations())
                {
                    var output = sensorSweepIteration.GetOutput();
                    for (int i = 0; i < net.Layers.Count; ++i)
                    {
                        output = net.Layers[i].Infer(output);
                    }

                    net.Classifier.Learn(output, cat);
                }
            }
        }
    }*/
}
