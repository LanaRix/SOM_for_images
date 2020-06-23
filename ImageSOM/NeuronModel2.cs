using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSOM
{
    public class NeuronModel2
    {
        RGB [] m;

        public RGB[] M { get => m; set => m = value; }
    }
    public class RGB
    {
        int r, g, b;

        public int R { get => r; set => r = value; }
        public int G { get => g; set => g = value; }
        public int B { get => b; set => b = value; }
    }
}
