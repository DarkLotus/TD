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

        List<DrawnObject> MobsToSpawn = new List<DrawnObject>();
        internal Objects.ParticleManager ParticleMan = new Objects.ParticleManager();
        /// <summary>
        /// Called when New Game is clicked
        /// </summary>
        public World(GameForm gf)
        {
            Player = new Tower_Defense.Player();
            Gameform = gf;
            Map = new Level();
            MobsToSpawn.Add(new Monsters.Runner(gf.d2dFactory, Map));

        }

        double spawntimer = 0;

        public void Update(double curTime)
        {
            if (curTime > spawntimer)
            {
                DrawableObjects.Add(new Monsters.Runner(Gameform.d2dFactory, Map));
                spawntimer = curTime + 250;
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
