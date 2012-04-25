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
using Algorithms;
using System.Xml;
using System.IO;
using Tower_Defense.Maps;

namespace Tower_Defense
{


    public class Level
    {
        public MapTile[] Map;
        public int Width, Height;
        public static int TileSize = 40;
        public int Tilesize { get { return TileSize; } }
        public System.Drawing.Point Start;
        public System.Drawing.Point Dest;
        public List<PathFinderNode> Path { get { if (_path == null) { _path = buildPath(); } return _path.ToList(); } }

        internal Queue<Queue<Monster>> Waves = new Queue<Queue<Monster>>();
        
        byte[,] grid;
        private List<PathFinderNode> _path;

        //TODO Level(string levelname)
        // Load all variables from Level file.
        public Level(string levelname)
        {
            Width = 16; Height = 16;
            Map = LoadMap(levelname + ".tmx");
            grid = BuildNavMesh();
            Waves = LoadWaves(levelname + ".xml");
        }

        private Queue<Queue<Monster>> LoadWaves(string path)
        {
            Maps.LevelDef d = new LevelDef();
            //d.populate();
            var st = File.Open("Maps\\" + path,FileMode.Open);
            //ProtoBuf.Serializer.SerializeWithLengthPrefix<LevelDef>(st, d, ProtoBuf.PrefixStyle.Base128);
            //st.Close();
            //st = File.Open("Maps\\" + path, FileMode.Open);
            d = ProtoBuf.Serializer.DeserializeWithLengthPrefix<LevelDef>(st, ProtoBuf.PrefixStyle.Base128);
            st.Close();
            Queue<Queue<Monster>> waves = new Queue<Queue<Monster>>();
            foreach (var w in d.Waves)
            {
                waves.Enqueue(makeawave(w.makeMob(this,w.MobLevel), w.NumberOfMobs));
            }
            return waves;
        }


        private Queue<Monster> makeawave(Monster type,int amount)
        {

            Queue<Monster> mobs = new Queue<Monster>();
            for (int i = 0; i < amount; i++)
                mobs.Enqueue(type.Clone());
            return mobs;
        }



        public MapTile[] LoadMap(string MapName)
        {
            string mapdata = "";
            MapTile[] tiles = new MapTile[Width * Height];
            var a = File.Open("Maps\\" + MapName, FileMode.Open);
            XmlReader xr = XmlReader.Create(a);
            while (xr.Read())
            {
            if(xr.Name == "data")
            {
                xr.Read();
                mapdata = xr.Value.ToString();
                break;
            }
            
            }
            string[] values = mapdata.Split(new string[] {","},StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Contains("\n"))
                    values[i] = values[i].Replace("\n", "");
                var tiletype = Convert.ToInt16(values[i]);
                tiletype--;
                if (tiletype < 10)
                    tiles[i] = new MapTile(i % Width, i / Height, (MapTileType)tiletype);
                if((MapTileType)tiletype == MapTileType.Start)
                    this.Start = new System.Drawing.Point(i % Width, i / Height);
                if ((MapTileType)tiletype == MapTileType.Dest)
                    this.Dest = new System.Drawing.Point(i % Width, i / Height);
            }
            a.Close();
            
            return tiles;
            
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
            for (int i = 0; i < Map.Length; i++)
                grid[i % Height, i / Width] = (byte)(Map[i].Type & MapTileType.Path);
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
            MapTile[] tiles = new MapTile[Width * Height];
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

                        if (y == 0)
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
            public int ViewX { get { return (WorldX * TileSize) + GameForm.ViewPort.Left;  } }
            public int ViewY { get { return (WorldY * TileSize) + GameForm.ViewPort.Top ; } }
            public DrawingPoint ViewCentre { get { return new DrawingPoint(ViewX + TileSize / 2, ViewY + TileSize / 2); } }
            public MapTile(int worldx,int worldy,MapTileType type)
            {
                this.WorldX = worldx;
                this.WorldY = worldy;
                this.Type = type;
                this.ScreenSprite = new RectangleF(ViewX, ViewY, ViewX + TileSize, ViewY + TileSize);
            }


            internal void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget)
            {
                switch (Type)
                {
                    case MapTileType.EmptyTile:
                        //d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        break;
                    case MapTileType.TowerHere:
                        d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        break;
                    default:
                        var rect = new RectangleF(ViewX + 0.2f, ViewY + 0.2f, ViewX + TileSize - 0.2f, ViewY + TileSize - 0.2f);
                        d2dRenderTarget.FillRectangle(rect, new SharpDX.Direct2D1.SolidColorBrush(d2dRenderTarget,Colors.Red));
                        d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        break;
                      
                }
            }

            internal void Draw(GameForm gameForm)
            {
                switch (Type)
                {
                    case MapTileType.EmptyTile:
                        //d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        gameForm.d2dRenderTarget.DrawBitmap(gameForm.MapTiles[0],ScreenSprite, 0.8f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
                        break;
                    case MapTileType.TowerHere:
                        //d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        gameForm.d2dRenderTarget.DrawBitmap(gameForm.MapTiles[0], ScreenSprite, 0.8f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
                        break;
                    default:
                        gameForm.d2dRenderTarget.DrawBitmap(gameForm.MapTiles[1], ScreenSprite, 0.8f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
                        //var rect = new RectangleF(ViewX + 0.2f, ViewY + 0.2f, ViewX + TileSize - 0.2f, ViewY + TileSize - 0.2f);
                        //d2dRenderTarget.FillRectangle(rect, new SharpDX.Direct2D1.SolidColorBrush(d2dRenderTarget, Colors.Red));
                        //d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
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
