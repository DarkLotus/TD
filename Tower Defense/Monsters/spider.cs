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
namespace Tower_Defense.Monsters
{
    public class Tank : Monster
    {
        public Tank()
            : base(1, 32, 32)
        {
            this._baseVelocity = 0.02f;
            this._baseHits = 15f;
            this.ScoreValue = 12;
        }
      

        public override void Update(World world, double curTime)
        {
            base.Update(world,curTime);

        }
        public override Monster Clone()
        {
            var m = new Tank(); m.initMob(Map); m.SetLevel(this.Level);
            return m;
        }
    }
}
