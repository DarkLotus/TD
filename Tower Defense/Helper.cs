using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Defense
{
    public static class Helper
    {
        public static float GetDistance(float x1, float y1, float x2, float y2)
        {
            double part1 = Math.Pow((x2 - x1), 2);
            //Take y2-y1, then sqaure it
            double part2 = Math.Pow((y2 - y1), 2);
            //Add both of the parts together
            double underRadical = part1 + part2;
            //Get the square root of the parts
            var result =(int)Math.Sqrt(underRadical);
            return result;
          
        }
        public static Random random = new Random();

        public static float RandomFloat(float max)
        {
            Random random = new Random();
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }
    }
}
