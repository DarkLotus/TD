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

        public int Cost;
        internal Monster Target;
        public Tower(short TextureIndex,int worldX, int worldY, int width = 0, int height = 0)
            : base(TextureIndex,worldX, worldY, width, height)
        {
            Type = ObjectType.Tower;

        }

        internal  virtual void LevelUP()
        {
            this.Level++;
            this._damage *= 1.2f;
            this._range *= 1.2f;
            this._fireRateMS = (int)(_fireRateMS * 1.2);
        }
    }

    public static class TowerStats
    {
        //TODO add level up data to towers, Level Cap?
        public static class Basic
        {
            public static string Name = "Arrow Tower";
            public static int Price = 25;
            public static float BaseDamage = 8f;
            public static int BaseFireRateMS = 1000;
            public static float RangeinTiles = 2f;
        }
        public static class Cannon
        {
            public static string Name = "Cannon Tower";
            public static int Price = 50;
            public static float BaseDamage = 20f;
            public static int BaseFireRateMS = 1800;
            public static float RangeinTiles = 4f;
        }
        public static class Light
        {
            public static string Name = "Lightning Tower";
            public static int Price = 100;
            public static float BaseDamage = 8f;
            public static int BaseFireRateMS = 5000;
            public static float RangeinTiles = 3f;
        }
        public static class Slow
        {
            public static string Name = "Slowing Tower";
            public static int Price = 100;
            public static float BaseDamage = 3f;
            public static int BaseFireRateMS = 4500;
            public static float RangeinTiles = 2f;

            public static double SlowDuration = 4000;
            public static float SlowAmount = 0.06f;
        }
    }
}
