using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsp_shared
{
    public class Transformations
    {

        public static Cycle PMXFirstStep(Cycle cycle)
        {
            var random = new Random();
            var resultNodes = new List<Node>();
            for (int i = 0; i < cycle.Nodes.Count; i++)
            {
                resultNodes.Add(new Node());
            }

            var resultCycle = new Cycle(resultNodes);
            var randomLength = random.Next(1, cycle.Nodes.Count - 1);
            var randomPosition = random.Next(0, cycle.Nodes.Count - randomLength);
                
            for (int i = 0; i < randomLength; i++)
            {
                resultNodes[randomPosition + i] = cycle.Nodes[randomPosition + i];
            }
            
            return resultCycle;

        }

        public static Cycle PMXSecondStep(Cycle cycle, Cycle midCycle)
        {

            foreach (var node in cycle.Nodes)
            {
                if (midCycle.Nodes.Contains(node))
                {
                    continue;
                }
                midCycle.SetFirstEmptyNode(node);
                    
            }
            
            return midCycle;
        }

        public static Cycle PMXMutate(Cycle cycleA, Cycle cycleB)
        {
            var midCycle = PMXFirstStep(cycleA);
            return PMXSecondStep(cycleB, midCycle);

        }

        public static Cycle ThreeOPT(Cycle cycle)
        {
            var random = new Random();
            var randomPosition = random.Next(0, cycle.Nodes.Count - 3);

            var node1 = cycle.Nodes[randomPosition];
            var node2 = cycle.Nodes[randomPosition + 1];
            var node3 = cycle.Nodes[randomPosition + 2];

            var newNodes = new List<Node>(cycle.Nodes);
            newNodes[randomPosition] = node2;
            newNodes[randomPosition + 1] = node3;
            newNodes[randomPosition + 2] = node1;

            return new Cycle(newNodes);
        }
    }
}
