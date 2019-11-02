using System;
using System.Collections.Generic;
using System.Text;

namespace fNIRS.Hardware.Models
{
    public class DataPacket
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public string Time { get; set; }
        // public Byte[] data { get; set; }
        public ICollection<Detector> Detectors { get; set; }
    }

    public class Detector
    {
        public int Address { get; set; }
        public int Value { get; set; }
    }
}
