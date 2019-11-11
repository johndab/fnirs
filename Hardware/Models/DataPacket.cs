using System;
using System.Collections.Generic;
using fNIRS.Hardware.ISS;
using Newtonsoft.Json;

namespace fNIRS.Hardware.Models
{
    public class DataPacket
    {
        public DataPacket() {
            this.DataIndex = 0;
            this.ReadIndex = 0;
            this.Cycles = new List<CycleData>();
        }
        public int Index { get; set; }
        public int Size { get; set; }
        public string Time { get; set; }
        public int DataIndex { get; set; }
        public int ReadIndex { get; set; }
        [JsonIgnore]
        public Byte[] Data { get; set; }
        public HEADERDATA6? Header { get; set; }
        public List<CycleData> Cycles { get; set; }
        [JsonIgnore]
        public ICollection<Detector> Detectors { get; set; }
    }

    public class Detector
    {
        public int Address { get; set; }
        public int Value { get; set; }
    }
}
