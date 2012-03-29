using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Defense
{
    internal class Monster : DrawnObject
    {
        internal float _velocity;
        internal float _hits;
        List<Algorithms.PathFinderNode> path;
        public Monster(Map map, int width = 0, int height = 0)
            : base(map.Start.X + (float)(Helper.random.NextDouble()), map.Start.Y, width, height)
        {
            Type = ObjectType.Monster;
            path = map.Path;
        }

        public override void Update(World world, double curTime)
        {
            if (DeleteMe)
                return;
            if (this._hits <= 0)
                this.DeleteMe = true;
            if (path.Count > 0)
            {
                Move();
            }
            else
                this.DeleteMe = true;
            base.Update(world,curTime);
        }

        public void DoDamage(float damage)
        {
            _hits -= damage;
        }
        private void Move()
        {
            var nextpath = path.Last();
            if (this.WorldX < nextpath.X)
             WorldX += _velocity;
            else if (this.WorldX > nextpath.X + 1)
             WorldX -= _velocity;
            else if (this.WorldY < nextpath.Y)
             WorldY += _velocity;
            else if (this.WorldY > nextpath.Y + 1)
             WorldY -= _velocity;
            if (GetDistance(WorldX, WorldY, nextpath.X, nextpath.Y) < 0.5f)
                path.RemoveAt(path.Count - 1);
            else if(new System.Drawing.Rectangle(nextpath.X,nextpath.Y,1,1).Contains((int)WorldX,(int)WorldY))
                path.RemoveAt(path.Count - 1);
        }
        private float GetDistance(float x, float y, float destx, float desty)
        {
            float X = x - destx;
            float Y = y - desty;
            return ((X * X) + (Y * Y));
        }

    }
}
