
namespace fNIRS.Hardware.Models
{
    public class CycleData
    {
        public uint Number { get; set; }
        public int Mode { get; set; }
        public DC[] DCData { get; set; }
    }

    public class DC
    {
        public int Row;
        public int Col;
        public double Value;
        public double[] Real;
        public double[] Imag;
    }
}