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
        
        /// <summary>
        /// Called when New Game is clicked
        /// </summary>
        public World()
        {
            Map = new Map();
        }

        public void Update()
        {
            List<DrawnObject> _deleteme = new List<DrawnObject>();
                foreach (var o in DrawableObjects)
                {
                    if (o.DeleteMe)
                        _deleteme.Add(o);
                    else
                        o.Update();
                }
                lock (DrawableObjects)
                {
                    foreach (var o in _deleteme)
                        DrawableObjects.Remove(o);
                }               
               
            }
        

    }
}
