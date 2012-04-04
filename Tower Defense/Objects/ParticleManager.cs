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

        List<Particle> DrawnParticles = new List<Particle>();
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
        }

        public void CreateExplosion(int x, int y)
        {
            for (int i = 0; i < 50; i++)
            {
                Particles.Add(new Particle(x, y, new Vector2(2 * ((float)getrandFloat() - 0.5f), 2 * ((float)getrandFloat() - 0.5f)), 1000 + rand.Next(5000)) { Color = Colors.WhiteSmoke });
            }

        }
        public void CreateBullet(Tower t, Monster m)
        {
            var vec = new Vector2(m.ViewX - t.ViewX, m.ViewY - t.ViewY);
            vec.Normalize();
            vec += vec;


        Particles.Add(new Particle(t.ViewX,t.ViewY,vec,500){ Color = Colors.Blue});
        }

        private float getrandFloat()
        {
            //Thread.Sleep(1);
            return (float)rand.NextDouble();
        }
        public void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget)
        {
            if (DrawnParticles == null)
                return;
            GameForm.solidColorBrush.Color = Colors.AliceBlue;
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Draw(d2dRenderTarget);
            }
                
            
        }


        public int ParticleCount { get { return DrawnParticles.Count; } }
    }

    internal class Particle
    {
        int X, Y;
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
        public Particle(float x, float y, Vector2 velocity,int lifeSpan)
        {
            Location = new DrawingPointF(x, y);
            Location2 = new Vector2(x, y);
            LifeSpanMS = lifeSpan;
            Velocity = velocity;
            el = new SharpDX.Direct2D1.Ellipse(Location, 1, 1);

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
