using System;

namespace tsp_shared
{
    [Serializable]
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
