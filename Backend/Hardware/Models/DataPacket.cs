using System;
using System.Collections.Generic;
using fNIRS.Hardware.ISS;
using Newtonsoft.Json;

namespace fNIRS.Hardware.Models
{
    public class DataPacket
    {
        public DataPacket() 
        {
            this.Cycles = new List<CycleData>();
        }

        public int Index { get; set; }
        public int Size { get; set; }
        public string Time { get; set; }
        [JsonIgnore]
        public HeaderData Header { get; set; }
        [JsonIgnore]
        public List<CycleData> Cycles { get; set; }
        // public ICollection<SourceDetector> Values { get; set; }
    }
}
