using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace tsp_shared
{
    public class Node
    {
        public int Number { get; set; }
        public Vector2 Position { get; set; }

        public Node(int number, Vector2 position)
        {
            Number = number;
            Position = position;
        }
    }
}
