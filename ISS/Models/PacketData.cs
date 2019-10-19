using System;
using System.Collections.Generic;
using System.Text;

namespace MiBrain.ISS.Models
{
    public class PacketData
    {
        public int Index { get; set; }
        public int Size { get; set; }
        public string Time { get; set; }
        public Byte[] data { get; set; }
    }
}
