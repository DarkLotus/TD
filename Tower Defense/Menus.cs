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
        public static Button[] Buttons = new Button[4];
        static BuildMenu()
        {
        
        }
        internal static void Draw(GameForm gf)
        {
            foreach (var b in Buttons)
                b.Draw(gf);
        }
    }

    public static class UpgradeMenu
    {
        public static Button[] Buttons = new Button[1];
        static UpgradeMenu()
        {

        }
        internal static void Draw(GameForm gf)
        {
            foreach (var b in Buttons)
                b.Draw(gf);
        }
    }

    public class Button
    {
        public string Text;
        int ScreenX, ScreenY;
        public RectangleF button;
        int Width = 0;
        public RectangleF textbox { get 
        {
            if (box.Left == 0)
                box = new RectangleF(ScreenX + Width / 5, ScreenY + 20, ScreenX + Width, ScreenY + 80);
            return box;
        
        } }
        private RectangleF box;
        public Button(string Text,int X, int Y) //:base(50,0,0,200,60)
        {
            this.Text = Text;
            this.ScreenX = X;
            this.ScreenY = Y;
            Width = 200;
            this.button = new RectangleF(X, Y, X + 200, Y + 80);
        }
        public Button(string Text, int X, int Y,int width,int height)
            //: base(50, 0, 0, width, height)
        {
            this.Text = Text;
            this.ScreenX = X;
            this.ScreenY = Y;
            Width = width;
            this.button = new RectangleF(X, Y, X + width, Y + height);
        }

        public void Draw(GameForm gf)
        {
            //GameForm.solidColorBrush.Color = Colors.Blue;
            //gf.d2dRenderTarget.DrawRectangle(button, GameForm.solidColorBrush);
            if (ScreenX == 0)
            {
                ScreenX = (gf.Width / 2) - 160;
                ScreenY = (int)(gf.Height / 1.5) - (ScreenY * 80);
                this.button = new RectangleF(ScreenX, ScreenY, ScreenX + 320, ScreenY + 80);
            }
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[50], this.button, 1f, BitmapInterpolationMode.Linear);
            GameForm.solidColorBrush.Color = Colors.Red;
            gf.d2dRenderTarget.DrawText(Text, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 20.0f), textbox, GameForm.solidColorBrush);      
        }
    }
}
