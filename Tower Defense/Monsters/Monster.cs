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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tower_Defense
{
    public abstract class Monster : DrawnObject
    {
        internal abstract int SpawnSpacer { get;}
        internal float _velocity { get { return this._baseVelocity - _velModifier; } }
        internal float _velModifier;
        internal float _baseVelocity;
        internal float _hits;
        internal float _baseHits;
        internal float _baseHitsAfterLevel { get { return _baseHits * Level; } }
        internal int ScoreValue;
        internal int Level = 1;
        public List<Algorithms.PathFinderNode> path;
        internal Level Map;
        internal float _size 
        { 
            get 
            {
                float x = _hits / _baseHitsAfterLevel; // = .50
                return (Width *x); 
            } 
        }
        double nextanim = 0;
        double _lastMove = 0;
        double _slowEffect = 0;
        byte framenum = 0;
        public int MoveDelay = 50;
        SharpDX.RectangleF HPoutline { get { return new SharpDX.RectangleF(this.ViewX + 10, this.ViewY - 6, this.ViewX + 30, this.ViewY - 2); } }

        SharpDX.RectangleF HPRect
        {
            get
            {
                float x = _hits / _baseHitsAfterLevel; // = .50
                float viewx = (this.ViewX + 10) + (20 * x); // lefthand side + length * x gives us bar percent long
                if (viewx < this.ViewX + 10) { viewx = this.ViewX + 11; }
                //if (x < 0.1f) { x = 0.1f; }
                return new SharpDX.RectangleF(this.ViewX + 10, this.ViewY - 5, viewx, this.ViewY - 3);
            }
        }

        public Monster(short TextureIndex, int width = 0, int height = 0) : base(TextureIndex,0,0,width,height)
        {
            Type = ObjectType.Monster;
            _hits = _baseHitsAfterLevel;
        }
        public Monster(short TextureIndex, Level map, int width = 0, int height = 0)
            : base(TextureIndex, 0, 0, width, height)
        {
            Type = ObjectType.Monster;
            _hits = _baseHitsAfterLevel;
        }
        public void initMob(Level map)
        {
            this.Map = map;
            var pathnum = Helper.random.Next(map.Starts.Count());
            this.WorldX = map.Starts[pathnum].X;
            this.WorldY = map.Starts[pathnum].Y;
            this.path = map.buildPath(map.Starts[pathnum]);
        }

      

       
        public void SetLevel(int level)
        {
            Level = level;
            _hits = _baseHitsAfterLevel;
        }

        public virtual Monster Clone()
        {
            return null;
            /*var m = new Monster(TextureIndex, Width, Height);
            m.initMob(Map);
            m.SetLevel(Level);
            return m;*/
        }

        public override void Update(World world, double curTime)
        {
            if (DeleteMe)
                return;
            if (this._hits <= 0f) 
            {
                this.Die(world);
            }

            if (path.Count > 0 && curTime > _lastMove)
            {
                Move(_velocity);
                _lastMove = curTime + (MoveDelay / Helper.GameSpeed);
                if (_slowEffect != 0)
                    _lastMove += (MoveDelay / Helper.GameSpeed);
            }
            else if(path.Count == 0)
            {
                world.Player.Lives--;
                this.DeleteMe = true;
            }
            if (_slowEffect < curTime && _slowEffect != 0)
            {
                this._velModifier = 0;
                this._slowEffect = 0;
            }
            if (curTime > nextanim)
            {
                framenum++;
                if (framenum >= (Direction + 1) * 19)
                    framenum = (byte)(Direction * 19);
                nextanim = curTime + 52;
                //Currently playing at 19FPS for animations.
            }
            
            base.Update(world,curTime);
        }

        private void Die(World world)
        {
            world.ParticleMan.CreateExplosion(ViewX, ViewY,world,this);
            world.Player.Score += this.ScoreValue;
            world.Player.Gold += (int)(this.ScoreValue * 0.7);
            this.DeleteMe = true; 
        }
        
        public override void Draw(GameForm gf)
        {
            if (_hits > 1) // HPRect Size will be wrong it HP = 0.
            {
                GameForm.solidColorBrush.Color = SharpDX.Colors.Black;
                gf.d2dRenderTarget.DrawRectangle(HPoutline, GameForm.solidColorBrush);
                GameForm.solidColorBrush.Color = SharpDX.Colors.Green;
                gf.d2dRenderTarget.FillRectangle(HPRect, GameForm.solidColorBrush);
            }
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
        }
        internal byte Direction = 1;
        private void Move(float _velocity)
        {
            var nextpath = path.Last();
            Vector3D dest = new Vector3D(nextpath.X, nextpath.Y,0);
            Vector3D ourloc = new Vector3D(WorldX, WorldY,0);
            Vector3D movepos = PowerMath.TranslateDirection2D(ourloc, dest, ourloc, this._velocity);

            if (this.WorldX < nextpath.X)
            {
                //WorldX += _velocity;
                Direction = 2;
            }
            else if (this.WorldX > nextpath.X)
            {
                //WorldX -= _velocity;
                Direction = 1;
            }
            else if (this.WorldY < nextpath.Y)
            {
                //WorldY += _velocity;
                Direction = 0;
            }
            WorldX = movepos.X;
            WorldY = movepos.Y;

            if (GetDistance(WorldX, WorldY, nextpath.X, nextpath.Y) < 0.3f)
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
