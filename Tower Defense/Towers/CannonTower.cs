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
using SharpDX;
using SharpDX.Direct2D1;

namespace Tower_Defense.Towers
{
    public class CannonTower : Tower
    {
        public CannonTower()
            : base(103, 0, 0, Helper.TowerSize, Helper.TowerSize)
        {
            _baseDamage = TowerStats.Cannon.BaseDamage;
            _fireRateMS = TowerStats.Cannon.BaseFireRateMS;
            _range = TowerStats.Cannon.RangeinTiles;
            Cost = TowerStats.Cannon.Price;
        }
       

        internal override void LevelUP()
        {
            base.LevelUP();
        }
        public override void Update(World world, double curTime)
        {
           
            if (curTime > _fireTimer)
            {
                try
                {
                    Monster target = (Monster)world.DrawableObjects.First(x => x.Type == ObjectType.Monster && Helper.GetDistance(x.WorldX, x.WorldY, this.WorldX, this.WorldY) < _range);
                    if (target != null)
                    {
                        Fired = true;
                        Target = target;
                        var debug = Helper.GetDistance(target.WorldX, target.WorldY, this.WorldX, this.WorldY);
                        target.DoDamage(_damage);
                        world.ParticleMan.CreateBullet(this, target);
                        _fireTimer = curTime + (_fireRateMS / Helper.GameSpeed);
                    }
                    Fired = false;
                    
                }
                catch { }
            }
            base.Update(world,curTime);

        }     
      
    }
}
