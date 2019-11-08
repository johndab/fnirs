using System;
using System.Collections.Generic;
using System.Text;
using fNIRS.Hardware.ISS;

namespace fNIRS.Hardware.Models
{
    public class DataPacket
    {
        public DataPacket() {
            this.DataIndex = 0;
            this.ReadIndex = 0;
            this.Cycles = new List<CYCLEDATA6>();
        }
        public int Index { get; set; }
        public int Size { get; set; }
        public string Time { get; set; }
        public int DataIndex { get; set; }
        public int ReadIndex { get; set; }
        public Byte[] Data { get; set; }
        public HEADERDATA6? Header { get; set; }
        public List<CYCLEDATA6> Cycles { get; set; }
        public ICollection<Detector> Detectors { get; set; }
    }

    public class Detector
    {
        public int Address { get; set; }
        public int Value { get; set; }
    }
}
