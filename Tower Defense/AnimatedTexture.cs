using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using Tower_Defense;
namespace Tower_Defense
{
    public class AnimatedTexture
    {
        //127/95
        public Bitmap Texture;
        public RectangleF DrawRegion;
        // TODO Add load from sprite sheet.
        private short spriteX, spriteY, count;
        int width;
        public AnimatedTexture(string texturename, short spriteX,short spriteY,short count,Device device, RenderTarget d2dRender)
        {
            Texture = GameForm.LoadFromFile(d2dRender,File.OpenRead("art\\" + texturename + ".png"));
            width = (int)Texture.Size.Width / spriteX;
            DrawRegion = new RectangleF(0, 0, spriteX, spriteY);
            this.count = count;
            this.spriteY = spriteY;
            this.spriteX = spriteX;
        }
        public bool SetVisibleFrame(short FrameNum)
        {
            if (FrameNum > this.count)
                return false;
            else
            {
                var x = (FrameNum % width) * spriteX;
                var y = (FrameNum / width) * spriteY;
                this.DrawRegion = new RectangleF(x,y,x+spriteX,y+spriteY);
            }
            return true;
        }

    }
}
