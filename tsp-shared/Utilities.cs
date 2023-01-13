using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsp_shared
{
    public static class Utilities
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static double StrangeClamp(this double number, double rangeAMin, double rangeAMax, double rangeBMin, double rangeBMax)
        {
            double rangeA = rangeAMax - rangeAMin;
            double rangeB = rangeBMax - rangeBMin;

            double value = (number - rangeAMin) / rangeA;

            return rangeBMin + (value * rangeB);
        }
    }
}
