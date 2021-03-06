﻿/*
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
using SharpDX.Windows;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Device1 = SharpDX.Direct3D11.Device;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using SharpDX.DXGI;
using SharpDX.Direct3D11;

namespace Tower_Defense
{
    public class GameForm : SharpDX.Windows.RenderForm
    {

        public static Brush EmptyTileBrush;
        public static SolidColorBrush solidColorBrush;
        public static SolidColorBrush MonsterBrush;
        public static SolidColorBrush TowerBrush;
        public static int _drawXoffset = 0;
        public static int _drawYoffset = 0;
        public static System.Drawing.Rectangle ViewPort;

        public Game Game { get; set; }

        public Dictionary<short, Bitmap> MapTiles = new Dictionary<short, Bitmap>();
        public Dictionary<short, AnimatedTexture> MonsterModels = new Dictionary<short, AnimatedTexture>();
        public RenderTarget d2dRenderTarget;
        public SharpDX.Direct2D1.Factory d2dFactory;
        public SharpDX.DirectWrite.Factory fontFactory;

        static Ionic.Zip.ZipFile Data;
        Stopwatch stopwatch = new Stopwatch();
        int fps = 0;

        public GameForm(Game g)
            : base("Project Majestic")
        {
            // TODO: Complete member initialization
            this.Game = g;
            //this.Show2();
        }
        /// <summary>
        /// Setup DirectX Renderer.
        /// </summary>
        private void SetupDX()
        {
            d2dFactory = new SharpDX.Direct2D1.Factory();
            fontFactory = new SharpDX.DirectWrite.Factory();

            HwndRenderTargetProperties properties = new HwndRenderTargetProperties();
            properties.Hwnd = this.Handle;
            properties.PixelSize = new System.Drawing.Size(this.Width, this.Height);
            properties.PresentOptions = PresentOptions.None;

            d2dRenderTarget = new WindowRenderTarget(d2dFactory, new RenderTargetProperties(new PixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Ignore)), properties);
            d2dRenderTarget.AntialiasMode = AntialiasMode.Aliased;
            d2dRenderTarget.TextAntialiasMode = TextAntialiasMode.Cleartype;

            EmptyTileBrush = new SolidColorBrush(d2dRenderTarget, Colors.Aquamarine);
            solidColorBrush = new SolidColorBrush(d2dRenderTarget, Colors.White);
            MonsterBrush = new SolidColorBrush(d2dRenderTarget, Colors.Wheat);
            TowerBrush = new SolidColorBrush(d2dRenderTarget, Colors.Purple);
        }
   
        /// <summary>
        /// Load Textures into memory.
        /// </summary>
        private void LoadTexs()
        {
            //try{
            Data = Ionic.Zip.ZipFile.Read("Art\\art.dat");
            MapTiles.Add(0, LoadFromFile(d2dRenderTarget, GetFile("Grass.jpg")));
            MapTiles.Add(1, LoadFromFile(d2dRenderTarget, GetFile("Soil.jpg")));
            MapTiles.Add(100, LoadFromFile(d2dRenderTarget, GetFile("tower_basic.png")));
            MapTiles.Add(101, LoadFromFile(d2dRenderTarget, GetFile("tower_slow.png")));
            MapTiles.Add(102, LoadFromFile(d2dRenderTarget, GetFile("tower_light.png")));
            MapTiles.Add(103, LoadFromFile(d2dRenderTarget, GetFile("tower_cannon.png")));

            MapTiles.Add(51, LoadFromFile(d2dRenderTarget, GetFile("bg.png")));
            MapTiles.Add(50, LoadFromFile(d2dRenderTarget, GetFile("blue_button.png")));
            //Animated textures must be PNG.
            MonsterModels.Add(0, new AnimatedTexture("impnew", 128, 128, 60, d2dRenderTarget));
            MonsterModels.Add(1, new AnimatedTexture("spider_128", 128, 128, 60, d2dRenderTarget));
            MonsterModels.Add(2, new AnimatedTexture("bird_128", 128, 128, 60, d2dRenderTarget));
            Data.Dispose();
            Data = null;
            //} catch(Exception e) {MessageBox.Show(e.Message);}
        }
  
        /// <summary>
        /// Extract texture from compressed data
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static MemoryStream GetFile(string path)
        {

            MemoryStream ms = new MemoryStream();
            Data[path].Extract(ms);
            ms.Position = 0;
            return ms;

        }

        /// <summary>
        /// Render Loop
        /// </summary>
        public void callback()
        {

            if (Game.GameState == GameState.Exit)
                this.Close();
                d2dRenderTarget.BeginDraw();
                d2dRenderTarget.Clear(Colors.Black);
                Game.Draw(this);
                /*switch (Game.GameState)
                {
                    case GameState.InGame:
                        Game.World.Draw(this); // Draw static map/UI
                                            break;
                    case GameState.MainMenu:
                                            Menu.Draw(this);
                        //MainMenu.Draw(this);
                        break;
                    case GameState.About:
                        AboutMenu.Draw(this);
                        break;
                    case GameState.LevelSelect:
                        LevelSelectMenu.Draw(this);
                        break;
                    case GameState.EndGame:
                        ScoreMenu.Draw(this);
                        break;
                    case GameState.InGamePause:
                        Game.World.Draw(this); // Draw static map/UI
                        PauseMenu.Draw(this);
                        break;

                }*/

                //frame++;
                if (Game.Debug)
                    d2dRenderTarget.DrawText("FPS: " + fps + "UPS:" + Game.UpdateTime, new SharpDX.DirectWrite.TextFormat(fontFactory, "Arial", 15.0f), new SharpDX.RectangleF(0, 0, 500, 25), solidColorBrush);
                //if (Game.Debug)
                //    d2dRenderTarget.DrawText("MX " + mousex + " MY " + mousey, new SharpDX.DirectWrite.TextFormat(fontFactory, "Arial", 15.0f), new RectangleF(0, 25, 500, 225), solidColorBrush);
                d2dRenderTarget.EndDraw();
         

            //swapChain.Present(1, PresentFlags.None);
                //stopwatch.Stop();
            //if (stopwatch.Elapsed.Ticks < 16600000)
            //    Thread.Sleep((int)((16600000 - stopwatch.Elapsed.Ticks) / 1000000));
            //Thread.Sleep(4);
            fps = (int)(1000 / stopwatch.Elapsed.TotalMilliseconds);
            stopwatch.Restart();
        }

       
        public void Init()
        {
            //Debugger.Show();
            this.Size = new System.Drawing.Size(1440, 900);
            ViewPort = new System.Drawing.Rectangle(-40, 50, (int)(this.Width / 1.31), this.Size.Height);
            ;

            SetupDX();
            LoadTexs();
            //myblock = new DrawingStateBlock(d2dFactory); stopwatch.Start();
            RenderLoop.Run(this, new RenderLoop.RenderCallback(callback));
            // Release all resources
            //renderView.Dispose();
            //backBuffer.Dispose();

            //device.Dispose();
            //swapChain.Dispose();
            //factory.Dispose();
        }

       
        /// <summary>
        /// Loads a Direct2D Bitmap from a file using System.Drawing.Image.FromFile(...)
        /// </summary>
        /// <param name="renderTarget">The render target.</param>
        /// <param name="file">The file.</param>
        /// <returns>A D2D1 Bitmap</returns>
        public static Bitmap LoadFromFile(RenderTarget renderTarget, Stream file)
        {
            // Loads from file using System.Drawing.Image
            using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(file))
            {
                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied));
                var size = new System.Drawing.Size(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            //int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            int rgba = B | (G << 8) | (R << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }

                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;

                    return new Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);
                }
            }
        }

    }
}
