namespace tsp_shared
{
    [Serializable]
    public class Node
    {
        public int Number { get; set; }
        public Vector2 Position { get; set; }

        public Node() {
            Number = -1;
            Position = new Vector2(0, 0);
        }
        public Node(int number, Vector2 position)
        {
            Number = number;
            Position = position;
        }

        public override string ToString()
        {
            return $"Number: {Number}, Position: {Position}";
        }
    }
}
