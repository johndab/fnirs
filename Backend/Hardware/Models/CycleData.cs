using System.Collections.Generic;

namespace fNIRS.Hardware.Models
{
    public class CycleData
    {
        public uint Index { get; set; }
        public int Mode { get; set; }
        public ICollection<DetectorMeasurement> Values { get; set; }
    }

    public class DetectorMeasurement
    {
        public int Detector { get; set; }
        public int Measurement { get; set; }
        public double DC { get; set; }
        public double[] Real;
        public double[] Imag;
    }
}