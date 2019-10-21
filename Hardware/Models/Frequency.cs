
namespace fNIRS.Hardware.Models 
{
    public class Frequency {

        public Frequency(int index, string value)
        {
            this.Index = index;
            this.Value = value;
        }

        public int Index { get; set; }
        public string Value { get; set; }
    }
}