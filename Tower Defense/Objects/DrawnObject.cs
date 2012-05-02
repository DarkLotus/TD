/*
 * Copyright (C) 2011 - 2012 James Kidd
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
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
        private float wx, wy;
        public float WorldX { get { return wx; } set { wx = value; screenspr = new RectangleF(ViewX, ViewY, ViewX + Width, ViewY + Height); } }
        public float WorldY { get { return wy; } set { wy = value; screenspr = new RectangleF(ViewX, ViewY, ViewX + Width, ViewY + Height); } }
        public short TextureIndex;
        public int Width;
        public int Height;
        public int ViewX { get { return (int)(WorldX * Level.TileSize) + GameForm.ViewPort.Left; } }
        public int ViewY { get { return (int)(WorldY * Level.TileSize) + GameForm.ViewPort.Top; } }
        public byte ViewZ;
        public bool DeleteMe = false;
        private RectangleF screenspr;
        public RectangleF ScreenSprite { get { return screenspr;} set { screenspr = value; } }
        public DrawnObject(short TextureIndex,float worldX,float worldY, int width = 0,int height = 0)
        {
            this.TextureIndex = TextureIndex;
            this.WorldX = worldX;
            this.WorldY = worldY;
            if (width == 0 && height == 0)
            {
                throw new Exception("width not set");
            }
            else { this.Width = width; this.Height = height; }
            ScreenSprite = new RectangleF(ViewX, ViewY, ViewX + Width, ViewY + Height);
        }
            
        
        public virtual void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[TextureIndex], ScreenSprite, 0.8f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
        }


        public virtual void Update(World world, double curTime)
        {
            
        }
    }

    
}
