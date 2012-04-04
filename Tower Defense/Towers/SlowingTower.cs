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
        private float _damage;
        private float _range;
        private int _fireRateMS;
        public SlowingTower(int x, int y)
            : base(x, y, 24, 24)
        {
            _damage = 5f;
            _fireRateMS = 300;
            _range = 2f;
            
        }

        double _fireTimer = 0;
        public override void Update(World world, double curTime)
        {
            if(ScreenSprite == null)
                this.ScreenSprite = new RectangleGeometry(world.Gameform.d2dFactory, new RectangleF(ViewX +12, ViewY +12, ViewX + Width, ViewY + Height));
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
                        _fireTimer = curTime + _fireRateMS;
                    }
                    Fired = false;
                    
                }
                catch { }
            }
            base.Update(world,curTime);

        }
        private bool Fired = false;
        private Monster Target;
        public override void Draw(SharpDX.Direct2D1.RenderTarget d2dRenderTarget)
        {
            if (ScreenSprite != null)
            d2dRenderTarget.DrawGeometry(ScreenSprite, GameForm.TowerBrush);
            //if(Fired && Target != null)
            //    d2dRenderTarget.DrawLine(new DrawingPointF(this.ViewX +15,this.ViewY + 15),new DrawingPointF(Target.ViewX + 15,Target.ViewY + 15),GameForm.TowerBrush);
            //base.Draw(d2dRenderTarget);
        }

       
    }
}
