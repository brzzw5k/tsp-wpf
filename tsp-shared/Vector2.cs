using System;
using System.Collections.Generic;
using System.Text;

namespace tsp_shared
{
    [Serializable]
    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }
        
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public float Distance(Vector2 other)
        {
            return (float)Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }
    }
}
