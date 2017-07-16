using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{

    using SharpGL;

    class Texture2D
    {

        private int _width;
        private int _height;

        public int width { get { return _width; } }
        public int height { get { return _height; } }

        private SharpGL.SceneGraph.Assets.Texture _tex;
        public SharpGL.SceneGraph.Assets.Texture tex { get { return _tex; } }

        public Texture2D(SharpGL.SceneGraph.Assets.Texture tex, int width, int height)
        {
            _tex = tex;
            _width = width;
            _height = height;
        }
       

        public static Texture2D fromBitmap(OpenGL gl, System.Drawing.Bitmap bitmap)
        {
            var _bitmap = bitmap.Clone() as System.Drawing.Bitmap;
            //_bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

            var tex = new SharpGL.SceneGraph.Assets.Texture();
            tex.Create(gl, _bitmap);
            var tex2d = new Texture2D(tex, bitmap.Width, bitmap.Height);

            return tex2d;
        }

    }
}
