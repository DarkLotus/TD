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
            Buttons[0] = new Button("New Game", 400, 400);
            Buttons[1] = new Button("Exit", 400, 460);
        }
        internal static void Draw(GameForm gf)
        {
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
            Buttons[0] = new Button("DemoLevel", 400, 400);
        }
        internal static void Draw(GameForm gf)
        {
            foreach (var b in Buttons)
                b.Draw(gf);
        }
    }

    public class Button : DrawnObject
    {
        public string Text;
        int ScreenX, ScreenY;
        public RectangleF button;
        public Button(string Text,int X, int Y) :base(50,0,0,200,60)
        {
            this.Text = Text;
            this.ScreenX = X;
            this.ScreenY = Y;
            this.button = new RectangleF(X, Y, X + 200, Y + 60);
        }


        public override void Draw(GameForm gf)
        {
            GameForm.solidColorBrush.Color = Colors.Blue;
            gf.d2dRenderTarget.DrawRectangle(button, GameForm.solidColorBrush);
            GameForm.solidColorBrush.Color = Colors.Red;
            gf.d2dRenderTarget.DrawText(Text, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 20.0f), button, GameForm.solidColorBrush);      
        }
    }
}
