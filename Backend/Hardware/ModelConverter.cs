using System;
using System.Collections.Generic;
using fNIRS.Hardware.Models;
using System.Linq;
using System.Runtime.InteropServices;
using fNIRS.Memory;

namespace fNIRS.Hardware
{
    public static class ModelConverter
    {
        public unsafe static GraphValues ToModel(this DataPacket packet, GraphModel graph)
        {
            GraphValues valueGraph = new GraphValues(packet.Index);

            foreach (var key in graph.Keys)
            {
                valueGraph.AddDetector(key);
            }

            ProcessCycle(packet.Cycles.First(), graph, valueGraph);

            return valueGraph;
        }

        public static void ProcessCycle(CycleData cycle, GraphModel graph, GraphValues values)
        {
            foreach(var detector in graph.Keys)
            {
                var detectorData = cycle.Values
                    .Where(x => x.Detector == detector)
                    .ToArray();

                graph.TryGetValue(detector, out var sources);

                foreach(var source in sources)
                {
                    try
                    {
                        var row = (source - 1) % cycle.Mode;
                        var measurement = detectorData[row];
                    
                        var i = source / cycle.Mode;
                        var imag = measurement.Imag[i];
                        var real = measurement.Real[i];

                        var ac = Math.Sqrt(Math.Pow(imag, 2) + Math.Pow(real, 2));
                        var phase = Math.Atan2(imag, real);

                        values.SetSourceValue(detector, source, new NodeValue()
                        {
                            AC = ac,
                            Phase = phase
                        });
                    }
                    catch
                    { }
                }
            }
        }


    }

}