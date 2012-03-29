using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;

namespace Tower_Defense
{
    public enum ObjectType
    {
        Monster = 0,
        Tower = 1
    }
    public class DrawnObject
    {
        public ObjectType Type;
        public float WorldX, WorldY;
        public Geometry ScreenSprite;
        public int TextureIndex;
        public int Width;
        public int Height;
        public int ViewX { get { return (int)(WorldX * 40); } } // TODO convert world to view world * TileSize
        public int ViewY { get { return (int)(WorldY * 40); ; } }
        public byte ViewZ;
        public bool DeleteMe = false;
        public DrawnObject(float worldX,float worldY, int width = 0,int height = 0)
        {
            //this.TextureIndex = TextureIndex;
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
        if(ScreenSprite != null)
        {  
            if (this.GetType() == typeof(Tower))
                d2dRenderTarget.DrawGeometry(ScreenSprite, GameForm.TowerBrush);
            else
                d2dRenderTarget.DrawGeometry(ScreenSprite, GameForm.MonsterBrush);
        }
            
        }


        public virtual void Update(World world, double curTime)
        {
            
        }
    }

    
}
