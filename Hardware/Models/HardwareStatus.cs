using System;

namespace fNIRS.Hardware.Models
{
    public class HardwareStatus
    {
        public HardwareStatus()
        {
            ADC = new ModuleStatus();
            PMT = new ModuleStatus();
            DDS = new ModuleStatus();
        }

        public DateTime Time { get; set; }
        public int MaxBytes { get; set; }
        public string Version { get; set; }
        public ModuleStatus ADC { get; set; }
        public ModuleStatus PMT { get; set; }
        public ModuleStatus DDS { get; set; }

        public override string ToString()
        {
            return $"Hardware status " +
                $"Time: {this.Time} " +
                $"MaxBytes: {this.MaxBytes} ";
        }
    }

    public class ModuleStatus
    {
        public bool PrimaryConnected { get; set; }
        public bool PrimaryVirtualConnected { get; set; }
        public bool SecondaryConnected { get; set; }
        public bool SecondaryVirtualConnected { get; set; }
    }
}
