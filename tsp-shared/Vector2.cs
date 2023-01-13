using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsp_shared
{
    [Serializable]
    public class Vector2
    {
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public double Distance(Vector2 other)
        {
            var xDiff = other.X - X;
            var yDiff = other.Y - Y;

            return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
    }
}
