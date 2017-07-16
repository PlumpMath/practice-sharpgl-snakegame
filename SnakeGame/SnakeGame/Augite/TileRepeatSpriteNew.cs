using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{
    using SharpGL;
    using Augite.Events;



    class TileRepeatSpriteNew : Sprite
    {

        private System.Drawing.Bitmap _bitmap;
        public int tileWidth;
        public int tileHeight;
        private Texture2D _tex;

        public TileRepeatSpriteNew(Game game) : base(game)
        {
        }

        public TileRepeatSpriteNew(Game game, Texture2D tex):base(game, tex)
        {
            tileWidth = tex.width;
            tileHeight = tex.height;
        }

        public TileRepeatSpriteNew(Game game, System.Drawing.Bitmap bitmap) : base(game)
        {
            _bitmap = bitmap;
            _tex = game.graphicsDevice.texFromBitmap(bitmap);
        }


        public override void draw(DrawArgs args)
        {
            var t1 = DateTime.Now;

            _draw(args);
            var delta = DateTime.Now - t1;

            Console.WriteLine("TILESPRITE DRAW={0}", delta);
        }

        public override System.Drawing.Rectangle drawBounds
        {
            get
            {
                return new System.Drawing.Rectangle(0, 0, width, height);
            }
        }

       
        private void _draw(DrawArgs args)
        {
            var gl = args.gl;

            if (visible)
            {
                gl.PushMatrix();
                //gl.Scale(1, 1, 1);              
                gl.Translate(x, y, 0);
                gl.Rotate(0, 0, rotation);

                gl.PushMatrix();
                gl.Scale(scaleX, scaleY, 1);


                if (_bitmap != null)
                {
                    _tex.tex.Bind(gl);

                    int tileTexWidth = tileWidth;
                    int tileTexHeight = tileHeight;


                    var bd = _bitmap.LockBits(new System.Drawing.Rectangle(0, 0, _bitmap.Width, _bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, _bitmap.PixelFormat);

                    var drawBounds = this.drawBounds;


                   //
                  //  gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, 32, 32, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, bd.Scan0);

                    gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                    gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                
                    gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                    gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                    gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, _bitmap.Width, _bitmap.Height, 0, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, bd.Scan0);
                   // gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, OpenGL.GL_RGBA, _bitmap.Width, _bitmap.Height, OpenGL.GL_RGBA, OpenGL.GL_UNSIGNED_BYTE, bd.Scan0);

                    gl.Color(1.0f, 1.0f, 1.0f, this.alpha);

                    gl.Enable(OpenGL.GL_BLEND);
                    gl.Enable(OpenGL.GL_TEXTURE_2D);
                    gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
                    gl.DepthFunc(OpenGL.GL_LEQUAL);

                    int x2 = drawBounds.X + drawBounds.Width;
                    int y2 = drawBounds.Y + drawBounds.Height;

                    gl.Begin(OpenGL.GL_QUADS);
                    {

                        gl.TexCoord(0, 0);
                        gl.Vertex(drawBounds.X, drawBounds.Y);

                        gl.TexCoord(0, 1);
                        gl.Vertex(drawBounds.X, y2);

                        gl.TexCoord(1, 1);
                        gl.Vertex(x2, y2);


                        gl.TexCoord(1, 0);
                        gl.Vertex(x2, drawBounds.Y);

                    }

                    gl.End();

                    _bitmap.UnlockBits(bd);
                  

                    gl.Disable(OpenGL.GL_BLEND);
                    gl.Disable(OpenGL.GL_TEXTURE_2D);
                }


            

                for (int i = 0; i < children.Count; ++i)
                {
                    children[i].draw(args);
                }

                gl.PopMatrix();
                gl.PopMatrix();
            }

        }
    }

}
