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
using SharpDX;
using SharpDX.Direct2D1;

namespace Tower_Defense.Towers
{
    public class SlowingTower : Tower
    {

        private double SlowDurationMS = 4000;
        private float SlowEffect = 0.08f;
        public SlowingTower(int x, int y)
            : base(99,x, y, 24, 24)
        {
            _damage = 1f;
            _fireRateMS = 1500;
            _range = 3f;
            color = Colors.LightBlue;
            
        }

        public override void Update(World world, double curTime)
        {
                        if (curTime > _fireTimer)
            {
                try
                {
                    var monsters = world.DrawableObjects.FindAll(a => a.Type == ObjectType.Monster && Helper.GetDistance(a.WorldX, a.WorldY, this.WorldX, this.WorldY) < _range);
                    if (monsters.Count > 0)
                    {
                        Fired = true;
                        foreach (var target in monsters)
                        {
                            if (target != null)
                            {
                                
                                Target = (Monster)target;
                                //var debug = Helper.GetDistance(target.WorldX, target.WorldY, this.WorldX, this.WorldY);
                                Target.DoDamage(_damage);
                                Target.SlowMe(SlowEffect, curTime + SlowDurationMS);

                                _fireTimer = curTime + _fireRateMS;
                            }
                            
                        }
                        world.ParticleMan.CreatePulse(this.ViewX + Width/2, this.ViewY + Height/2);
                    }
                    Fired = false;
                    //Monster target = (Monster)world.DrawableObjects.First(x => x.Type == ObjectType.Monster && Helper.GetDistance(x.WorldX, x.WorldY, this.WorldX, this.WorldY) < _range);

                    
                }
                catch { }
            }
            base.Update(world,curTime);

        }
        
               
    }
}
