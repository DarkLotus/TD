using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Defense
{
    class Monster : DrawnObject
    {
        public Monster(int TextureIndex, int worldX, int worldY, int width = 0, int height = 0)
            : base(TextureIndex, worldX, worldY)
        { }
    }
}
