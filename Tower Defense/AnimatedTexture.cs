using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using Tower_Defense;
namespace Tower_Defense
{
    public class AnimatedTexture
    {
        //Dictionary<byte, Texture2D> _frames = new Dictionary<byte, Texture2D>();
        Dictionary<byte, Bitmap> _frames = new Dictionary<byte, Bitmap>();
        // TODO Add load from sprite sheet.
        public AnimatedTexture(string texturename, byte count,Device device, RenderTarget d2dRender)
        {
            for (int i = 0; i < count; i++)
            {
                //_frames.Add((byte)i,Texture2D.FromFile<Texture2D>(device,texturename + "_0.png"));
                _frames.Add((byte)i,GameForm.LoadFromFile(d2dRender, File.OpenRead("Art\\" + texturename + "_0.png")));
            }
        }

    }
}
