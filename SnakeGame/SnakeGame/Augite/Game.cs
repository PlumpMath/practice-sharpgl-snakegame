using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{

    using SharpGL;
    using Augite.Events;

    class Game : IDisposable
    {
        public delegate void GameUpdateEvent(GameTime gameTime);
        public delegate void KeyPressEvent(KeyEvent evt);

        public event GameUpdateEvent gameUpdateEvent;        
        public event KeyPressEvent keyPressEvent;
        public event KeyPressEvent keyDownEvent;

        public static Random random = new Random();

        private GameWindow _gameWindow;
        private GraphicsDeviceGL _graphicsDevice;
        private _Stage _stage;
        private GameTime _gameTime;
        private DateTime _lastGameTime;

        public GameTime gameTime { get { return _gameTime; } }

        public Game()
        {
            _graphicsDevice = new GraphicsDeviceGL(this);
            _gameWindow = new GLGameWindow(this, _graphicsDevice);
            _stage = new _Stage(this);
            _gameTime = new GameTime();
            _gameTime.TotalGameTime = TimeSpan.Zero;
            _lastGameTime = DateTime.Now;

        }

        public void onResize(System.Drawing.Size size)
        {
            _stage.resize(size.Width, size.Height);
        }

        public T newSprite<T>()
        {
            T inst = (T)Activator.CreateInstance(typeof(T), this);

            return inst;
        }

        public T newSprite<T>(object arg1)
        {
            T inst = (T)Activator.CreateInstance(typeof(T), this, arg1);

            return inst;
        }

        public T newSprite<T>(params object[] args)
        {
            var objs = new List<object>();
            objs.Add(this);
            objs.AddRange(args);

            T inst = (T)Activator.CreateInstance(typeof(T), objs.ToArray());

            return inst;
        }

        public GameWindow gameWindow
        {
            get
            {
                return _gameWindow;
            }
        }

        public Stage stage
        {
            get
            {
                return _stage;
            }
        }


        public GraphicsDevice graphicsDevice
        {
            get
            {
                return _graphicsDevice;
            }
        }


        public void run()
        {
            _gameWindow.run();
        }


        public virtual void onKeyDown(System.Windows.Forms.Keys keyCode)
        {
            if (keyDownEvent != null)
            {
                keyDownEvent(new KeyEvent()
                {
                    keyCode = keyCode
                });
            }
        }

        public virtual void onKeyPress(System.Windows.Forms.Keys keyCode)
        {
            if (keyPressEvent != null)
            {
                keyPressEvent(new KeyEvent()
                {
                    keyCode = keyCode
                });
            }
        }

        public virtual void onMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            MouseEventArgs args = new MouseEventArgs()
            {
                stageX = x,
                stageY = y,
                stage = _stage,
                clientSize = new System.Drawing.Size(_gameWindow.width, _gameWindow.height),
            };

            _stage.dispatchMouseDown(args, x, y);
        }

        public virtual void onMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            MouseEventArgs args = new MouseEventArgs()
            {
                stageX = x,
                stageY = y,
                stage = _stage,
                clientSize = new System.Drawing.Size(_gameWindow.width, _gameWindow.height),
            };

            _stage.dispatchMouseMove(args, x, y);
        }

        public virtual void onMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            MouseEventArgs args = new MouseEventArgs()
            {
                stageX = x,
                stageY = y,
                stage = _stage,
                clientSize = new System.Drawing.Size(_gameWindow.width, _gameWindow.height),
            };

            _stage.dispatchMouseUp(args, x, y);


        }

        public virtual void onMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            MouseEventArgs args = new MouseEventArgs()
            {
                stageX = x,
                stageY = y,
                stage = _stage,
                clientSize = new System.Drawing.Size(_gameWindow.width, _gameWindow.height),
            };

            _stage.dispatchMouseClick(args, x, y);
        }

        public virtual void init()
        {
        }


        public virtual void drawGDI(System.Drawing.Graphics g)
        {
            _stage.drawGDI(g);

            //g.DrawString(string.Format("DRAW DELTA={0}", _drawExecDelta), System.Drawing.SystemFonts.DefaultFont, System.Drawing.Brushes.Red, new System.Drawing.Point(10, 10));
        }

        TimeSpan _drawExecDelta;

        public void draw(OpenGL gl)
        {

           
            var now = DateTime.Now;
            var delta = now - _lastGameTime;
            _lastGameTime = now;
            _gameTime.TotalGameTime += delta;


            var t2 = DateTime.Now;

            if (gameUpdateEvent != null)
            {
                try
                {
                    gameUpdateEvent(_gameTime);
                } catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

               
            }

            var updateDelta = DateTime.Now - t2;

           // Console.WriteLine("GAME UPDATE DELTA={0}", updateDelta);

            Tweener.get().update(_gameTime);


            var t1 = DateTime.Now;
           
           var viewportBounds = new System.Drawing.RectangleF(0, 0, gameWindow.width, gameWindow.height);

            DrawArgs args = new DrawArgs()
            {
                gl = gl,
                viewportBounds = viewportBounds,
            };


            _stage.draw(args);

            _drawExecDelta = DateTime.Now - t1;

            //Console.WriteLine("GAME DRAW DELTA={0}", drawDelta);
        }


        public void Dispose()
        {
        }

        class _Stage : Stage
        {
            public override event MouseClickEvent mouseUp;
            public override event MouseClickEvent mouseMove;
            private Game _game;
            private Sprite _container;

            public _Stage(Game game)
            {
                _game = game;
                _container = _game.newSprite<Sprite>();
            }

            public void resize(int width, int height)
            {
                _container.width = width;
                _container.height = height;
            }

            public virtual int dispatchMouseClick(MouseEventArgs args, int x, int y)
            {
                return _container.dispatchMouseClick(args, x, y);
            }

            public virtual int dispatchMouseDown(MouseEventArgs args, int x, int y)
            {
                return _container.dispatchMouseDown(args, x, y);
            }

            public virtual int dispatchMouseMove(MouseEventArgs args, int x, int y)
            {
                if (mouseMove != null)
                {
                    MouseEvent evt = new MouseEvent(args);
                    evt.target = this;

                    evt.x = x;
                    evt.y = y;

                    mouseMove(evt);
                }              

                return _container.dispatchMouseMove(args, x, y);
            }




            public virtual int dispatchMouseUp(MouseEventArgs args, int x, int y)
            {

                if (mouseUp != null)
                {
                    MouseEvent evt = new MouseEvent(args);
                    evt.target = this;

                    evt.x = x;
                    evt.y = y;

                    mouseUp(evt);
                }


                int rtn = _container.dispatchMouseUp(args, x, y);              

                return rtn;
            }


            public override void addChild(Sprite sprite)
            {
                _container.addChild(sprite);
            }

            public override List<Sprite> children { get { return _container.children; } }

            public void draw(DrawArgs args)
            {
                _container.draw(args);
            }

            public void drawGDI(System.Drawing.Graphics g)
            {
                _container.drawGDI(g);
            }
        }
        

    }


}
