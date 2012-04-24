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
    public class Tower : DrawnObject
    {
        internal float _damage;
        internal float _range;
        internal int _fireRateMS;
        internal bool Fired = false;
        internal double _fireTimer = 0;
        internal int Level = 1;
        internal Monster Target;
        public Tower(short TextureIndex,int worldX, int worldY, int width = 0, int height = 0)
            : base(TextureIndex,worldX, worldY, width, height)
        {
            Type = ObjectType.Tower;

        }

        internal void LevelUP()
        {
            this.Level++;
            this._damage *= 1.2f;
            this._range *= 1.2f;
            this._fireRateMS = (int)(_fireRateMS * 1.2);
        }
    }
}
