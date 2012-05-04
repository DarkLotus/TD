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
using Amazon;
using ProtoBuf;
using System.Net.Sockets;
namespace Tower_Defense
{
    public class MainMenu
    {
        public static Button[] Buttons = new Button[3];
        static MainMenu()
        {
            Buttons[0] = new Button("New Game", 0, 4);
            Buttons[1] = new Button("About", 0, 3);
            Buttons[2] = new Button("Exit", 0, 2);
        }
        internal static void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[51], 1f, BitmapInterpolationMode.Linear);
            foreach (var b in Buttons)
                b.Draw(gf);
        }
    }

    [ProtoContract]
    public class SubmitScore
    {
        [ProtoMember(1)]
        public string Name {get;set;}
        [ProtoMember(2)]
        public Int64 Score {get;set;}


    }
    [ProtoContract]
    public class HighScores
    {
        [ProtoMember(1)]
        public List<SubmitScore> TopTen = new List<SubmitScore>();


    }
    public class ScoreMenu
    {
        public static Button[] Buttons = new Button[1];
        private static string[] strings = new string[] { "Code by James Kidd 2012", "Thanks to SharpDX an Open Source DX Wrapper", "Special thanks to CGTextures.com for their amazing Textures." };

        static ScoreMenu()
        {
            Buttons[0] = new Button("Main Menu", 0, -2);
        }
        public static void InitScoreMenu(Player p)
        {
            SubmitScore s = new SubmitScore() { Name = p.Name, Score = p.Score };
            try
            {
                TcpClient server = new TcpClient("ec2-107-20-58-57.compute-1.amazonaws.com", 2593);
                Serializer.SerializeWithLengthPrefix<SubmitScore>(server.GetStream(), s, PrefixStyle.Base128);
                server.GetStream().Flush();
                var scores = Serializer.DeserializeWithLengthPrefix<HighScores>(server.GetStream(), PrefixStyle.Base128);
                strings = new string[scores.TopTen.Count];
                int i = 0;
                bool topten = false;
                foreach (var str in scores.TopTen)
                {
                    if (s.Name == str.Name)
                        topten = true;
                    strings[i++] = str.Name + " " + str.Score;
                }
                if (!topten)
                    strings[i++] = s.Name + " " + s.Score;
                server.Close();
            }
            catch {
                strings = new string[] { "LoadFailed", s.Name + " " + s.Score };
            }
            
            /*string accesskey = "";
            string secretkey = "";
            
            Amazon.SimpleDB.AmazonSimpleDB sdb = AWSClientFactory.CreateAmazonSimpleDBClient(accesskey, secretkey);
            Amazon.SimpleDB.Model.CreateDomainRequest cdr = new Amazon.SimpleDB.Model.CreateDomainRequest();
            cdr.DomainName = "High_SCORE_DOMAIN";
            sdb.CreateDomain(cdr);*/
            
        }
        internal static void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[51], 1f, BitmapInterpolationMode.Linear);
            foreach (var b in Buttons)
                b.Draw(gf);
            for (int i = 0; i < strings.Count(); i++)
            {
                gf.d2dRenderTarget.DrawText(strings[i], new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 20.0f), new RectangleF(gf.Width * 0.2f, (gf.Height * 0.3f) + (i * 22), gf.Width * 0.8f, gf.Height * 0.9f), GameForm.solidColorBrush);
            }

        }
    }

    public class AboutMenu
    {
        public static Button[] Buttons = new Button[1];
        private static string[] about = new string[] {"Code by James Kidd 2012","Thanks to SharpDX an Open Source DX Wrapper","Special thanks to CGTextures.com for their amazing Textures."};
        
        static AboutMenu()
        {
            Buttons[0] = new Button("Main Menu", 0, -2);
        }
        internal static void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[51], 1f, BitmapInterpolationMode.Linear);
            foreach (var b in Buttons)
                b.Draw(gf);
            for (int i = 0; i < about.Count(); i++)
            {
                gf.d2dRenderTarget.DrawText(about[i], new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 20.0f), new RectangleF(gf.Width * 0.2f, (gf.Height * 0.3f) + (i*22), gf.Width * 0.8f, gf.Height * 0.9f), GameForm.solidColorBrush);
            }
                
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

    public static class LevelSelectMenu
    {
        public static Button[] Buttons = new Button[1];
        static LevelSelectMenu()
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
            var height = (int)((gf.Height - 75) / 5);
            var width = (int)(gf.Width - (gf.Width / 1.3));

            X = x; Y = y;
            Buttons[0] = new TowerBuildButton(typeof(Towers.BasicTower),TowerStats.Basic.Name,TowerStats.Basic.Price,TowerStats.Basic.BaseDamage,TowerStats.Basic.BaseFireRateMS,X, Y,width,height);
            Buttons[1] = new TowerBuildButton(typeof(Towers.CannonTower), TowerStats.Cannon.Name, TowerStats.Cannon.Price, TowerStats.Cannon.BaseDamage, TowerStats.Cannon.BaseFireRateMS, X, Y + height, width, height);
            Buttons[2] = new TowerBuildButton(typeof(Towers.SlowingTower), TowerStats.Slow.Name, TowerStats.Slow.Price, TowerStats.Slow.BaseDamage, TowerStats.Slow.BaseFireRateMS, X, Y + height * 2, width, height);
            Buttons[3] = new TowerBuildButton(typeof(Towers.LightTower), TowerStats.Light.Name, TowerStats.Light.Price, TowerStats.Light.BaseDamage, TowerStats.Light.BaseFireRateMS, X, Y + height * 3, width, height);
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
            gf.d2dRenderTarget.DrawText("Cost" + (UpgradeMenu.Tower.Cost * (UpgradeMenu.Tower.Level + 1)), new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(Location.Left + 15, Location.Top + 40, Location.Left + 100, Location.Top + 70), GameForm.solidColorBrush);
            gf.d2dRenderTarget.DrawText("Dmg:" + Tower._damage + "Rng" + Tower._range, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(Location.Left + 15, Location.Top + 55, Location.Left + 100, Location.Top + 70), GameForm.solidColorBrush);
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
        public int firerate { get; set; }
        /*public TowerBuildButton(int x, int y, int width, int height)
            : base("", x, y, width, height)
        { }*/

        public TowerBuildButton(Type towerType,string name, int cost, float dmg, int firerate, int x, int y, int width, int height)
            : base("", x, y, width, height)
        {
            // TODO: Complete member initialization
            this.towerType = towerType;
            this.name = name;
            this.cost = cost;
            this.dmg = (int)dmg;
            this.firerate = firerate;

        }
        public override void Draw(GameForm gf)
        {
            gf.d2dRenderTarget.DrawBitmap(gf.MapTiles[50], this.button, 1f, BitmapInterpolationMode.Linear);
            GameForm.solidColorBrush.Color = Colors.Red;
            gf.d2dRenderTarget.DrawText(name, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 15.0f), new RectangleF(this.button.Left + 15, this.button.Top + 10, this.button.Left + 100, this.button.Top + 20), GameForm.solidColorBrush);
            gf.d2dRenderTarget.DrawText("$" + cost, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(this.button.Left + 15, this.button.Top + 50, this.button.Left + 100, this.button.Top + 70), GameForm.solidColorBrush);
            gf.d2dRenderTarget.DrawText("Damage: " + dmg, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(this.button.Left + 15 + 75, this.button.Top + 50, this.button.Left + 200, this.button.Top + 70), GameForm.solidColorBrush);
            gf.d2dRenderTarget.DrawText("FireRate: " + firerate + "ms", new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 10.0f), new RectangleF(this.button.Left + 15, this.button.Top + 80, this.button.Left + 100, this.button.Top + 100), GameForm.solidColorBrush);
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
