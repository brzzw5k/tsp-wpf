using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace tsp_shared
{
    [Serializable]
    public class Vertex
    {
        public Vector2 Point { get; set; }
        public int Number { get; set; }

        public Vertex(int number, Vector2 point)
        {
            Number = number;
            Point = point;
        }
    }
}
