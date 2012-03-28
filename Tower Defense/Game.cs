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
        public const double UpdateInterval = 50; // Milliseconds
        public bool Debug = true;
        GameForm Gameform;
        public Game()
        {
            
        }

        /// <summary>
        /// Called By a new Thread
        /// </summary>
        internal void Start(object gf)
        {
            //World = new Tower_Defense.World();
            Gameform = (GameForm)gf;
            GameState = Tower_Defense.GameState.MainMenu;
            Gameform.MouseClick += Gameform_MouseClick;
            Update();
        }
        private List<System.Windows.Forms.MouseEventArgs> Clicks = new List<System.Windows.Forms.MouseEventArgs>();
        void Gameform_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Clicks.Add(e);
        }

        private void Update()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            while (true)
            {
                if (GameState == Tower_Defense.GameState.InGame)
                    World.Update();
                if (GameState == Tower_Defense.GameState.Exit)
                    break;
                HandleUserInput();

                if (s.Elapsed.TotalMilliseconds < UpdateInterval)
                    Thread.Sleep((int)(UpdateInterval - s.Elapsed.TotalMilliseconds));
            
            }
            

        }

        private void HandleUserInput()
        {
            while (Clicks.Count > 0)
            {
                var click = Clicks[0]; Clicks.RemoveAt(0);
                if (click.Button != System.Windows.Forms.MouseButtons.Left)
                    continue;
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
        }

        private void handleInGameInput(System.Windows.Forms.MouseEventArgs click)
        {
            throw new NotImplementedException();
        }

        private void handleMenuInputPauseMenuInput(System.Windows.Forms.MouseEventArgs click)
        {
            throw new NotImplementedException();
        }

        private void handleMenuInput(System.Windows.Forms.MouseEventArgs click)
        {
               if (Contains(MainMenu.Buttons[0].button, click.Location))
               {
                    // new game
                   this.World = new World();
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
