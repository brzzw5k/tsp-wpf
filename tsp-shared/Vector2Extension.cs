using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace tsp_shared
{
    public static class Vector2Extension
    {
        public static double Distance(this Vector2 vector, Vector2 other)
        {
            var xDiff = other.X - vector.X;
            var yDiff = other.Y - vector.Y;

            return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
    }

}
