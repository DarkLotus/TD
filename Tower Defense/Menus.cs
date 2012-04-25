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
    public class MainMenu
    {
        public static Button[] Buttons = new Button[2];
        static MainMenu()
        {
            Buttons[0] = new Button("New Game", 0, 3);
            Buttons[1] = new Button("Exit", 0, 2);
        }
        internal static void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[51], 1f, BitmapInterpolationMode.Linear);
            foreach (var b in Buttons)
                b.Draw(gf);
        }
    }

    public static class PauseMenu
    {
        public static Button[] Buttons = new Button[2];
        static PauseMenu()
        {
            Buttons[0] = new Button("Continue", 400, 400);
            Buttons[1] = new Button("Exit", 400, 460);
        }
        internal static void Draw(GameForm gf)
        {
            foreach (var b in Buttons)
                b.Draw(gf);
        }
    }

    public static class LevelSelect
    {
        public static Button[] Buttons = new Button[1];
        static LevelSelect()
        {
            //TODO Parse list of levels, add buttons for each level.
            Buttons[0] = new Button("DemoLevel", 0, 3);
        }
        internal static void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[51], 1f, BitmapInterpolationMode.Linear);
            foreach (var b in Buttons)
                b.Draw(gf);
        }
    }

   
 
    public static class BuildMenu
    {
        public static int X, Y;
        public static Button[] Buttons = new Button[4];
        static BuildMenu()
        {

        }
        public static void initMenu(int x,int y,GameForm gf)
        {
            var height = (int)((gf.Height - 75) / 4.2);
            var width = (int)(gf.Width - (gf.Width / 1.3));

            X = x; Y = y;
            Buttons[0] = new TowerBuildButton(typeof(Towers.BasicTower),"Basic Tower", 10,8,"800ms",X, Y,width,height);
            Buttons[1] = new TowerBuildButton(typeof(Towers.CannonTower),"Cannon Tower", 25, 25, "2000ms", X, Y + height, width, height);
            Buttons[2] = new TowerBuildButton(typeof(Towers.SlowingTower),"Slowing Tower", 50, 3, "1500ms", X, Y + height * 2, width, height);
            Buttons[3] = new TowerBuildButton(typeof(Towers.LightTower),"Lightning Tower", 50, 6, "2000ms", X, Y + height * 3, width, height);
        }

        internal static void Draw(GameForm gf)
        {
            foreach (var b in Buttons)
                b.Draw(gf);
        }
    }

    public static class UpgradeMenu
    {
        public static int X, Y;
        public static Button[] Buttons = new Button[1];
        private static RectangleF Location;
        public static Tower Tower;
        static UpgradeMenu()
        {
            
        }
        internal static void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[50], Location, 1f, BitmapInterpolationMode.Linear);
            foreach (var b in Buttons)
                b.Draw(gf);
            gf.d2dRenderTarget.DrawText("Level" + Tower.Level, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(Location.Left + 15, Location.Top + 25, Location.Left + 100, Location.Top + 70), GameForm.solidColorBrush);
        }

        internal static void Update(Tower tower)
        {
            Location = new RectangleF(tower.ViewX + 50, tower.ViewY, tower.ViewX + 150, tower.ViewY + 150);
            Buttons[0] = new Button("Upgrade", (int)(Location.Left + 10), (int)(Location.Top + 50),50,25) { fontsize = 10f};
            Tower = tower;
        }
    }
    public class TowerBuildButton : Button
    {
        public string name { get; set; }

        public int cost { get; set; }

        public int dmg { get; set; }
        public Type towerType;
        public string firerate { get; set; }
        /*public TowerBuildButton(int x, int y, int width, int height)
            : base("", x, y, width, height)
        { }*/

        public TowerBuildButton(Type towerType,string name, int cost, int dmg, string firerate, int x, int y, int width, int height)
            : base("", x, y, width, height)
        {
            // TODO: Complete member initialization
            this.towerType = towerType;
            this.name = name;
            this.cost = cost;
            this.dmg = dmg;
            this.firerate = firerate;

        }
        public override void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[50], this.button, 1f, BitmapInterpolationMode.Linear);
            GameForm.solidColorBrush.Color = Colors.Red;
            gf.d2dRenderTarget.DrawText(name, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 15.0f), new RectangleF(this.button.Left + 15, this.button.Top + 10, this.button.Left + 100, this.button.Top + 20), GameForm.solidColorBrush);
            gf.d2dRenderTarget.DrawText("$" + cost, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(this.button.Left + 15, this.button.Top + 50, this.button.Left + 100, this.button.Top + 70), GameForm.solidColorBrush);
            gf.d2dRenderTarget.DrawText("Damage: " + dmg, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(this.button.Left + 15 + 75, this.button.Top + 50, this.button.Left + 200, this.button.Top + 70), GameForm.solidColorBrush);
            gf.d2dRenderTarget.DrawText("FireRate: " + firerate, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(this.button.Left + 15, this.button.Top + 80, this.button.Left + 100, this.button.Top + 100), GameForm.solidColorBrush);
        }

    }


    public class Button
    {
        public string Text;
        int ScreenX, ScreenY;
        public RectangleF button;
        int Width = 0;
        public float fontsize = 20f;
        public RectangleF textbox { get 
        {
            if (box.Left == 0)
                box = new RectangleF(ScreenX + (Width * 0.1f), ScreenY + (Height * 0.3f), ScreenX + Width, ScreenY + 60);
            return box;
        
        } }
        private RectangleF box;
        public Button(string Text,int X, int Y) //:base(50,0,0,200,60)
        {
            this.Text = Text;
            this.ScreenX = X;
            this.ScreenY = Y;
            Width = 200;
            Height = 80;
            this.button = new RectangleF(X, Y, X + 200, Y + 80);
        }
        public Button(string Text, int X, int Y,int width,int height)
            //: base(50, 0, 0, width, height)
        {
            this.Text = Text;
            this.ScreenX = X;
            this.ScreenY = Y;
            Width = width;
            Height = height;
            this.button = new RectangleF(X, Y, X + width, Y + height);
        }

        public virtual void Draw(GameForm gf)
        {
            //GameForm.solidColorBrush.Color = Colors.Blue;
            //gf.d2dRenderTarget.DrawRectangle(button, GameForm.solidColorBrush);
            if (ScreenX == 0)
            {
                ScreenX = (gf.Width / 2) - 150;
                ScreenY = (int)(gf.Height / 1.5) - (ScreenY * 80);
                this.button = new RectangleF(ScreenX, ScreenY, ScreenX + 320, ScreenY + 80);
            }
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[50], this.button, 1f, BitmapInterpolationMode.Linear);
            GameForm.solidColorBrush.Color = Colors.Red;
            gf.d2dRenderTarget.DrawText(Text, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", fontsize), textbox, GameForm.solidColorBrush);      
        }

        public int Height { get; set; }

       
    }
}
