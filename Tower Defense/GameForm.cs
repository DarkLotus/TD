using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device1 = SharpDX.Direct3D11.Device;
using Factory = SharpDX.DXGI.Factory;
using FeatureLevel = SharpDX.Direct3D11.DebugFeatureFlags;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;
using OpenUO.Ultima;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
namespace Tower_Defense
{
    public class GameForm : SharpDX.Windows.RenderForm
    {
        public Game Game { get; set; }
        public static Brush EmptyTileBrush;
        public static SolidColorBrush solidColorBrush;
        public static Dictionary<int, Bitmap> LandBitmaps = new Dictionary<int, Bitmap>();
        public static Dictionary<int, Bitmap> StaticBitmaps = new Dictionary<int, Bitmap>();
        RenderTarget d2dRenderTarget;
        int[,] StaticsToDraw = new int[41, 41];
        int[,] TilesToDraw = new int[41, 41];
        int charposx = 1424, charposy = 2549;
        int _drawXoffset = -200;
        int _drawYoffset = 400;
        SharpDX.Direct2D1.Factory d2dFactory;
        SharpDX.DirectWrite.Factory fontFactory;
        Device1 device;
        SwapChain swapChain;
        Factory factory;
        Texture2D backBuffer;
        RenderTargetView renderView;
        private void SetupDX()
        {
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(this.ClientSize.Width, this.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = this.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput

            };

            // Create Device and SwapChain 
            Device1.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.BgraSupport | DeviceCreationFlags.SingleThreaded, desc, out device, out swapChain);

            d2dFactory = new SharpDX.Direct2D1.Factory();
            fontFactory = new SharpDX.DirectWrite.Factory();
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            var rectangleGeometry = new RoundedRectangleGeometry(d2dFactory, new RoundedRect() { RadiusX = 32, RadiusY = 32, Rect = new SharpDX.RectangleF(128, 128, width - 128, height - 128) });

            // Ignore all windows events
            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(this.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);

            Surface surface = backBuffer.QueryInterface<Surface>();
            d2dRenderTarget = new RenderTarget(d2dFactory, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));

            EmptyTileBrush = new SolidColorBrush(d2dRenderTarget, Colors.Aquamarine);
            solidColorBrush = new SolidColorBrush(d2dRenderTarget, Colors.White);

        }
        public void Show2()
        {
            this.Size = new System.Drawing.Size(1440, 900);
            var viewport = new System.Drawing.Rectangle(0, 50, this.Size.Width - 200, this.Size.Height - 80);
            SetupDX();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Main loop
            var fps = 0;
            var frame = 0;
           // this.KeyDown += myform_KeyDown;
           this.MouseMove += myform_MouseMove;
            int oldx = 0, oldy = 0;
           RenderLoop.Run(this, () =>
            {
                if (Game.GameState == GameState.Exit)
                    this.Close(); 
                d2dRenderTarget.BeginDraw();
                d2dRenderTarget.Clear(Colors.Black);
                switch (Game.GameState)
                {
                    case GameState.InGame:
                        foreach (var x in Game.World.Map.Sprites)
                        {
                            if(Contains(viewport,x))
                                x.Draw(d2dRenderTarget);
                        }
                        foreach (var o in Game.World.DrawableObjects) // TODO THREAD SAFE
                        {
                            if (Contains(viewport, o))
                           o.Draw(d2dRenderTarget);
                        }
                        break;
                    case GameState.MainMenu:
                        MainMenu.Draw(d2dRenderTarget, d2dFactory, fontFactory);
                        break;
                    case GameState.InGamePause:
                        foreach (var x in Game.World.Map.Sprites)
                        {
                           x.Draw(d2dRenderTarget);
                        }
                        foreach (var o in Game.World.DrawableObjects) // TODO THREAD SAFE
                        {
                           o.Draw(d2dRenderTarget);
                        }
                        PauseMenu.Draw(d2dFactory, fontFactory);
                        break;

                }
               if(Game.Debug)           
                d2dRenderTarget.DrawText("FPS: " + fps + "Framecount:" + frame++, new SharpDX.DirectWrite.TextFormat(fontFactory, "Arial", 15.0f), new SharpDX.RectangleF(0, 0, 500, 25), solidColorBrush);
               if (Game.Debug)
                   d2dRenderTarget.DrawText("MX " + mousex + " MY " + mousey, new SharpDX.DirectWrite.TextFormat(fontFactory, "Arial", 15.0f), new RectangleF(0, 25, 500, 225), solidColorBrush);
                
                d2dRenderTarget.EndDraw();
                swapChain.Present(0, PresentFlags.None);
                //if (stopwatch.Elapsed.Ticks < 16600000)
                 //   Thread.Sleep((int)((16600000 - stopwatch.Elapsed.Ticks) / 1000000));
                fps = (int)(1000 / stopwatch.Elapsed.TotalMilliseconds);
                stopwatch.Restart();              
            });

            // Release all resources
            renderView.Dispose();
            backBuffer.Dispose();

            device.Dispose();
            swapChain.Dispose();
            factory.Dispose();
        }

        private bool Contains(System.Drawing.Rectangle rect, DrawnObject point)
        {
            if (rect.Top < point.ViewY && rect.Bottom > point.ViewY + point.Height && rect.Left < point.ViewX && rect.Right > point.ViewX + point.Width)
                return true;
            return false;
        }

        private bool Contains(System.Drawing.Rectangle rect, Map.MapTile point)
        {
            if (rect.Top < point.ScreenSprite.Top && rect.Bottom > point.ScreenSprite.Bottom && rect.Left < point.ScreenSprite.Left && rect.Right > point.ScreenSprite.Right)
                return true;
            return false;
        }



        private int mousex, mousey;
        void myform_MouseMove(object sender, MouseEventArgs e)
        {
            mousex = e.X;
            mousey = e.Y;
        }
        int[,] __tiles = new int[41, 41];
        ArtworkFactory<System.Drawing.Bitmap> texfactory = new OpenUO.Ultima.ArtworkFactory<System.Drawing.Bitmap>(OpenUO.Ultima.InstallationLocator.Locate().First());
        OpenUO.Ultima.Map map = new OpenUO.Ultima.Maps(OpenUO.Ultima.InstallationLocator.Locate().First()).Felucca;
        private Game g;


        private int[,] GetDrawableTiles(int X, int Y)
        {

            MemoryStream st = new MemoryStream();
            int a = 0, m = 0;
            for (int x = X - 20; x <= X + 20; x++)
            {
                for (int y = Y + 20; y >= Y - 20; y--)
                {
                    var tile = map.Tiles.GetLandTile(x, y);
                    if (!LandBitmaps.ContainsKey(tile.Id))
                    {
                        var texture = texfactory.GetLand(tile.Id);
                        texture.MakeTransparent(System.Drawing.Color.Black);
                        texture.Save(st, ImageFormat.Png);
                        LandBitmaps.Add(tile.Id, LoadFromFile(d2dRenderTarget, st));

                    }
                    __tiles[a, m] = tile.Id;

                    st = new MemoryStream();
                    m++;
                }
                m = 0;
                a++;
            }
            return __tiles;
        }


        private int[,] GetDrawableStatics(int X, int Y)
        {
            //ushort[,] __tiles = new ushort[41, 41];
            MemoryStream st = new MemoryStream();
            int a = 0, m = 0;
            for (int x = X - 20; x <= X + 20; x++)
            {
                for (int y = Y + 20; y >= Y - 20; y--)
                {
                    var tile = map.Tiles.GetStaticTiles(x, y);
                    foreach (var t in tile)
                    {
                        if (!StaticBitmaps.ContainsKey(t.Id))
                        {

                            var texture = texfactory.GetStatic(t.Id);
                            texture.MakeTransparent(System.Drawing.Color.Black);
                            texture.Save(st, ImageFormat.Png);
                            StaticBitmaps.Add(t.Id, LoadFromFile(d2dRenderTarget, st));
                            st = new MemoryStream();
                        }
                        __tiles[a, m] = t.Id;
                    }

                    m++;
                }
                m = 0;
                a++;
            }
            return __tiles;
        }
        void myform_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                charposx++;
            }
            if (e.KeyCode == Keys.S)
            {
                charposx--;
            }
            if (e.KeyCode == Keys.D)
            {
                charposy++;
            }
            if (e.KeyCode == Keys.A)
            {
                charposy--;
            }
            if (e.KeyCode == Keys.V)
            {
                _drawXoffset++;
            }
            if (e.KeyCode == Keys.C)
            {
                _drawYoffset++;
            }
        }
        public GameForm()
            : base("test")
        {
            this.Show2();
        }

        public GameForm(Game g)
        {
            // TODO: Complete member initialization
            this.Game = g;
            //this.Show2();
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
                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
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
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
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
