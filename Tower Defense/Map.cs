using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Algorithms;
namespace Tower_Defense
{
    public class Map
    {
        public MapTile[] Sprites;
        public int Width, Height;
        public static int TileSize = 40;
        public int Tilesize { get { return TileSize; } }
        byte[,] grid;
        public System.Drawing.Point Start;
        public System.Drawing.Point Dest;
        private List<PathFinderNode> _path;
        public List<PathFinderNode> Path { get { if (_path == null) { _path = buildPath(); } return _path.ToList(); } }
        public Map()
        {
            Width = 32; Height = 32;
            Sprites = BuildDummyMap();
            grid = BuildNavMesh();
        }


        private List<PathFinderNode> buildPath()
        {
            var pather = new PathFinderFast(grid);
            pather.Diagonals = false;
            var path = pather.FindPath(Start, Dest);
            if (path == null)
                return new List<PathFinderNode>();
            return path;        
        }


        private byte[,] BuildNavMesh()
        {
            var grid = new byte[Width, Height];
            for (int i = 0; i < Sprites.Length; i++)
                grid[i / Height, i % Width] = (byte)(Sprites[i].Type & MapTileType.Path);
            return grid;
        }
        /// <summary>
        /// Replace with load from file/ random gen
        /// </summary>
        /// <returns></returns>
        public MapTile[] BuildDummyMap()
        {
            Start = new System.Drawing.Point(5, 0);
            Dest = new System.Drawing.Point(6, 31);
            MapTile[] tiles = new MapTile[Width*Height];
            int XX = 5;
            int i = 0;
            bool turn = false;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int xx = x * TileSize;
                    int yy = y * TileSize;
                    tiles[i] = new MapTile(x, y, MapTileType.EmptyTile);
                    if (x == XX || x == 5)
                    {
                        if (x == 5 && turn)
                        {
                            i++;
                            continue;
                        }
                        if (x == 5 && y == 11 && !turn) 
                            turn = true;
                            
                        if (x == 6 && y < 10)
                        {
                            i++;
                            continue;
                        }
                        tiles[i] = new MapTile(x, y, MapTileType.Path);

                        if(y== 0)
                            tiles[i] = new MapTile(x, y, MapTileType.Start);
                        if (y == Height - 1)
                            tiles[i] = new MapTile(x, y, MapTileType.Dest);
                        if (y == 10 && XX == 5) { XX++; }
                            
                    }
                        
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
                    default:
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
            TowerHere = 2,
            Path = 1,
            Start = 5,
            Dest = 3
        }


    }




}
