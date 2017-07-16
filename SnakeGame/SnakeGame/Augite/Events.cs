using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite.Events
{
    delegate void MouseClickEvent(MouseEvent evt);

    class KeyEvent
    {
        public System.Windows.Forms.Keys keyCode;
    }

    class MouseEvent
    {
        public int x;
        public int y;
        public int stageX { get { return _args.stageX; } }
        public int stageY { get { return _args.stageY; } }
        public Stage stage { get { return _args.stage; } }
        public System.Drawing.Size clientSize { get { return _args.clientSize; } }

        public object target;

        private MouseEventArgs _args;

        public MouseEvent(MouseEventArgs args)
        {
            _args = args;
        }

        public override string ToString()
        {
            return string.Format("<MouseEvent x={0} y={1}>", x, y);
        }
    }

    class MouseEventArgs
    {
        public Stage stage;
        public int stageX;
        public int stageY;
        public System.Drawing.Size clientSize;
    }


    class JoystickEvent
    {
        public float angle;

        public override string ToString()
        {
            return string.Format("<JoystickEvent angle={0}>", angle);
        }
    }
}
