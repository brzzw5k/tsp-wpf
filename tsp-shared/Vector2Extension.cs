using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace tsp_shared
{
    public static class Vector2Extension
    {
        public static float Distance(this Vector2 vector, Vector2 other)
        {
            return (float)Math.Sqrt(Math.Pow(vector.X - other.X, 2) + Math.Pow(vector.Y - other.Y, 2));
        }
    }
}
