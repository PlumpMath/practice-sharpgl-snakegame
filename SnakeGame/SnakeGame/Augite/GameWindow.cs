using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{

    using System.Windows.Forms;
    using SharpGL;
    using Augite.Events;

    abstract class GameWindow
    {

        public virtual string title { get; set; }
        public virtual int width { get; set; }
        public virtual int height { get; set; }
        public virtual int frameRate { get; set; }
        public virtual void run() { }

    }    
   
    class GLGameWindow : GameWindow
    {

        private System.Windows.Forms.Form _form;
        private Game _game;
        private GraphicsDeviceGL _graphicsDevice;

        public GLGameWindow(Game game, GraphicsDeviceGL graphicsDevice)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _form = new System.Windows.Forms.Form()
            {
                MaximizeBox = false,
            };

            _form.ClientSize = new System.Drawing.Size(720, 540);

            _form.Controls.Add(graphicsDevice.glControl);

            _form.Load += _form_Load;
            _form.Resize += _form_Resize;
            _form.Shown += _form_Shown;          
        }

        public override void run()
        {
            System.Windows.Forms.Application.Run(_form);
        }
        /**/

        public override int frameRate
        {
            get { return _graphicsDevice.glControl.FrameRate; }
            set { _graphicsDevice.glControl.FrameRate = value; }
        }

        public override string title
        {
            get
            {
                return _form.Text;
            }
            set
            {
                _form.Text = value;
            }
        }

        public override int width
        {
            get
            {
                return _graphicsDevice.glControl.ClientSize.Width;
            }

            set
            {
                var size = _form.ClientSize;
                size.Width = value;
                _form.ClientSize = size;
            }
        }

        public override int height
        {
            get
            {
                return _graphicsDevice.glControl.ClientSize.Height;
            }
            set
            {
                var size = _form.ClientSize;
                size.Height = value;
                _form.ClientSize = size;
            }
        }



        /**/


        private void _form_Shown(object sender, EventArgs e)
        {
            _game.init();
        }


        private void _form_Load(object sender, EventArgs e)
        {            
        }


        private void _form_Resize(object sender, EventArgs e)
        {
            if (_graphicsDevice.glControl != null)
            {
                _graphicsDevice.glControl.ClientSize = _form.ClientSize;
            }

            if(_game != null)
            {
                _game.onResize(_form.ClientSize);

            }
        }



    }

}
