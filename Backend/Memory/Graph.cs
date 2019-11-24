using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fNIRS.Memory
{
    public class GraphModel : Dictionary<int, ICollection<int>>
    {
    }

    public class NodeValue
    {
        public double AC { get; set; }
        public double Phase { get; set; }
    }

    public class GraphValues
    {
        public int Index { get; set; }
        public double? AcMin { get; set; }
        public double? AcMax { get; set; }
        public Dictionary<int, Dictionary<int, NodeValue>> Values { get; set; }

        public GraphValues(int index)
        {
            this.Index = index;
            this.Values = new Dictionary<int, Dictionary<int, NodeValue>>();
        }

        public void AddDetector(int detector)
        {
            Values.Add(detector, new Dictionary<int, NodeValue>());
        }

        public void SetSourceValue(int detector, int source, NodeValue value)
        {
            AcMax = !AcMax.HasValue ? value.AC : Math.Max(value.AC, AcMax.Value);
            AcMin = !AcMin.HasValue ? value.AC : Math.Min(value.AC, AcMin.Value);

            if (Values.TryGetValue(detector, out var detectorDict))
            {
                detectorDict.Add(source, value);
            }
        }
    }
}
