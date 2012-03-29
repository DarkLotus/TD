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
        internal Runner(SharpDX.Direct2D1.Factory d2dfactory, Map map)
            : base(map, 10, 10)
        {
            this._velocity = 0.1f;
            this._hits = 50f;
            this.ScreenSprite = new RectangleGeometry(d2dfactory,new RectangleF(ViewX, ViewY, ViewX + Width, ViewY + Height));
        }

        public override void Update(World world, double curTime)
        {
            base.Update(world,curTime);
            this.ScreenSprite = new RectangleGeometry(ScreenSprite.Factory, new RectangleF(ViewX, ViewY, ViewX + Width, ViewY + Height));
        }
    }
}
