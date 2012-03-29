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
        public Map Map;
        public GameForm Gameform;
        // need a player object
        public int Lives = 20;

        List<DrawnObject> MobsToSpawn = new List<DrawnObject>();

        /// <summary>
        /// Called when New Game is clicked
        /// </summary>
        public World(GameForm gf)
        {
            Gameform = gf;
            Map = new Map();
            MobsToSpawn.Add(new Monsters.Runner(gf.d2dFactory, Map));
            MobsToSpawn.Add(new Monsters.Runner(gf.d2dFactory, Map));
            MobsToSpawn.Add(new Monsters.Runner(gf.d2dFactory, Map));
            MobsToSpawn.Add(new Monsters.Runner(gf.d2dFactory, Map));
        }

        double spawntimer = 0;

        public void Update(double curTime)
        {
            if (curTime > spawntimer && MobsToSpawn.Count > 0)
            {
                DrawableObjects.Add(MobsToSpawn.First());
                MobsToSpawn.RemoveAt(0);
                spawntimer = curTime + 1000;
            }
               

            List<DrawnObject> _deleteme = new List<DrawnObject>();
                foreach (var o in DrawableObjects)
                {
                    if (o.DeleteMe)
                        _deleteme.Add(o);
                    else
                        o.Update(this,curTime);
                }
                lock (DrawableObjects)
                {
                    foreach (var o in _deleteme)
                        DrawableObjects.Remove(o);
                }               
               
            }
        

    }
}
