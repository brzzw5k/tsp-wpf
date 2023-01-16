using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tsp_shared
{
    [Serializable]
    public class Cycle
    {
        public List<Node> Nodes { get; set; }
        public float MinX { get; set; }
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }

        public Cycle(List<Node> nodes)
        {
            Nodes = nodes;
            CalculateMinMaxXY();
        }

        public Cycle(string path)
        {
            Nodes = Reader.GetNodesFromFile(path);
            CalculateMinMaxXY();
        }

        public void CalculateMinMaxXY()
        {
            if (Nodes.Count == 0)
            {
                return;
            }
            MinX = Nodes.Min(x => x.Position.X);
            MaxX = Nodes.Max(x => x.Position.X);
            MinY = Nodes.Min(x => x.Position.Y);
            MaxY = Nodes.Max(x => x.Position.Y);
        }

        public float GetLength()
        {
            float length = 0;

            for (int i = 0; i < Nodes.Count - 1; i++)
            {
                length += Nodes[i].Position.Distance(Nodes[i+1].Position);
            }

            length += Nodes[^1].Position.Distance(Nodes[0].Position);

            return length;
        }

        public List<Node> GetNormalizedNodes(float width, float height)
        {
            var normalizedNodes = new List<Node>();
            foreach (var node in Nodes)
            {
                var x = (node.Position.X - MinX) / (MaxX - MinX) * width;
                var y = (node.Position.Y - MinY) / (MaxY - MinY) * height;
                var normalizedNode = new Node(node.Number, new Vector2(x, y));
                normalizedNodes.Add(normalizedNode);
            }

            return normalizedNodes;
        }

        internal void SetFirstEmptyNode(Node node)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Number == -1)
                {
                    Nodes[i] = node;
                    return;
                }
            }
        }

        public Cycle ShuffledCopy()
        {
            var random = new Random();
            var shuffledNodes = Nodes.OrderBy(x => random.Next()).ToList();
            return new Cycle(shuffledNodes);
        }

        public Cycle Copy()
        {
            return new Cycle(Nodes);
        }

        public override string ToString()
        {
            return string.Join(" ", Nodes.Select(x => x.Number));
        }
    }
}
