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
            Buttons[1] = new Button("Exit", 400, 440);
        }
        internal static void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget,SharpDX.Direct2D1.Factory d2dFactory, SharpDX.DirectWrite.Factory fontFactory)
        {
            foreach (var b in Buttons)
                b.Draw(d2dRenderTarget,d2dFactory, fontFactory);
        }
    }

    public static class PauseMenu
    {

        internal static void Draw(SharpDX.Direct2D1.Factory d2dFactory, SharpDX.DirectWrite.Factory fontFactory)
        {

        }
    }

    public class Button
    {
        string Text;
        int ScreenX, ScreenY;
        public RectangleF button;
        public Button(string Text,int X, int Y)
        {
            this.Text = Text;
            this.ScreenX = X;
            this.ScreenY = Y;
            this.button = new RectangleF(X, Y, X + 100, Y + 40);
        }

        internal void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget,SharpDX.Direct2D1.Factory d2dFactory, SharpDX.DirectWrite.Factory fontFactory)
        {
            GameForm.solidColorBrush.Color = Colors.Blue;
            d2dRenderTarget.DrawRectangle(button, GameForm.solidColorBrush);
            GameForm.solidColorBrush.Color = Colors.Red;
            d2dRenderTarget.DrawText(Text, new SharpDX.DirectWrite.TextFormat(fontFactory, "Arial", 20.0f), button,GameForm.solidColorBrush);           
        }
    }
}
