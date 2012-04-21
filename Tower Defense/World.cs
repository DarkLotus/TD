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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tower_Defense
{
    public class World
    {
        public List<DrawnObject> DrawableObjects = new List<DrawnObject>();
        public Level Map;
        public Player Player;
        public GameForm Gameform;
        // need a player object
        public int Lives = 20;

        //List<DrawnObject> MobsToSpawn = new List<DrawnObject>();
        internal Objects.ParticleManager ParticleMan = new Objects.ParticleManager();
        /// <summary>
        /// Called when New Game is clicked
        /// </summary>
        public World(GameForm gf,string mapname)
        {
            Player = new Tower_Defense.Player();
            Gameform = gf;
            Map = new Level(mapname);
            //MobsToSpawn.Add(new Monsters.Runner(gf.d2dFactory, Map));

        }

        double spawntimer = 0;

        double initalTimeBetweenMobs = 500;
        Queue<Monster> CurrentWave = new Queue<Monster>();
        public int Wave = 0;
        public int MobsRemaining { get { return CurrentWave.Count; } }
        public void Update(double curTime)
        {
            if (curTime > spawntimer)
            {
                //DrawableObjects.Add(new Monsters.Runner(Gameform.d2dFactory, Map));
                if (CurrentWave.Count == 0)
                {
                    if (Map.Waves.Count > 0)
                    {
                        CurrentWave = Map.Waves.Dequeue();
                        Wave++;
                    }
                }
                if(CurrentWave.Count > 0)
                DrawableObjects.Add(CurrentWave.Dequeue());
                spawntimer = curTime + initalTimeBetweenMobs + Helper.random.Next(500);

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
               
            }
        

    }
}
