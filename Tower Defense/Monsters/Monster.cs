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

namespace Tower_Defense
{
    internal class Monster : DrawnObject
    {
        internal float _velocity { get { return this._baseVelocity - _velModifier; } }
        internal float _velModifier;
        internal float _baseVelocity;
        internal float _hits;
        internal float _baseHits;
        internal int ScoreValue;
        List<Algorithms.PathFinderNode> path;
        
        internal float _size { get { return Width - ((_baseHits - _hits)/2); } }
        public Monster(Level map, int width = 0, int height = 0)
            : base(map.Start.X + (float)(Helper.random.NextDouble()), map.Start.Y, width, height)
        {
            Type = ObjectType.Monster;
            path = map.Path;
        }
        double _lastMove = 0;
        double _slowEffect = 0;
        public override void Update(World world, double curTime)
        {
            if (DeleteMe)
                return;
            if (this._hits <= 0f) 
            {
                world.ParticleMan.CreateExplosion(ViewX, ViewY);
                world.Player.Score += this.ScoreValue;
                this.DeleteMe = true; 
            }

            if (path.Count > 0 && curTime > _lastMove)
            {
                Move(_velocity);
                _lastMove = curTime + 30;
            }
            else
            {
                world.Player.Lives--;
                this.DeleteMe = true;
            }
            if (_slowEffect < curTime && _slowEffect != 0)
            {
                this.color.Blue -= 50;
                this._velModifier = 0;
                this._slowEffect = 0;
            }
                
            base.Update(world,curTime);
        }

        public void DoDamage(float damage)
        {
            _hits -= damage;
              
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SlowVal"> FP value to slow unit down by</param>
        /// <param name="DurationMS">Curtime+wanted delay</param>
        public void SlowMe(float SlowVal,double DurationMS)
        {
            _slowEffect = DurationMS;
            _velModifier = SlowVal;
            this.color.Blue += 50;
        }
        private void Move(float _velocity)
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
