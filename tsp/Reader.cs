using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using tsp_shared;

namespace tsp
{
    public class Reader
    {
        private const string COORDS_MARK = "NODE_COORD_SECTION";
        private const string END_MARK = "EOF";
        public static List<Vertex> GetVertexesFromFile(string path)
        {
            var vertexes = new List<Vertex>();
            string[] lines = File.ReadAllLines(path);
            var isInCorrectSection = false;

            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if(line == COORDS_MARK)
                {
                    isInCorrectSection = true;
                    continue;
                }

                if(line == END_MARK)
                {
                    break;
                }

                if(isInCorrectSection)
                {
                    var numbers = line.Split(' ');
                    var number = int.Parse(numbers[0]);
                    var x = float.Parse(numbers[1], CultureInfo.InvariantCulture);
                    var y = float.Parse(numbers[2], CultureInfo.InvariantCulture);
                    var vertex = new Vertex(number, new Vector2(x, y));
                    vertexes.Add(vertex);
                }
            }

            return vertexes;
        }
    }
}
