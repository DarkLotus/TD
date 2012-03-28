using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Tower_Defense
{
    public class Map
    {
        public MapTile[] Sprites;
        public const int TileSize = 40;
        public Map()
        {
            Sprites = BuildDummyMap();
        }
        /// <summary>
        /// Replace with load from file/ random gen
        /// </summary>
        /// <returns></returns>
        public MapTile[] BuildDummyMap()
        {
            MapTile[] tiles = new MapTile[40*40];
            int i = 0;
            for (int x = 0; x < 40; x++)
            {
                for (int y = 0; y < 40; y++)
                {
                    int xx = x * TileSize;
                    int yy = y * TileSize;
                    tiles[i] = new MapTile(x, y, MapTileType.EmptyTile);
                    if(x == 5)
                        tiles[i] = new MapTile(x, y, MapTileType.Path);
                    i++;
                }
            }
            return tiles;
        
        }
        public class MapTile
        {
            public int WorldX,WorldY;
            public RectangleF ScreenSprite;
            public MapTileType Type;
            public int ScreenX { get { return WorldX * TileSize; } }
            public int ScreenY { get { return WorldY * TileSize; } }
            public MapTile(int worldx,int worldy,MapTileType type)
            {
                this.WorldX = worldx;
                this.WorldY = worldy;
                this.Type = type;
                this.ScreenSprite = new RectangleF(worldx * TileSize,worldy* TileSize,worldx * TileSize + TileSize,worldy* TileSize + TileSize);
            }


            internal void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget)
            {
                switch (Type)
                {
                    case MapTileType.EmptyTile:
                        d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        break;
                    case MapTileType.Path:
                        var rect = new RectangleF(ScreenX + 0.2f, ScreenY + 0.2f, ScreenX + TileSize - 0.2f, ScreenY + TileSize - 0.2f);
                        d2dRenderTarget.FillRectangle(rect, new SharpDX.Direct2D1.SolidColorBrush(d2dRenderTarget,Colors.Red));
                        d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        break;
                      
                }
            }
        }

        public enum MapTileType
        {
            EmptyTile = 0,
            Path = 1,
            Start = 2,
            Dest = 3
        }


    }




}
