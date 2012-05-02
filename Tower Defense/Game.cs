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
using System.Reflection;
namespace Tower_Defense
{
    public enum GameState
    {
        MainMenu = 0x00,
        InGamePause = 0x01,
        InGame = 0x02,
        EndGame = 0x03,
        Exit = 0x04,
        LevelSelect = 0x05,
        About = 0x06
    }
    /// <summary>
    /// Game base class, responsible for handling game stat
    /// </summary>
    public class Game
    {
        public GameState GameState;// // 00 MainMenu, 01 Pause, 02 Ingame
        public World World;
        public const double UpdateInterval = 16; // Milliseconds
        public bool Debug = true;
        GameForm Gameform;
        public int UpdateTime = 0;
        private List<System.Windows.Forms.KeyEventArgs> Keys = new List<System.Windows.Forms.KeyEventArgs>();
        private List<System.Windows.Forms.MouseEventArgs> Clicks = new List<System.Windows.Forms.MouseEventArgs>();
        Tower TowerToBuild = null;
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
                //if (UpdateTime > UpdateInterval)
                 //   Gameform.Debugger.Debug("GameLoop took " + UpdateTime + "ms");
                if (s.Elapsed.TotalMilliseconds < UpdateInterval)
                    Thread.Sleep((int)(UpdateInterval - s.Elapsed.Milliseconds));
                s.Restart();
            }
            

        }
        #region Input

        void Gameform_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Keys.Add(e);
        }

        void Gameform_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Clicks.Add(e);
        }

        private void HandleUserInput()
        {
            while (Clicks.Count > 0)
            {
                var click = Clicks[0]; Clicks.RemoveAt(0);
                /*if (click.Button != System.Windows.Forms.MouseButtons.Left)
                    continue;*/
                switch (GameState)
                {
                    case Tower_Defense.GameState.MainMenu:
                        handleMenuInput(click);
                        break;
                    case Tower_Defense.GameState.LevelSelect:
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
            foreach (var z in this.World.UIElements)
            {
                if (Helper.Contains(z.button, click.Location))
                {
                    if (z.Text == "Pause Game")
                    {
                        this.GameState = Tower_Defense.GameState.InGamePause;
                        break;
                    }
                    if (z.Text == "Next Wave")
                    {
                        this.World.NextWave();
                        break;
                    }
                    if (z.Text == "Exit")
                    {
                        GameState = Tower_Defense.GameState.MainMenu;
                        World = null;
                        break;
                    }
                }
            }
            if (this.World != null)
            {
                if (this.World.ShowUpgradeMenu)
                {
                    if (Helper.Contains(UpgradeMenu.Buttons[0].button, click.Location))
                    {
                        UpgradeMenu.Tower.LevelUP();
                        this.World.ShowUpgradeMenu = false;
                    }
                    else
                    {
                        this.World.ShowUpgradeMenu = false;
                        return;
                    }
                }
            }
            foreach (TowerBuildButton o in BuildMenu.Buttons)
            {
                if (Helper.Contains(o.button, click.Location))
                {
                    //o.Clicked(this, Gameform);
                    TowerToBuild = (Tower)Assembly.GetAssembly(o.towerType).CreateInstance(o.towerType.FullName);
                }
            }

            if (!Helper.Contains(GameForm.ViewPort, click.Location))
                return;
            foreach (var o in this.World.DrawableObjects)
                if (Helper.Contains(o.ScreenSprite, click.Location))
                {
                    if (o.Type == ObjectType.Tower)
                    {
                        UpgradeMenu.Update((Tower)o);
                        this.World.ShowUpgradeMenu = true;
                    }
                }
            //return;

            foreach (var m in this.World.Map.Map)
                if (Helper.Contains(m.ScreenSprite, click.Location))
                {
                    if (TowerToBuild != null)
                    {
                        if (World.Player.Gold < TowerToBuild.Cost)
                        {
                            TowerToBuild = null;
                            return;
                        }
                        World.Player.Gold -= TowerToBuild.Cost;
                        m.Type = Level.MapTileType.TowerHere;
                        Tower t = TowerToBuild;
                        t.WorldX = m.WorldX;
                        t.WorldY = m.WorldY;
                        World.DrawableObjects.Add(t);
                        TowerToBuild = null;
                    }
                }
            return;
            var x = click.X - GameForm.ViewPort.Left;
            var y = click.Y - GameForm.ViewPort.Top;
            x = x / this.World.Map.Width;
            y = y / this.World.Map.Height;
            var total = (((click.X - GameForm.ViewPort.Left) * this.World.Map.Tilesize) / this.World.Map.Width) + ((click.Y - GameForm.ViewPort.Top) / this.World.Map.Tilesize);
            total = x + (y * this.World.Map.Height);
            var tile = World.Map.Map[total];
            if (tile.Type == Level.MapTileType.EmptyTile)
            {
                World.Map.Map[total].Type = Level.MapTileType.TowerHere;
                World.DrawableObjects.Add(new Towers.BasicTower(tile.WorldX, tile.WorldY));
            }

        }
        private void handleMenuInputPauseMenuInput(System.Windows.Forms.MouseEventArgs click)
        {
            if (Helper.Contains(PauseMenu.Buttons[0].button, click.Location))
            {
                // continue

                this.GameState = Tower_Defense.GameState.InGame;
            }
            if (Helper.Contains(PauseMenu.Buttons[1].button, click.Location))
            {
                // exit
                this.GameState = Tower_Defense.GameState.MainMenu;
            }
        }

        private void handleMenuInput(System.Windows.Forms.MouseEventArgs click)
        {
            switch (GameState)
            {
                case Tower_Defense.GameState.MainMenu:
                    if (Helper.Contains(MainMenu.Buttons[0].button, click.Location))
                    {
                        // new game
                        //this.World = new World(Gameform);
                        this.GameState = Tower_Defense.GameState.LevelSelect;
                    }
                    if (Helper.Contains(MainMenu.Buttons[2].button, click.Location))
                    {
                        // exit
                        this.GameState = Tower_Defense.GameState.Exit;
                    }
                    if (Helper.Contains(MainMenu.Buttons[1].button, click.Location))
                    {
                        // exit
                        this.GameState = Tower_Defense.GameState.About;
                    }
                    break;
                case Tower_Defense.GameState.LevelSelect:
                    foreach (var b in LevelSelect.Buttons)
                    {
                        if (Helper.Contains(b.button, click.Location))
                        {
                            this.World = new World(Gameform, b.Text);
                            this.GameState = Tower_Defense.GameState.InGame;
                        }
                    }
                    break;
                case Tower_Defense.GameState.About:
                    if (Helper.Contains(MainMenu.Buttons[0].button, click.Location))
                        this.GameState = Tower_Defense.GameState.MainMenu;
                    break;
            }

        }

        #endregion
       

        



        
    }



   
}
