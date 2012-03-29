using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Defense
{
    public static class Helper
    {
        public static float GetDistance(float x, float y, float destx, float desty)
        {
            float X = x - destx;
            float Y = y - desty;
            return ((X * X) + (Y * Y));
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
