using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Defense
{
    public class DrawnObject
    {
        public int WorldX, WorldY;
        public int TextureIndex;
        public int Width;
        public int Height;
        public int ViewX { get { return 0; } } // TODO convert world to view world * TileSize
        public int ViewY { get { return 0; } }
        public byte ViewZ;
        public bool DeleteMe = false;
        public DrawnObject(int TextureIndex,int worldX,int worldY, int width = 0,int height = 0)
        {
            this.TextureIndex = TextureIndex;
            this.WorldX = worldX;
            this.WorldY = worldY;
            if (width == 0 && height == 0)
            {
                this.Width = (int)GameForm.StaticBitmaps[TextureIndex].Size.Width;
                this.Height = (int)GameForm.StaticBitmaps[TextureIndex].Size.Height;
            }
            else { this.Width = width; this.Height = height; }
        }
        public virtual void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget)
        {
        
        }

        public virtual void Update()
        { }
    }

    
}
