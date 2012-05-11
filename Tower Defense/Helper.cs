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

namespace Tower_Defense
{


    public static class Helper
    {
        public static int TowerSize = 40;
        public static int MonsterSize = 40;
        public static byte GameSpeed = 1; // 1-2;
        public static bool Contains(System.Drawing.Rectangle rect, System.Drawing.Point point)
        {
            if (rect.Top < point.Y && rect.Bottom > point.Y && rect.Left < point.X && rect.Right > point.X)
                return true;
            return false;
        }


        public static bool Contains(RectangleF rect, System.Drawing.Point point)
        {
            if (rect.Top < point.Y && rect.Bottom > point.Y && rect.Left < point.X && rect.Right > point.X)
                return true;
            return false;
        }
        public static bool Contains(System.Drawing.Rectangle ViewPort, RectangleF rectangleF)
        {
            if (ViewPort.Contains((int)rectangleF.Left, (int)rectangleF.Right))
                return true;
            return false;
        }


        public static bool Contains(System.Drawing.Rectangle rect, Level.MapTile point)
        {
            if (rect.Top < point.ScreenSprite.Top && rect.Bottom > point.ScreenSprite.Bottom && rect.Left < point.ScreenSprite.Left && rect.Right > point.ScreenSprite.Right)
                return true;
            return false;
        }
        public static float GetDistance(float x1, float y1, float x2, float y2)
        {
            double part1 = Math.Pow((x2 - x1), 2);
            //Take y2-y1, then sqaure it
            double part2 = Math.Pow((y2 - y1), 2);
            //Add both of the parts together
            double underRadical = part1 + part2;
            //Get the square root of the parts
            var result =(int)Math.Sqrt(underRadical);
            return result;
          
        }
        public static Random random = new Random();

        public static float RandomFloat(float max)
        {
            Random random = new Random();
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }
    }

    /// <summary>
    /// GPL Code taken from the Mooege project, www.mooege.org www.github.com/mooege/
    /// </summary>
    #region Mooege Classes
    public class Vector3D
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3D()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public Vector3D(Vector3D vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
            this.Z = vector.Z;
        }

        public Vector3D(float x, float y, float z)
        {
            Set(x, y, z);
        }




        public void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("Vector3D:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad);
            b.AppendLine("X: " + X.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Y: " + Y.ToString("G"));
            b.Append(' ', pad);
            b.AppendLine("Z: " + Z.ToString("G"));
            b.Append(' ', --pad);
            b.AppendLine("}");
        }

        public void Set(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Calculates the distance squared from this vector to another.
        /// </summary>
        /// <param name="point">the second <see cref="Vector3" /></param>
        /// <returns>the distance squared between the vectors</returns>
        public float DistanceSquared(ref Vector3D point)
        {
            float x = point.X - X;
            float y = point.Y - Y;
            float z = point.Z - Z;

            return ((x * x) + (y * y)) + (z * z);
        }

        public static bool operator ==(Vector3D a, Vector3D b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            return a.Equals(b);
        }

        public static bool operator !=(Vector3D a, Vector3D b)
        {
            return !(a == b);
        }

        public static bool operator >(Vector3D a, Vector3D b)
        {
            if (object.ReferenceEquals(null, a))
                return !object.ReferenceEquals(null, b);
            return a.X > b.X
                && a.Y > b.Y
                && a.Z > b.Z;
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static bool operator <(Vector3D a, Vector3D b)
        {
            return !(a > b);
        }

        public static bool operator >=(Vector3D a, Vector3D b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            return a.X >= b.X
                && a.Y >= b.Y
                && a.Z >= b.Z;
        }

        public static bool operator <=(Vector3D a, Vector3D b)
        {
            if (object.ReferenceEquals(null, a))
                return object.ReferenceEquals(null, b);
            return a.X <= b.X
                && a.Y <= b.Y
                && a.Z <= b.Z;
        }

        public override bool Equals(object o)
        {
            if (object.ReferenceEquals(this, o))
                return true;
            var v = o as Vector3D;
            if (v != null)
            {
                return this.X == v.X
                    && this.Y == v.Y
                    && this.Z == v.Z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2}", X, Y, Z);
        }
    }

    public static class PowerMath
    {
        #region Vector operations





        public static Vector3D Normalize(Vector3D v)
        {
            float mag = v.X * v.X + v.Y * v.Y + v.Z * v.Z;
            if (mag == 0)
                return new Vector3D(0, 0, 0);

            Vector3D r = new Vector3D(v);
            float len = 1f / (float)Math.Sqrt(mag);
            r.X *= len;
            r.Y *= len;
            r.Z *= len;
            return r;
        }

        public static float Distance(Vector3D a, Vector3D b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) +
                                    Math.Pow(a.Y - b.Y, 2) +
                                    Math.Pow(a.Z - b.Z, 2));
        }

        public static float Distance2D(Vector3D a, Vector3D b)
        {
            return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) +
                                    Math.Pow(a.Y - b.Y, 2));
        }

        public static Vector3D TranslateDirection2D(Vector3D source, Vector3D destination, Vector3D point, float amount)
        {
            Vector3D norm = Normalize(new Vector3D(destination.X - source.X, destination.Y - source.Y, 0f));
            return new Vector3D(point.X + norm.X * amount,
                                point.Y + norm.Y * amount,
                                point.Z);
        }

        public static Vector3D VectorRotateZ(Vector3D v, float radians)
        {
            float cosRad = (float)Math.Cos(radians);
            float sinRad = (float)Math.Sin(radians);

            return new Vector3D(v.X * cosRad - v.Y * sinRad,
                                v.X * sinRad + v.Y * cosRad,
                                v.Z);
        }

        public const float DegreesToRadians = (float)(Math.PI / 180.0);

        public static Vector3D[] GenerateSpreadPositions(Vector3D center, Vector3D targetPosition, float spacingDegrees, int count)
        {
            Vector3D baseRotation = targetPosition - center;
            float spacing = spacingDegrees * DegreesToRadians;
            float median = count % 2 == 0 ? spacing * (count + 1) / 2.0f : spacing * (float)Math.Ceiling(count / 2.0f);
            Vector3D[] output = new Vector3D[count];

            float offset = 1f;
            for (int i = 0; i < count; ++i)
            {
                output[i] = center + VectorRotateZ(baseRotation, offset * spacing - median);
                offset += 1f;
            }

            return output;
        }

        #endregion

    }
    #endregion

    
}
