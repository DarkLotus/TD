using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Defense
{
    public class Player
    {
        public int Score = 0;
        public int Lives = 20;
        public int Gold = 50;
        //current score?
        // hold level here?

        public Player()
        {
            Name = "Lotus" + Helper.random.Next(100);
        }




        public string Name { get; set; }
    }
}
