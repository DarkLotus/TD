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
    internal class Runner : Monster
    {
        internal Runner( Level map)
            : base(0,map, 64, 48)
        {
            this._baseVelocity = 0.1f;
            this._hits = 50f;
            this._baseHits = 50f;
            this.ScoreValue = 10;
            this.color = Colors.SpringGreen;
            //this.ScreenSprite = new RectangleGeometry(d2dfactory,new RectangleF(ViewX, ViewY, ViewX + Width, ViewY + Height));
        }

        public override void Update(World world, double curTime)
        {
            base.Update(world,curTime);

        }
        public override Monster Clone()
        {
            return new Runner(Map);
        }
    }
}
