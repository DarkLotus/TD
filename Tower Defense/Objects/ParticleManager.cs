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

// ToDo split projectiles into own class.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX;

namespace Tower_Defense.Objects
{
    internal class ParticleManager
    {
        List<Particle> Particles = new List<Particle>();
        
        //List<Particle> DrawnParticles = new List<Particle>();
        Random rand = new Random();
        public ParticleManager()
        { }


        public void Update(double curMS)
        {
            List<Particle> _delme = new List<Particle>();
            foreach (var p in Particles)
            {
                p.Update(curMS);
                if (p.DeleteMe)
                    _delme.Add(p);
            }


                if (_delme.Count > 0)
                    lock (Particles)
                        foreach (var p in _delme)
                            Particles.Remove(p);

                if (lightdelay == 0)
                    lightdelay = curMS + 200;
                if (lightdelay > 0 && lightdelay < curMS)
                    lock(Lines)
                        Lines.Clear();
        }

        public void CreateExplosion(int x, int y,World world,int texindex)
        {
            var b = world.Gameform.MonsterModels[(short)texindex];
            System.Drawing.Color c;
            for(int xx = 0; xx < 128;xx++)
            {
                for (int yy = 0; yy < 128; yy++)
            {
                c = b.FakeTex.GetPixel(xx, yy);
                if (c.B == 0 && c.G == 0 && c.R == 0)
                    continue;
                Vector3D v = new Vector3D(x + rand.Next(200) - 100, y + rand.Next(200) - 100,0);
                //var v = new Vector3D(x, y, 0);
                 Particles.Add(new Particle(x + (xx /2),y + (yy /2),v,(float)rand.NextDouble() - 0.1f,500+rand.Next(1500), c));
                 //y++;
            }
                //x++;
            }

            /*b.FakeTex.getp
            Color4[] colors = new Color4[] { Colors.WhiteSmoke, Colors.Red, Colors.Gray, Colors.DarkRed };
            for (int i = 0; i < 100; i++)
            {
                //var v = new Vector2(2 * ((float)getrandFloat() - 0.5f), 2 * ((float)getrandFloat() - 0.5f));
                //var v = new Vector2(rand.Next(200) - 100, rand.Next(200) - 100);
                Vector3D v = new Vector3D(x + rand.Next(200) - 100, y + rand.Next(200) - 100,0);
                //Particles.Add(new Particle(x, y,v , 500 + rand.Next(2000)) { Color = Colors.WhiteSmoke });
                Particles.Add(new Particle(x,y,v,(float)rand.NextDouble(),500+rand.Next(1500),colors[rand.Next(3)]));
            }*/

        }

        
        public void CreatePulse(int x, int y)
        {
            for (int i = 0; i < 100; i++)
            {
                //var v = new Vector2(2 * ((float)getrandFloat() - 0.5f), 2 * ((float)getrandFloat() - 0.5f));
                //v.Normalize();
                Vector3D v = new Vector3D(x + rand.Next(200) - 100, y + rand.Next(200) - 100, 0);
                //var v = new Vector3D(x, y, 0);
                Particles.Add(new Particle(x, y, v, (float)0.5f, 1500 + rand.Next(200),Colors.Blue));
            }

        }
        public void CreateBullet(Tower t, Monster m)
        {
            //Vector3D v = new Vector3D(m.ViewX - t.ViewX, m.ViewY - t.ViewY, 0);
            Vector3D v = new Vector3D(m.ViewX, m.ViewY, 0);
            Particles.Add(new Particle(t.ViewX, t.ViewY, v, 1f, 1500, Colors.Red));
        }
       

        private float getrandFloat()
        {
            return (float)rand.NextDouble();
        }
        public void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget)
        {
            //if (DrawnParticles == null)
            //    return;
            GameForm.solidColorBrush.Color = Colors.AliceBlue;
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Draw(d2dRenderTarget);
            }
            var x = Lines.ToArray();
            GameForm.solidColorBrush.Color = Colors.Yellow;
            for (int i = 0; i < x.Count() - 1; i++)
                d2dRenderTarget.DrawLine(x[i], x[i + 1], GameForm.solidColorBrush);

                
            
        }
        public List<DrawingPointF> Lines = new List<DrawingPointF>();
        double lightdelay;
        public void CreateLightning(Tower t, Monster m, List<DrawnObject> mobs)
        {
            Lines.Add(new DrawingPointF(t.ViewX,t.ViewY));
            Lines.Add(new DrawingPointF(m.ViewX,m.ViewY));
            foreach(var x in mobs)
                Lines.Add(new DrawingPointF(x.ViewX, x.ViewY));
            lightdelay = 0;
        }


       // public int ParticleCount { get { return DrawnParticles.Count; } }
    }

    internal class Particle
    {
        public Color4 Color;
        Vector3D Location;
        Vector3D Dest;
        float Velocity;
        public bool DeleteMe = false;
        double LifeSpan, tickToDieAt;

        SharpDX.Direct2D1.Ellipse el;
        public Particle(float x, float y, Vector3D dest,float vel, int lifeSpan,Color4 color)
        {
            Location = new Vector3D(x, y, 0);
            Velocity = vel;
            LifeSpan = lifeSpan;
            Color = color;
            Dest = dest;
            el = new SharpDX.Direct2D1.Ellipse(new DrawingPointF(x, y), 0.5f, 0.5f);

        }

        internal void Update(double curMS)
        {
            if (tickToDieAt == 0.0)
                tickToDieAt = curMS + LifeSpan;
            var dest =PowerMath.TranslateDirection2D(Location, Dest, Location, Velocity);
            this.Location = dest;
            el.Point = new DrawingPointF(Location.X, Location.Y);
            if (curMS > tickToDieAt)
                this.DeleteMe = true;

        }
        internal void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget)
        {
            if (Color != null)
                GameForm.solidColorBrush.Color = Color;
            d2dRenderTarget.FillEllipse(el, GameForm.solidColorBrush);
        }
    }

    internal class Particle2
    {
        public Color4 Color;
        DrawingPointF Location;
        Vector2 Location2, Velocity;
        int LifeSpanMS;
        public bool DeleteMe = false;
        private double tickToDieAt;
        SharpDX.Direct2D1.Ellipse el;
        internal void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget) 
        {
            if (Color != null)
                GameForm.solidColorBrush.Color = Color;
            d2dRenderTarget.FillEllipse(el, GameForm.solidColorBrush);
        }

        internal void Update(double curMS) 
        {
            if (tickToDieAt == 0.0)
                tickToDieAt = curMS + LifeSpanMS;
            Location2 += Velocity;
            Location.X = Location2.X;
            Location.Y = Location2.Y;
            el.Point = Location;
            if (curMS > tickToDieAt)
                this.DeleteMe = true;
        }
        public Particle2(float x, float y, Vector2 velocity,int lifeSpan)
        {
            Location = new DrawingPointF(x, y);
            Location2 = new Vector2(x, y);
            LifeSpanMS = lifeSpan;
            Velocity = velocity;

            el = new SharpDX.Direct2D1.Ellipse(Location, 0.5f, 0.5f);

        }
    }

   /* internal class Bullet : Particle
    {
        internal override void Draw()
        {
            throw new NotImplementedException();
        }

        internal override void Update(double curMS)
        {
            throw new NotImplementedException();
        }
    }*/
}
