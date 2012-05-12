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
        public GameState GameState;
        public World World;
        private Menu _menu;
        public Menu Menu { get {
            switch (GameState)
            { 
                case GameState.MainMenu:
                    if (_menu == null || _menu.GetType() != typeof(MainMenu)) { _menu = new MainMenu(); }
                    return _menu;
                case GameState.InGamePause:
                    if (_menu == null || _menu.GetType() != typeof(PauseMenu)) { _menu = new PauseMenu(); }
                    return _menu;
                case GameState.LevelSelect:
                    if (_menu == null || _menu.GetType() != typeof(LevelSelectMenu)) { _menu = new LevelSelectMenu(); }
                    return _menu;
                case GameState.About:
                    if (_menu == null || _menu.GetType() != typeof(AboutMenu)) { _menu = new AboutMenu(); }
                    return _menu;
                case GameState.EndGame:
                    if (_menu == null || _menu.GetType() != typeof(ScoreMenu)) { _menu = new ScoreMenu(); }
                    return _menu;
                default:
                    return null;
            }
        } }
        public const double UpdateInterval = 16; // Milliseconds
        
        public bool Debug = true;
        GameForm Gameform;
        public int UpdateTime = 0;
        private List<System.Windows.Forms.KeyEventArgs> Keys = new List<System.Windows.Forms.KeyEventArgs>();
        private List<System.Windows.Forms.MouseEventArgs> Clicks = new List<System.Windows.Forms.MouseEventArgs>();
        Tower TowerToBuild = null;
        public Game()
        {
            //ScoreMenu.InitScoreMenu(new Player() { Score = Helper.random.Next(10000), Name = "Lotus" + Helper.random.Next(100) },"Test".GetHashCode());

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
            Gameform.KeyDown += Gameform_KeyDown;

            //Menu = new MainMenu();
            Update();
        }
        System.Windows.Forms.KeyEventArgs _curKey;
        void Gameform_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            _curKey = e;
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
                        ScoreMenu.InitScoreMenu(this.World.Player,this.World.Map.Name.GetHashCode());
                        GameState = Tower_Defense.GameState.EndGame;
                        World = null;
                    }
                }
                if (GameState == Tower_Defense.GameState.Exit)
                    break;
                
                HandleUserInput();
                if (_curKey != null)
                {
                    var x = _curKey.KeyCode;
                    HandleIngameKey(x);
                }
               

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
            _curKey = null;
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
                    case Tower_Defense.GameState.InGame:
                        handleInGameInput(click);
                        break;
                    default:
                        handleMenuInput(click);
                        break;
                }
            }
            while (Keys.Count > 0)
            {
                var key = Keys[0]; Keys.RemoveAt(0);
                switch (GameState)
                {
                    case Tower_Defense.GameState.InGame:
                        HandleIngameKey(key.KeyCode);
                        
                        break;
                    case Tower_Defense.GameState.InGamePause:
                        if (key.KeyData == System.Windows.Forms.Keys.Escape)
                            GameState = Tower_Defense.GameState.InGame;
                        break;
                    case Tower_Defense.GameState.MainMenu:
                        if (key.KeyData == System.Windows.Forms.Keys.Escape)
                            GameState = Tower_Defense.GameState.Exit;
                        break;
                    case Tower_Defense.GameState.About:
                        if (key.KeyData == System.Windows.Forms.Keys.Escape)
                            GameState = Tower_Defense.GameState.MainMenu;
                        break;
                    case Tower_Defense.GameState.EndGame:
                        if (key.KeyData == System.Windows.Forms.Keys.Escape)
                            GameState = Tower_Defense.GameState.MainMenu;
                        break;

                }
            }
        }


        private void HandleIngameKey(System.Windows.Forms.Keys x)
        {
            if (x == System.Windows.Forms.Keys.Escape)
            {
                if (GameState == GameState.InGame)
                {
                    GameState = Tower_Defense.GameState.InGamePause;
                    //this.Menu = new PauseMenu();
                }
                else { GameState = Tower_Defense.GameState.InGame; }
            }
            if (x == System.Windows.Forms.Keys.Right)
                GameForm._drawXoffset += 2;
            if (x == System.Windows.Forms.Keys.Left)
                GameForm._drawXoffset -= 2;
            if (x == System.Windows.Forms.Keys.Up)
                GameForm._drawYoffset += 2;
            if (x == System.Windows.Forms.Keys.Down)
                GameForm._drawYoffset -= 2;
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
                        //this.Menu = new PauseMenu();
                        return;
                    }
                    if (z.Text == "Next Wave")
                    {
                        this.World.NextWave();
                        return;
                    }
                    if (z.Text == "Exit")
                    {
                        GameState = Tower_Defense.GameState.MainMenu;
                        //this.Menu = new MainMenu();
                        World = null;
                        return;
                    }
                    if (z.Text == "Speed")
                    {
                        if (Helper.GameSpeed == 1)
                            Helper.GameSpeed = 2;
                        else
                            Helper.GameSpeed = 1;
                        return;
                    }
                }
            }
            if (this.World != null)
            {
                if (this.World.ShowUpgradeMenu)
                {
                    if (Helper.Contains(UpgradeMenu.Buttons[0].button, click.Location))
                    {
                        if (World.Player.Gold > UpgradeMenu.Tower.UpgradeCost)
                        {
                            World.Player.Gold -= UpgradeMenu.Tower.UpgradeCost;
                            UpgradeMenu.Tower.LevelUP();
                        }                       
                        this.World.ShowUpgradeMenu = false;
                        return;
                    }
                    else
                    {
                        this.World.ShowUpgradeMenu = false;
                        return;
                    }
                }
                foreach (TowerBuildButton o in World.BuildMenu.Buttons)
                {
                    if (Helper.Contains(o.button, click.Location))
                    {
                        TowerToBuild = (Tower)Assembly.GetAssembly(o.towerType).CreateInstance(o.towerType.FullName);
                        return;
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

            foreach (var m in this.World.Map.Map)
                if (Helper.Contains(m.ScreenSprite, click.Location))
                {
                    if (TowerToBuild != null)
                        BuildTower(m);
                }
            return;
            }
        }

        private void BuildTower(Level.MapTile m)
        {
            if (World.Player.Gold < TowerToBuild.Cost || m.Type != Level.MapTileType.EmptyTile)
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

        

        private void handleMenuInput(System.Windows.Forms.MouseEventArgs click)
        {
            for (int i = 0; i < Menu.Buttons.Count(); i++)
            {
                if (Helper.Contains(Menu.Buttons[i].button, click.Location))
                {
                    if (GameState == GameState.MainMenu)
                    {
                        switch (i)
                        {
                            case 0:
                                this.GameState = Tower_Defense.GameState.LevelSelect;
                            //this.Menu = new LevelSelectMenu();
                            return;
                            case 1:
                                 this.GameState = Tower_Defense.GameState.About;
                            //this.Menu = new AboutMenu();
                            return;
                            case 2:
                                 this.GameState = Tower_Defense.GameState.Exit;
                            return;
                        }
                    }
                    if (GameState == GameState.LevelSelect)
                    {
                        this.World = new World(Gameform, Menu.Buttons[i].Text);
                        this.GameState = Tower_Defense.GameState.InGame;
                        return;
                    }
                    if (GameState == GameState.About)
                    {
                        this.GameState = Tower_Defense.GameState.MainMenu;
                        //this.Menu = new MainMenu();
                        return;
                    }
                    if (GameState == GameState.EndGame)
                    {
                        this.GameState = Tower_Defense.GameState.MainMenu;
                        //this.Menu = new MainMenu();
                        return;
                    }
                    if (GameState == GameState.InGamePause)
                    {
                        if (i == 0)
                        {
                            this.GameState = Tower_Defense.GameState.InGame;
                        }
                        else if (i == 1)
                        {
                            GameState = Tower_Defense.GameState.MainMenu;
                            //this.Menu = new MainMenu();
                            World = null;
                            return;
                        }
                        return;
                    }

                }
            }
       
        }

        #endregion

        internal void Draw(GameForm gameForm)
        {
            switch (GameState)
            {
                case GameState.InGame:
                    if(World != null)
                        World.Draw(gameForm); // Draw static map/UI
                    break;
                /*case GameState.MainMenu:
                    Menu.Draw(gameForm);
                    //MainMenu.Draw(this);
                    break;
                case GameState.About:
                    Menu.Draw(gameForm);
                    break;
                case GameState.LevelSelect:
                    Menu.Draw(gameForm);
                    break;
                case GameState.EndGame:
                    Menu.Draw(gameForm);
                    break;
                case GameState.InGamePause:
                    World.Draw(gameForm); // Draw static map/UI
                    Menu.Draw(gameForm);
                    break;*/

            }
            if (Menu != null)
                Menu.Draw(gameForm);
        }
    }



   
}
