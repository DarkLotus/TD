﻿/*
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX;

namespace Tower_Defense
{
    public class World
    {
        public List<DrawnObject> DrawableObjects = new List<DrawnObject>();
        public List<Button> UIElements = new List<Button>();
        public Level Map;
        public Player Player;
        public GameForm Gameform;
        // need a player object
        double spawntimer = 0;
        public bool ShowUpgradeMenu { get; set; }
        Queue<Monster> CurrentWave = new Queue<Monster>();
        public int Wave = 1;
        public int MobsRemaining { get { return CurrentWave.Count; } }
        //List<DrawnObject> MobsToSpawn = new List<DrawnObject>();
        internal Objects.ParticleManager ParticleMan = new Objects.ParticleManager();
        public BuildMenu BuildMenu;
        /// <summary>
        /// Called when New Game is clicked
        /// </summary>
        public World(GameForm gf,string mapname)
        {
            Player = new Tower_Defense.Player();
            Gameform = gf;
            Map = new Level(mapname);
            UIElements.Add(new Button("Next Wave", gf.Width - 425, 5,150,50));
            UIElements.Add(new Button("Pause Game", gf.Width - 275, 5,150,50));
            UIElements.Add(new Button("Exit", gf.Width - 125, 5, 100, 50));
            UIElements.Add(new Button("Speed", gf.Width - 575, 5, 150, 50));
            BuildMenu = new Tower_Defense.BuildMenu();
            BuildMenu.initMenu((int)(gf.Width / 1.31), 75, gf);
        }




        public void Draw(GameForm gf)
        {
            foreach (var x in Map.Map)
            {
                if (Helper.Contains(GameForm.ViewPort, x.ScreenSprite))
                    x.Draw(gf);
            }
            foreach (var o in DrawableObjects.ToArray()) // FIX ME should be safer
            {
                if (Helper.Contains(GameForm.ViewPort, o.ScreenSprite))
                o.Draw(gf);
            }
            ParticleMan.Draw(gf.d2dRenderTarget);
            foreach (var o in UIElements) 
            {
                    o.Draw(gf);
            }
            //
            BuildMenu.Draw(gf);
            if (ShowUpgradeMenu)
                UpgradeMenu.Draw(gf);
            gf.d2dRenderTarget.DrawText("Score: " + Player.Score + " Gold: " + Player.Gold + " Lives Left: " + Player.Lives + " Wave #" + Wave + "MobsLeft: " + MobsRemaining, new SharpDX.DirectWrite.TextFormat(gf.fontFactory, "Arial", 15.0f), new RectangleF(gf.Width / 2 - 100, 0, gf.Width, 225), GameForm.solidColorBrush);
           
        }
        public void Update(double curTime)
        {
            if (curTime > spawntimer)
            {
                if (CurrentWave.Count > 0)
                {
                    var x = CurrentWave.Dequeue();
                    DrawableObjects.Add(x);
                    spawntimer = curTime + x.SpawnSpacer + Helper.random.Next(500);
                }
               
            }
               

            List<DrawnObject> _deleteme = new List<DrawnObject>();
                foreach (var o in DrawableObjects)
                {
                    if (o.DeleteMe)
                        _deleteme.Add(o);
                    else
                        o.Update(this,curTime);
                }

                    foreach (var o in _deleteme)
                        DrawableObjects.Remove(o);

                ParticleMan.Update(curTime);

                //if (Gameform.Buffer.Count < 3)
                //    Gameform.Buffer.Enqueue(DrawableObjects.ToArray());
            }



        internal void NextWave()
        {
            this.Player.Gold = (int)(Player.Gold * 1.1);
            if (this.MobsRemaining == 0)// && this.Map.Waves.Count > 0)
            {
                CurrentWave = Map.MakeWave(Wave);
                        //CurrentWave = Map.Waves.Dequeue();
                        Wave++;
            }


        }


    }
}
