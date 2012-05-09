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
using SharpDX;
using Algorithms;
using System.Xml;
using System.IO;
using Tower_Defense.Maps;

namespace Tower_Defense
{


    public class Level
    {
        public string Name;
        public MapTile[] Map;
        public int Width, Height;
        public static int TileSize = Helper.TowerSize;
        public int Tilesize { get { return TileSize; } }
        //public System.Drawing.Point Start;
        //public System.Drawing.Point Dest;

        public List<System.Drawing.Point> Starts = new List<System.Drawing.Point>();
        public List<System.Drawing.Point> Dests = new List<System.Drawing.Point>();
        //public List<PathFinderNode> Path { get { if (_path == null) { _path = buildPath(); } return buildPath(); return _path.ToList(); } }

        internal Queue<Queue<Monster>> Waves = new Queue<Queue<Monster>>();
        
        byte[,] grid;
        private List<PathFinderNode> _path;

        //TODO Level(string levelname)
        // Load all variables from Level file.
        public Level(string levelname)
        {
            Name = levelname;

            Map = LoadMap(levelname + ".tmx");
            grid = BuildNavMesh();
            //Waves = LoadWaves(levelname + ".xml");
        }
        public Type[] Types = new Type[] { typeof(Tower_Defense.Monsters.Runner), typeof(Tower_Defense.Monsters.Flyer), typeof(Tower_Defense.Monsters.Tank) };
        internal Queue<Monster> MakeWave(int Wave)
        {
            var mob = Types[Helper.random.Next(2)];
            Monster m = (Monster)mob.Assembly.CreateInstance(mob.FullName);
            m.initMob(this);
            m.SetLevel(Wave + 1);
            return makeawave(m,20);
        }



        private Queue<Queue<Monster>> LoadWaves(string path)
        {
            Maps.LevelDef d = new LevelDef();
            var st = File.Open("Maps\\" + path,FileMode.Open);
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
            var a = File.Open("Maps\\" + MapName, FileMode.Open);
            XmlReader xr = XmlReader.Create(a);
            while (xr.Read())
            {
                if (xr.Name == "layer")
                {
                    Width = Convert.ToInt32(xr.GetAttribute("width"));
                    Height = Convert.ToInt32(xr.GetAttribute("height"));
                }
            if(xr.Name == "data")
            {
                mapdata = xr.ReadElementContentAsString();
                //xr.Read();
                //mapdata = xr.Value.ToString();
                break;
            }
            
            }
            string[] values = mapdata.Split(new string[] {","},StringSplitOptions.RemoveEmptyEntries);
            MapTile[] tiles = new MapTile[Width * Height];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Contains("\n"))
                    values[i] = values[i].Replace("\n", "");
                var tiletype = Convert.ToInt16(values[i]);
                tiletype--;
                    tiles[i] = new MapTile(i % Width, i / Width, (MapTileType)tiletype);
                if ((MapTileType)tiletype == MapTileType.Start)
                    this.Starts.Add(new System.Drawing.Point(i % Width, i / Width));
                    //this.Start = new System.Drawing.Point(i % Width, i / Height);
                if ((MapTileType)tiletype == MapTileType.Dest)
                    this.Dests.Add(new System.Drawing.Point(i % Width, i / Width));
                    //this.Dest = new System.Drawing.Point(i % Width, i / Height);
            }
            a.Close();
            
            return tiles;
            
        }
        public List<PathFinderNode> buildPath(System.Drawing.Point Start)
        {
            List<PathFinderNode> path;
            var pather = new PathFinderFast(grid);
            pather.Diagonals = false;
            foreach (var dest in this.Dests)
            {
                path = pather.FindPath(Start, dest);
                if (path != null)
                    return path;
            }

            return null;
        }

        bool IsPowerOfTwo(int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        private byte[,] BuildNavMesh()
        {
            var gridsize = Math.Max(Width, Height);
            if (!IsPowerOfTwo(gridsize))
                gridsize = (gridsize | (1));
            var grid = new byte[gridsize, gridsize];
            for (int i = 0; i < Map.Length; i++)
                grid[i % Width, i / Width] = (byte)(Map[i].Type & MapTileType.Path);
            return grid;
        }
        /// <summary>
        /// Replace with load from file/ random gen
        /// </summary>
        /// <returns></returns>
        /*public MapTile[] BuildDummyMap()
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

        }*/
        public class MapTile
        {
            public int WorldX,WorldY;
            public RectangleF ScreenSprite { get { return new RectangleF(ViewX, ViewY, ViewX + TileSize, ViewY + TileSize); } }
            public MapTileType Type;
            public int ViewX { get { return (WorldX * TileSize) + GameForm.ViewPort.Left + GameForm._drawXoffset;  } }
            public int ViewY { get { return (WorldY * TileSize) + GameForm.ViewPort.Top + GameForm._drawYoffset; } }
            public DrawingPoint ViewCentre { get { return new DrawingPoint(ViewX + TileSize / 2, ViewY + TileSize / 2); } }
            public MapTile(int worldx,int worldy,MapTileType type)
            {
                this.WorldX = worldx;
                this.WorldY = worldy;
                this.Type = type;
                //this.ScreenSprite = new RectangleF(ViewX, ViewY, ViewX + TileSize, ViewY + TileSize);
            }


            /*internal void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget)
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
            }*/

            internal void Draw(GameForm gameForm)
            {
                switch (Type)
                {
                    case MapTileType.EmptyTile:
                        //d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        gameForm.d2dRenderTarget.DrawBitmap(gameForm.MapTiles[0],ScreenSprite, 1f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
                        break;
                    case MapTileType.TowerHere:
                        //d2dRenderTarget.DrawRectangle(ScreenSprite, GameForm.EmptyTileBrush);
                        gameForm.d2dRenderTarget.DrawBitmap(gameForm.MapTiles[0], ScreenSprite, 1f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
                        break;
                    default:
                        gameForm.d2dRenderTarget.DrawBitmap(gameForm.MapTiles[1], ScreenSprite, 1f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
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
