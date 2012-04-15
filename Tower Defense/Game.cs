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
using System.Diagnostics;
using System.Threading;
using SharpDX;
namespace Tower_Defense
{
    /// <summary>
    /// Game base class, responsible for handling game stat
    /// </summary>
    public class Game
    {
        public GameState GameState;// // 00 MainMenu, 01 Pause, 02 Ingame
        public World World;
        public const double UpdateInterval = 33; // Milliseconds
        public bool Debug = true;
        GameForm Gameform;
        public int UpdateTime = 0;
        public Game()
        {
            
        }

        /// <summary>
        /// Called By a new Thread
        /// </summary>
        internal void Start(object gf)
        {
            
            Gameform = (GameForm)gf;
            GameState = Tower_Defense.GameState.MainMenu;
            Gameform.MouseClick += Gameform_MouseClick;
            Gameform.KeyUp += Gameform_KeyUp;
            Update();
        }

        void Gameform_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Keys.Add(e);
        }

        private List<System.Windows.Forms.KeyEventArgs> Keys = new List<System.Windows.Forms.KeyEventArgs>();
        private List<System.Windows.Forms.MouseEventArgs> Clicks = new List<System.Windows.Forms.MouseEventArgs>();
        void Gameform_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Clicks.Add(e);
        }

        private void Update()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            Stopwatch TotalTimer = new Stopwatch();
            TotalTimer.Start();
            while (true)
            {
                if (GameState == Tower_Defense.GameState.InGame)
                {
                    World.Update(TotalTimer.Elapsed.TotalMilliseconds);
                    if (Gameform.Buffer.Count < 3)
                        Gameform.Buffer.Enqueue(World.DrawableObjects.ToArray());
                    //Gameform.Buffer.Enqueue(World.DrawableObjects.ToArray());
                    if (World.Player.Lives <= 0)
                    {
                        GameState = Tower_Defense.GameState.MainMenu;
                        World = null;
                    }
                }
                if (GameState == Tower_Defense.GameState.Exit)
                    break;
                
                HandleUserInput();

               

                UpdateTime = (int)(s.Elapsed.Milliseconds);
                if (UpdateTime > UpdateInterval)
                    Gameform.Debugger.Debug("GameLoop took " + UpdateTime + "ms");
                if (s.Elapsed.TotalMilliseconds < UpdateInterval)
                    Thread.Sleep((int)(UpdateInterval - s.Elapsed.Milliseconds));
                s.Restart();
            }
            

        }

        private void HandleUserInput()
        {
            while (Clicks.Count > 0)
            {
                var click = Clicks[0]; Clicks.RemoveAt(0);
                /*if (click.Button != System.Windows.Forms.MouseButtons.Left)
                    continue;*/
                switch(GameState)
                {
                    case Tower_Defense.GameState.MainMenu:
                        handleMenuInput(click);
                        break;
                    case Tower_Defense.GameState.InGamePause:
                        handleMenuInputPauseMenuInput(click);
                        break;
                    case Tower_Defense.GameState.InGame:
                        handleInGameInput(click);
                        break;
                }
            }
            while (Keys.Count > 0)
            {
                var key = Keys[0]; Keys.RemoveAt(0);
                switch (GameState)
                {
                    case Tower_Defense.GameState.InGame:
                        if (key.KeyData == System.Windows.Forms.Keys.Escape)
                            GameState = Tower_Defense.GameState.InGamePause;
                        break;
                    case Tower_Defense.GameState.InGamePause:
                        if (key.KeyData == System.Windows.Forms.Keys.Escape)
                            GameState = Tower_Defense.GameState.InGame;
                        break;
                    case Tower_Defense.GameState.MainMenu:
                        if (key.KeyData == System.Windows.Forms.Keys.Escape)
                            GameState = Tower_Defense.GameState.Exit;
                        break;
                
                }
            }
        }

        private void handleInGameInput(System.Windows.Forms.MouseEventArgs click)
        {
            if(!Contains(Gameform.ViewPort,click.Location))
                return;
            foreach (var o in this.World.DrawableObjects)
                if (Contains(o.ScreenSprite.GetBounds(), click.Location))
                    return;

            foreach (var m in this.World.Map.Map)
                if (Contains(m.ScreenSprite, click.Location))
                {
                    m.Type = Level.MapTileType.TowerHere;
                    if(click.Button == System.Windows.Forms.MouseButtons.Left)
                    World.DrawableObjects.Add(new Towers.BasicTower(m.WorldX, m.WorldY));
                    else if(click.Button == System.Windows.Forms.MouseButtons.Right)
                        World.DrawableObjects.Add(new Towers.SlowingTower(m.WorldX, m.WorldY));
                }
            return;
            var x = click.X - this.Gameform.ViewPort.Left;
            var y = click.Y - this.Gameform.ViewPort.Top;
            x = x / this.World.Map.Width;
            y = y / this.World.Map.Height;
            var total = (((click.X - this.Gameform.ViewPort.Left) * this.World.Map.Tilesize) / this.World.Map.Width) + ((click.Y - this.Gameform.ViewPort.Top) / this.World.Map.Tilesize);
            total = x + (y * this.World.Map.Height);
            var tile = World.Map.Map[total];
            if (tile.Type == Level.MapTileType.EmptyTile)
            {
                World.Map.Map[total].Type = Level.MapTileType.TowerHere;
                World.DrawableObjects.Add(new Towers.BasicTower(tile.WorldX, tile.WorldY));
            }
            
        }

        private bool Contains(System.Drawing.Rectangle rect, System.Drawing.Point point)
        {
            if (rect.Top < point.Y && rect.Bottom > point.Y && rect.Left < point.X && rect.Right > point.X)
                return true;
            return false;
        }

        private void handleMenuInputPauseMenuInput(System.Windows.Forms.MouseEventArgs click)
        {
            if (Contains(MainMenu.Buttons[0].button, click.Location))
            {
                // continue
                
                this.GameState = Tower_Defense.GameState.InGame;
            }
            if (Contains(MainMenu.Buttons[1].button, click.Location))
            {
                // exit
                this.GameState = Tower_Defense.GameState.MainMenu;
            }
        }

        private void handleMenuInput(System.Windows.Forms.MouseEventArgs click)
        {
               if (Contains(MainMenu.Buttons[0].button, click.Location))
               {
                    // new game
                   this.World = new World(Gameform);
                   this.GameState = Tower_Defense.GameState.InGame;
               }
               if (Contains(MainMenu.Buttons[1].button, click.Location))
               {
                   // exit
                   this.GameState = Tower_Defense.GameState.Exit;
               }
        }

        private bool Contains(RectangleF rect, System.Drawing.Point point)
        {
            if (rect.Top < point.Y && rect.Bottom > point.Y && rect.Left < point.X && rect.Right > point.X)
                return true;
            return false;
        }


    }



    public enum GameState
    {
        MainMenu = 0x00,
        InGamePause = 0x01,
        InGame = 0x02,
        EndGame = 0x03,
        Exit = 0x04
    
    }
}
