using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{
    using SharpGL;
    using Augite.Events;



    class Sprite
    {

        
        public event MouseClickEvent mouseClick;
        public event MouseClickEvent mouseDown;
        public event MouseClickEvent mouseMove;
        public event MouseClickEvent mouseUp;

        private EVisibility _visibility = EVisibility.Visible;

        public EVisibility visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
            }
        }

        private List<Sprite> _children;
        private Texture2D _tex;
        private float _alpha = 1.0f;
        private int _x = 0;
        private int _y = 0;

        public Texture2D tex { get { return _tex; } }
     
        public virtual int x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public virtual int y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }



        public int width;
        public int height;
        public float rotation;
        public bool visible = true;
        public float scaleX = 1.0f;
        public float scaleY = 1.0f;

      

        public virtual float alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                _alpha = value;
            }
        }


        private Sprite _parent;
        public Sprite parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }


        private Game _game;

        public Game game
        {
            get
            {
                return _game;
            }
        }


        public Sprite(Game game)
        {
            _init(game, null);
        }

        public Sprite(Game game, Texture2D tex)
        {
            _init(game, tex);
        }

        public List<Sprite> children
        {
            get
            {
                return _children; 
            }
        }


        private void _init(Game game, Texture2D tex)
        {
            _game = game;
            _tex = tex;
            _children = new List<Sprite>();
            visible = true;
            scaleX = 1.0f;
            scaleY = 1.0f;
            x = 0;
            y = 0;
            rotation = 0;

            if (_tex != null)
            {
                width = tex.width;
                height = tex.height;
            }
        }

        public void addChild(Sprite child)
        {
            _children.Add(child);
            child.parent = this;
        }

        public System.Drawing.PointF getGlobalPt()
        {
            int parentsX = this.x;
            int parentsY = this.y;

            Sprite _parent = this.parent;
            while (_parent != null)
            {
                parentsX += _parent.x;
                parentsY += _parent.y;

                _parent = _parent.parent;
            }

            return new System.Drawing.PointF(parentsX, parentsY);
        }


        public void clear()
        {
            _children.Clear();
        }

        public virtual System.Drawing.Rectangle bounds
        {
            get 
            {
                return new System.Drawing.Rectangle(this.x - width / 2, this.y - height / 2, width, height);
            }
        }

        public virtual System.Drawing.Rectangle getDrawHitBounds()
        {
            return new System.Drawing.Rectangle(- width / 2, - height / 2, width, height);

        }



        public virtual int dispatchMouseDown(MouseEventArgs args, int x, int y)
        {
            return _delegateMouseEvent(args, x, y, _mouseDownEvent, (sprite, args2, x2, y2)=> sprite.dispatchMouseDown(args2, x2, y2));
        }

        public virtual int dispatchMouseMove(MouseEventArgs args, int x, int y)
        {
            return _delegateMouseEvent(args, x, y, _mouseMoveEvent, (sprite, args2, x2, y2) => sprite.dispatchMouseMove(args2, x2, y2));
        }

        public virtual int dispatchMouseUp(MouseEventArgs args, int x, int y)
        {
            return _delegateMouseEvent(args, x, y, _mouseUpEvent, (sprite, args2, x2, y2) => sprite.dispatchMouseUp(args2, x2, y2));
        }

        public virtual int dispatchMouseClick(MouseEventArgs args, int x, int y)
        {
            return _delegateMouseEvent(args, x, y, _mouseClickEvent, (sprite, args2, x2, y2) => sprite.dispatchMouseClick(args2, x2, y2));
        }

        public delegate int DispathChildMouseEventCallback(Sprite sprite, MouseEventArgs args, int x, int y);

        delegate void DispatchMouseEvent( MouseEventArgs args, int x, int y);

        private void _mouseDownEvent(MouseEventArgs args, int x, int y)
        {
            if (mouseDown != null)
            {
                MouseEvent evt = new MouseEvent(args);
                evt.target = this;

                evt.x = x;
                evt.y = y;

                mouseDown(evt);
            }


        }

        private void _mouseUpEvent(MouseEventArgs args, int x, int y)
        {
            if (mouseUp != null)
            {
                MouseEvent evt = new MouseEvent(args);
                evt.target = this;

                evt.x = x;
                evt.y = y;

                mouseUp(evt);
            }
        }

        private void _mouseMoveEvent(MouseEventArgs args, int x, int y)
        {
            if (mouseMove != null)
            {
                MouseEvent evt = new MouseEvent(args);
                evt.target = this;

                evt.x = x;
                evt.y = y;

                mouseMove(evt);
            }
        }

        private void _mouseClickEvent(MouseEventArgs args, int x, int y)
        {
            if (mouseClick != null)
            {
                MouseEvent evt = new MouseEvent(args);
                evt.target = this;

                evt.x = x;
                evt.y = y;

                mouseClick(evt);
            }
        }


        private int _delegateMouseEvent(MouseEventArgs args, int x, int y, DispatchMouseEvent dispathCallback, DispathChildMouseEventCallback childEventCallback)
        {
            int rtn = 1;

           // Console.WriteLine("_delegateMouseEvent pt={0},{1}", x, y);

            if (bounds.Contains(x, y))
            {
                dispathCallback(args, x, y);
                rtn = 0;  
            }


            if (_children.Count > 0)
            {
                int x2 = x - this.x;
                int y2 = y - this.y;

                for (int i = _children.Count - 1; i >= 0; --i)
                {
                    var child = _children[i];

                    if(child.visibility == EVisibility.HitTestInvisible)
                    {
                        continue;
                    }

                    if(childEventCallback(_children[i], args, x2, y2) == 0)
                  //  if (_children[i].onMouseClick(args, x2, y2) == 0)
                    {
                        rtn = 0;
                        return 0;
                    }
                }
            }

            return rtn;
        }
        

        public virtual void drawGDI(System.Drawing.Graphics g)
        {
            for(int i=0; i<_children.Count; ++i)
            {
                _children[i].drawGDI(g);
            }
        }

        public virtual System.Drawing.Rectangle drawBounds
        {
            get
            {
                return new System.Drawing.Rectangle(-width/2, -height/2, width, height);
            }
        }

        public virtual void draw(DrawArgs args)
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

                if (_tex != null)
                {
                    var globalPt = getGlobalPt();

                    
                    //var drawHitBounds = this.getDrawHitBounds();
                    //drawHitBounds.X = (int)globalPt.X + drawHitBounds.X;
                    //drawHitBounds.Y = (int)globalPt.Y + drawHitBounds.Y;

                   // bool isDraw = args.viewportBounds.IntersectsWith(drawHitBounds);
                    
                   bool isDraw = true;
                    /*
                    if(isDraw == false)
                    {
                        Console.WriteLine("NODRAW {0} {1}", this, drawBounds);
                    }*/

                    if (isDraw)
                    {
                        _tex.tex.Bind(gl);
                        gl.Color(1.0f, 1.0f, 1.0f, this.alpha);

                        gl.Enable(OpenGL.GL_BLEND);
                        gl.Enable(OpenGL.GL_TEXTURE_2D);
                        gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
                        gl.DepthFunc(OpenGL.GL_LEQUAL);

                        gl.Begin(OpenGL.GL_QUADS);
                        {

                            var drawBounds = this.drawBounds;
                            int x2 = drawBounds.X + drawBounds.Width;
                            int y2 = drawBounds.Y + drawBounds.Height;

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
                        gl.Disable(OpenGL.GL_BLEND);
                        gl.Disable(OpenGL.GL_TEXTURE_2D);
                    }
                }

                for (int i = 0; i < _children.Count; ++i)
                {
                    _children[i].draw(args);
                }

                gl.PopMatrix();
                gl.PopMatrix();
            }
         
        }


    }

}
