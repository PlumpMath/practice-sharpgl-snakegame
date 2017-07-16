using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Augite
{
    using System.Windows.Forms;
    using SharpGL;

    class GraphicsDevice
    {

        public virtual Texture2D texFromBitmap(System.Drawing.Bitmap bitmap)
        {
            throw new NotImplementedException();
           // return Texture2D.fromBitmap(_gl, bitmap);
        }
    }

    class GraphicsDeviceGL : GraphicsDevice
    {

        private SharpGL.OpenGLControl _glControl;
        public SharpGL.OpenGLControl glControl { get { return _glControl; } }
        private Game _game;

        public GraphicsDeviceGL(Game game)
        {
            _game = game;
            _init();
        }

        private void _init()
        {
            _glControl = new OpenGLControl();

            var init = (System.ComponentModel.ISupportInitialize)_glControl;
            init.BeginInit();

            _glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            _glControl.FrameRate = 16;
            _glControl.DrawFPS = true;
            //_glControl.ClientSize = _form.ClientSize;
            _glControl.GDIDraw += _glControl_GDIDraw;
            _glControl.OpenGLDraw += _glControl_OpenGLDraw;
            _glControl.OpenGLInitialized += _glControl_OpenGLInitialized;
            _glControl.Resized += _glControl_Resized;
            _glControl.MouseDown += _glControl_MouseDown;
            _glControl.MouseMove += _glControl_MouseMove;
            _glControl.MouseUp += _glControl_MouseUp;
            _glControl.MouseClick += _glControl_MouseClick;
            _glControl.KeyPress += _glControl_KeyPress;
            _glControl.KeyUp += _glControl_KeyUp;
            _glControl.PreviewKeyDown += _glControl_PreviewKeyDown;


            init.EndInit();
        }

  

        public override Texture2D texFromBitmap(Bitmap bitmap)
        {
            return Texture2D.fromBitmap(_glControl.OpenGL, bitmap); 
        }


        private void _glControl_OpenGLInitialized(object sender, EventArgs e)
        {
            var gl = _glControl.OpenGL;

            gl.ClearColor(0, 0, 0, 1.0f);
        }


        private void _glControl_Resized(object sender, EventArgs e)
        {
            var gl = _glControl.OpenGL;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            //gl.MatrixMode(OpenGL.GL_2D);

            Console.WriteLine("OnResize");
        }


        private void _glControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //Console.WriteLine("_glControl_PreviewKeyDown");
            _game.onKeyDown(e.KeyCode);
            
        }

        private void _glControl_KeyUp(object sender, KeyEventArgs e)
        {
            _game.onKeyPress(e.KeyCode);
        }

        private void _glControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            _game.onKeyPress((Keys)e.KeyChar);

        }

        private void _glControl_MouseUp(object sender, MouseEventArgs e)
        {
            _game.onMouseUp(e);
        }

        private void _glControl_MouseDown(object sender, MouseEventArgs e)
        {
            _game.onMouseDown(e);
          //  Console.WriteLine("_glControl_MouseDown");
        }

        private void _glControl_MouseMove(object sender, MouseEventArgs e)
        {
            _game.onMouseMove(e);
        }

        private void _glControl_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;// _glControl.ClientSize.Height - e.Y;
           // Console.WriteLine("_glControl_MouseClick pt={0},{1}", x, y);
           
            _game.onMouseClick(e);           
        }


        private void _glControl_GDIDraw(object sender, RenderEventArgs args)
        {

            _game.drawGDI(args.Graphics);
        }

        private void _glControl_OpenGLDraw(object sender, RenderEventArgs args)
        {

          
          //  Console.WriteLine("_glControl_OpenGLDraw");

            var gl = _glControl.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.ClearDepth(1.0f);
            gl.LoadIdentity();

            gl.Ortho2D(0, _glControl.ClientSize.Width, -_glControl.ClientSize.Height, 0);
            gl.Viewport(0, 0, _glControl.ClientSize.Width, _glControl.ClientSize.Height);

            gl.PushMatrix();
            gl.Scale(1, -1, 1);

            _game.draw(gl);

            gl.PopMatrix();

            gl.Flush();

           


        }
    }


}
