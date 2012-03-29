using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Defense
{
    public class Tower : DrawnObject
    {
        public Tower(int worldX, int worldY, int width = 0, int height = 0)
            : base(worldX, worldY,width,height)
        {
            Type = ObjectType.Tower;

        }
    }
}
