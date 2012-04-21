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
        internal Level Map;
        internal float _size 
        { 
            get 
            {
                float x = _hits / _baseHits; // = .50
                return (Width *x); 
            } 
        }
        AnimatedTexture tex;
        public Monster(short TextureIndex,Level map, int width = 0, int height = 0)
            : base(TextureIndex,map.Start.X + (float)(Helper.random.NextDouble()), map.Start.Y, width, height)
        {
            
            Type = ObjectType.Monster;
            path = map.Path;
            Map = map;
        }
        double _lastMove = 0;
        double _slowEffect = 0;
        byte framenum = 0;
        public int MoveDelay = 30;


        public virtual Monster Clone()
        { return new Monster(TextureIndex,Map, Width, Height); }

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
                _lastMove = curTime + MoveDelay;
                if (_slowEffect != 0)
                    _lastMove += MoveDelay;
            }
            else if(path.Count == 0)
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
            if (curTime > nextanim)
            {
                framenum++;
                if (framenum >= (Direction + 1) * 19)
                    framenum = (byte)(Direction * 19);
                nextanim = curTime + 50;
            }
            
            base.Update(world,curTime);
        }
        double nextanim = 0;
        public override void Draw(GameForm gf)
        {

            gf.MonsterModels[TextureIndex].SetVisibleFrame(framenum);
            gf.d2dRenderTarget.DrawBitmap(gf.MonsterModels[TextureIndex].Texture, this.ScreenSprite, 1f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, gf.MonsterModels[TextureIndex].DrawRegion);
            //base.Draw(gf);
           
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
            //_velModifier = SlowVal;
            this.color.Blue += 50;
        }
        internal byte Direction = 1;
        private void Move(float _velocity)
        {
            var nextpath = path.Last();
            if (this.WorldX < nextpath.X)
            {
                WorldX += _velocity;
                Direction = 2;
            }
            else if (this.WorldX > nextpath.X + 1)
            {
                WorldX -= _velocity;
                Direction = 1;
            }
            else if (this.WorldY < nextpath.Y)
            {
                WorldY += _velocity;
                Direction = 0;
            }
            else if (this.WorldY > nextpath.Y + 1)
                WorldY -= _velocity;
            if (GetDistance(WorldX, WorldY, nextpath.X, nextpath.Y) < 0.5f)
                path.RemoveAt(path.Count - 1);
            else if(new System.Drawing.Rectangle(nextpath.X,nextpath.Y,1,1).Contains((int)WorldX,(int)WorldY))
                path.RemoveAt(path.Count - 1);

           

            this.ScreenSprite = new SharpDX.RectangleF(ViewX, ViewY, ViewX + Width, ViewY + Height);
        }
        private float GetDistance(float x, float y, float destx, float desty)
        {
            float X = x - destx;
            float Y = y - desty;
            return ((X * X) + (Y * Y));
        }

    }
}
