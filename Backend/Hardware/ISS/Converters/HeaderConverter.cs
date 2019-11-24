using System;
using System.Collections.Generic;
using fNIRS.Hardware.Models;
using fNIRS.Hardware.ISS;
using System.Runtime.InteropServices;

namespace fNIRS.Hardware.ISS.Converters
{
    public static class HeaderConverter
    {
        public unsafe static HeaderData ToModel(this HEADERDATA6 header)
        {
            return new HeaderData()
            {
                NumberOfCycles = header.NumCyclesPerDataPacket,
                MissedCycles = header.missed_cycles,
            };
        }

    }

}