using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Text;

namespace tsp_shared
{
    public class Reader
    {
        private const string COORDS_MARK = "NODE_COORD_SECTION";
        private const string END_MARK = "EOF";
        
        public static List<Node> GetNodesFromFile(string path)
        {
            var nodes = new List<Node>();

            string[] lines = File.ReadAllLines(path);
            var isInCorrectSection = false;

            foreach (var line in lines)
            {
                if (line == COORDS_MARK)
                {
                    isInCorrectSection = true;
                    continue;
                }

                if (line == END_MARK)
                {
                    break;
                }

                if (isInCorrectSection)
                {
                    var numbers = line.Split(' ');
                    var number = int.Parse(numbers[0]);
                    var x = float.Parse(numbers[1], CultureInfo.InvariantCulture);
                    var y = float.Parse(numbers[2], CultureInfo.InvariantCulture);
                    var node = new Node(number, new Vector2(x, y));
                    nodes.Add(node);
                }
            }
            
            return nodes;
        }
    }
}
