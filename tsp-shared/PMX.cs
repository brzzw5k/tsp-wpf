using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsp_shared
{
    public class Transformations
    {
        public static Cycle PMXFirstStep(Cycle cycleA, Cycle cycleB)
        {
            var random = new Random();
            var resultList = new List<Vertex>(new Vertex[cycleA.Length]);
            var resultCycle = new Cycle(resultList);

            var randomLength = random.Next(1, cycleA.Length-1);
            var randomPosition = random.Next(0, cycleA.Length - randomLength);

            for (int i = 0; i < randomLength; i++)
            {
                resultList[randomPosition + i] = cycleA.GetVertexAt(randomPosition + i);
            }

            return resultCycle;
        }

        public static Cycle PMXSecondStep(Cycle cycleA, Cycle cycleB, Cycle midCycle)
        {
            for (int i = 0; i < cycleB.Length; i++)
            {
                var vertex = cycleB.GetVertexAt(i);

                if(midCycle.ContainsVertex(vertex))
                {
                    continue;
                }
                midCycle.SetNextEmptyVertex(vertex);
            }

            return midCycle;
        }

        public static Cycle PMXMutate(Cycle cycleA, Cycle cycleB)
        {
            var step1 = PMXFirstStep(cycleA, cycleB);
            var step2 = PMXSecondStep(cycleA, cycleB, step1);

            return step2;
        }

        public static Cycle ThreeOpt(Cycle cycle)
        {
            var random = new Random();
            var randomPosition = random.Next(0, cycle.Length - 3);

            var vertex1 = cycle.GetVertexAt(randomPosition);
            var vertex2 = cycle.GetVertexAt(randomPosition + 1);
            var vertex3 = cycle.GetVertexAt(randomPosition + 2);

            var newCycle = new Cycle(cycle.Vertexes);

            newCycle.Vertexes[randomPosition] = vertex2;
            newCycle.Vertexes[randomPosition + 1] = vertex3;
            newCycle.Vertexes[randomPosition + 2] = vertex1;

            return newCycle;
        }
    }
}
