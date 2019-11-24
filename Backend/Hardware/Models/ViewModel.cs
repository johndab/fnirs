using System.Collections.Generic;

namespace fNIRS.Hardware.Models
{
    public class ViewModel
    {
        public int Index { get; set; }
        public ICollection<SourceDetector> Values { get; set; }
    }

    public class SourceDetector
    {
        public int Source { get; set; }
        public int Detector { get; set; }
        public double AC { get; set; }
    }
}