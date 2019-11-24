using System;
using System.Collections.Generic;
using fNIRS.Hardware.Models;
using System.Linq;
using System.Runtime.InteropServices;

namespace fNIRS.Hardware
{
    public static class ModelConverter
    {
        public unsafe static ViewModel ToModel(this DataPacket packet)
        {
            return new ViewModel()
            {
                Index = packet.Index,
                Values = GetSourceDetector(packet.Cycles)
            };
        }

        public static ICollection<SourceDetector> GetSourceDetector(
            ICollection<CycleData> data)
        {
            // TODO: Consider all cycles
            return GetCycle(data.First());
        }

        public static ICollection<SourceDetector> GetCycle(CycleData cycle)
        {
            var result = new List<SourceDetector>();

            for(int detector = 1; detector <= 32; detector++)
            {
                var detectorData = cycle.Values
                    .Where(x => x.Detector == detector)
                    .ToArray();

                for(int source = 1; source <= 32; source++)
                {
                    var row = source % cycle.Mode;
                    var measurement = detectorData[row];
                    
                    var i = source / cycle.Mode;
                    var imag = measurement.Imag[i];
                    var real = measurement.Real[i];

                    result.Add(new SourceDetector()
                    {
                        Detector = detector,
                        Source = source,
                        AC = Math.Sqrt(Math.Pow(imag, 2) +  Math.Pow(real, 2)),
                    });
                }
            }

            return result;
        }


    }

}