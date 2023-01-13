using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsp_shared
{
    [Serializable]
    public class Cycle
    {
        public List<Vertex> Vertexes { get; set; }

        public int Length { get => Vertexes.Count; }

        public Cycle(List<Vertex> vertexes)
        {
            Vertexes = vertexes;
        }

        public override string ToString()
        {
            var concated = "";

            foreach (var vertex in Vertexes)
            {
                if(vertex == null)
                {
                    concated += $"  ";
                }
                else
                {
                    concated += $"{vertex.Number} ";
                }
                
            }

            return concated;
        }

        public Vertex GetVertexAt(int position)
        {
            return Vertexes[position];
        }

        private bool ContainsVertexWithNumber(int number)
        {
            return Vertexes.Any(vertex => vertex != null && vertex.Number == number);
        }

        public bool ContainsVertex(Vertex vertex)
        {
            return ContainsVertexWithNumber(vertex.Number);
        }

        public void SetNextEmptyVertex(Vertex vertex)
        {
            for (int i = 0; i < Vertexes.Count; i++)
            {
                if (Vertexes[i] == null)
                {
                    Vertexes[i] = vertex;
                    return;
                }
            }
        }

        public double CalculateTotalDistance()
        {
            double distance = 0;

            for(int i=0; i < Vertexes.Count; i++)
            {
                var current = Vertexes[i];
                var next = Vertexes[(i + 1) % Vertexes.Count];
                
                
                distance += current.Point.Distance(next.Point);
                
            }

            return distance;
        }

        public Cycle GetShuffledCopy()
        {
            var vertexesCopy = new List<Vertex>(Vertexes);
            vertexesCopy.Shuffle();

            return new Cycle(vertexesCopy);
        }

        public Cycle GetCopy()
        {
            var vertexesCopy = new List<Vertex>(Vertexes);

            return new Cycle(vertexesCopy);
        }

        public double GetMinX()
        {
            return Vertexes.Min(vertex => vertex.Point.X);
        }

        public double GetMaxX()
        {
            return Vertexes.Max(vertex => vertex.Point.X);
        }

        public double GetMinY()
        {
            return Vertexes.Min(vertex => vertex.Point.Y);
        }

        public double GetMaxY()
        {
            return Vertexes.Max(vertex => vertex.Point.Y);
        }
    }
}
