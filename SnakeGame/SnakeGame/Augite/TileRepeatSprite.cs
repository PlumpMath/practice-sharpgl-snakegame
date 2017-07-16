using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{
    using SharpGL;
    using Augite.Events;



    class TileRepeatSprite:Sprite
    {

        public int tileWidth;
        public int tileHeight;

        public TileRepeatSprite(Game game) : base(game)
        {
        }

        public TileRepeatSprite(Game game, Texture2D tex):base(game, tex)
        {
            tileWidth = tex.width;
            tileHeight = tex.height;
        }

        public override System.Drawing.Rectangle drawBounds
        {
            get
            {
                return new System.Drawing.Rectangle(0, 0, width, height);
            }
        }


        public override void draw(DrawArgs args)
        {
            var t1 = DateTime.Now;

            _draw(args);
            var delta = DateTime.Now - t1;

           // Console.WriteLine("TILESPRITE DRAW={0}", delta);
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


                if (tex != null)
                {
                    // var _tex = tex.tex;

                    var parentsPt = getGlobalPt();
                    var drawBounds = this.drawBounds;

                    var globalDrawBounds = new System.Drawing.RectangleF(parentsPt.X + drawBounds.X, parentsPt.Y + drawBounds.Y, drawBounds.Width, drawBounds.Height);

                    var globalDrawBoundsIntersection = globalDrawBounds;
                    globalDrawBoundsIntersection.Intersect(args.viewportBounds);

                    // globalDrawBoundsIntersection = globalDrawBounds;

                    var drawBoundsResult = globalDrawBoundsIntersection;
                    drawBoundsResult.X -= parentsPt.X;
                    drawBoundsResult.X -= drawBounds.X;
                    drawBoundsResult.Y -= parentsPt.Y;
                    drawBoundsResult.Y -= drawBounds.Y;


                    float tileTexWidth = this.tileWidth;
                    float tileTexHeight = this.tileHeight;

                    int drawTileStartX = (int)(Math.Floor(drawBoundsResult.X / tileTexWidth));
                    int drawTileStartY = (int)(Math.Floor(drawBoundsResult.Y / tileTexHeight));
                    int drawLoopWidth = (int)Math.Ceiling((drawBoundsResult.Width / tileTexWidth));
                    int drawLoopHeight = (int)(Math.Ceiling(drawBoundsResult.Height / tileTexHeight));
                    int drawTileEndX = drawTileStartX + drawLoopWidth + 1;
                    int drawTileEndY = drawTileStartY + drawLoopHeight + 1;

                    int drawTileEndMaxX = (int)(Math.Ceiling(drawBounds.Width / tileTexWidth));
                    int drawTileEndMaxY = (int)(Math.Ceiling(drawBounds.Height / tileTexHeight));

                    if (drawTileEndY > drawTileEndMaxY)
                    {
                        drawTileEndY = drawTileEndMaxY;
                    }

                    if (drawTileEndX > drawTileEndMaxX)
                    {
                        drawTileEndX = drawTileEndMaxX;
                    }

                    tex.tex.Bind(gl);

                    gl.Color(1.0f, 1.0f, 1.0f, this.alpha);

                    gl.Enable(OpenGL.GL_BLEND);
                    gl.Enable(OpenGL.GL_TEXTURE_2D);
                    gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
                    gl.DepthFunc(OpenGL.GL_LEQUAL);

                    gl.PushMatrix();
                    gl.Translate(drawBounds.X, drawBounds.Y, 0);

                    int drawCount = 0;
                    for (int y= drawTileStartY; y< drawTileEndY; y++)
                    {
                        for (int x = drawTileStartX; x < drawTileEndX; x++)
                        {
                            float x2 = x * tileTexWidth;
                            float y2 = y * tileTexHeight;

                         //   Console.WriteLine("tile x2={0} y2={1}", x2, y2);

                            gl.PushMatrix();
                            gl.Translate(x2, y2, 0);

                            gl.Begin(OpenGL.GL_QUADS);
                            {

                                gl.TexCoord(0, 0);
                                gl.Vertex(0, 0);

                                gl.TexCoord(0, 1);
                                gl.Vertex(0, tileTexHeight);

                                gl.TexCoord(1, 1);
                                gl.Vertex(tileTexWidth, tileTexHeight);


                                gl.TexCoord(1, 0);
                                gl.Vertex(tileTexWidth, 0);

                            }
                            gl.End();
                            gl.PopMatrix();

                            drawCount++;
                        }
                    }

                    gl.PopMatrix();


                   // Console.WriteLine("DRAW COUNT={0}", drawCount);

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
