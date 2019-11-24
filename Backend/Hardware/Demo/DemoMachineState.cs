using fNIRS.Hardware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fNIRS.Hardware.Demo
{
    public class DemoMachineState
    {
        public DemoMachineState()
        {
            Frequency = 14;
            CycleNum = 8;
        }

        public int CycleNum { get; set; }
        public int Switch { get; set; }
        public int Frequency { get; set; }
        public HardwareStatus HardwareStatus { get; set; }
    }
}
