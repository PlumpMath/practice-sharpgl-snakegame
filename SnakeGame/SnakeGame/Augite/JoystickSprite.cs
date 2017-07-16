using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{


    using Augite.Events;

    class JoystickSprite :Sprite
    {

        public delegate void JoystickEventCallback(JoystickEvent evt);
        public event JoystickEventCallback movementEvent;
        private bool _isPress = false;
        private Sprite _sprite;
        private Sprite _innerCircleSprite;
        private Sprite _pointSprite;
        private Sprite _container;

        public JoystickSprite(Game game)
            :base(game)
        {

            this.width = 160;
            this.height = 160;

            _container = game.newSprite<Sprite>();
            addChild(_container);

            _sprite = game.newSprite<Sprite>(createTex(width, height));
            _innerCircleSprite = game.newSprite<Sprite>(createTex(128, 128));
            _pointSprite = game.newSprite<Sprite>(createPointTex());

            _innerCircleSprite.visibility = EVisibility.HitTestInvisible;
            _pointSprite.visibility = EVisibility.HitTestInvisible;

            _container.addChild(_sprite);
            _container.addChild(_innerCircleSprite);
            _container.addChild(_pointSprite);


            _sprite.x = 0;
            _sprite.y = 0;
            _innerCircleSprite.x = 0;
            _innerCircleSprite.y = 0;
            _pointSprite.x = 0;
            _pointSprite.y = 0;

            _sprite.mouseDown += JoystickSprite_mouseDown;
            _sprite.mouseMove += JoystickSprite_mouseMove;
            //_sprite.mouseUp += JoystickSprite_mouseUp;

            //this.mouseDown += JoystickSprite_mouseDown;
            //this.mouseMove += JoystickSprite_mouseMove;
            this.mouseUp += JoystickSprite_mouseUp;

            game.stage.mouseUp += Stage_mouseUp;
            game.stage.mouseMove += Stage_mouseMove;

            game.gameUpdateEvent += Game_gameUpdateEvent;
        }

     

        private TimeSpan _triggerExpire = TimeSpan.MinValue;

        private void Game_gameUpdateEvent(GameTime gameTime)
        {
            if (_isTrigger)
            {
                if(gameTime.TotalGameTime > _triggerExpire)
                {
                    _triggerExpire = gameTime.TotalGameTime + TimeSpan.FromSeconds(0.025f);

                   // Console.WriteLine(">> {0} TRIGGER", this );

                    if (movementEvent != null)
                    {
                        movementEvent(new JoystickEvent() { angle = _lastAngle });
                    }
                }
            }
        }

        private void _mouseUp()
        {
            _isPress = false;
            _isTrigger = false;
            _pointSprite.x = 0;
            _pointSprite.y = 0;
        }


        public override int dispatchMouseDown(MouseEventArgs args, int x, int y)
        {
            //Console.WriteLine("dispatchMouseDown");
            return base.dispatchMouseDown(args, x, y);
        }

        private void Stage_mouseUp(Augite.Events.MouseEvent evt)
        {
         
           // Console.WriteLine("Stage_mouseUp");
            _mouseUp();
        }

        private void JoystickSprite_mouseUp(Augite.Events.MouseEvent evt)
        {
            _mouseUp();
        }

        private float _lastAngle;
        private bool _isTrigger = false;

        private void Stage_mouseMove(MouseEvent evt)
        {
            if (_isPress)
            {
                //Console.WriteLine("Stage_mouseMove {0}", evt);
                var pt2 = new System.Drawing.Point(evt.x, evt.y);
                var gPt = _sprite.getGlobalPt();
                pt2.X -= (int)gPt.X;
                pt2.Y -= (int)gPt.Y;

                _onMouseMove(pt2);
            }
                
        }

        private void JoystickSprite_mouseMove(Augite.Events.MouseEvent evt)
        {
            if (_isPress)
            {
               // Console.WriteLine("JoystickSprite_mouseMove {0}", evt);
                var pt2 = new System.Drawing.Point(evt.x, evt.y);
                _onMouseMove(pt2);
            }
               
        }

        private void _onMouseMove(System.Drawing.Point pt2)
        { 
       
            if (_isPress)
            {
                
                var pt1 = new System.Drawing.Point(_innerCircleSprite.x, _innerCircleSprite.y);
              

                float dist = (float)(Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y  - pt1.Y, 2)));
                float angle = (float)(Math.Atan2(pt2.Y - pt1.Y, pt2.X - pt1.X) * 180.0f / Math.PI) -90 ;

                if(dist > 64)
                {
                    dist = 64; 
                }

                var transform = new System.Drawing.Drawing2D.Matrix();
                transform.Rotate(angle);

                var pts = new System.Drawing.PointF[] { new System.Drawing.PointF(0, dist) };
                transform.TransformPoints(pts);

                var pt = pts[0];

                _pointSprite.x = (int)pt.X;
                _pointSprite.y = (int)pt.Y;

                //Console.WriteLine("JoystickSprite_mouseMove angle={0} dist={1} evt={2}", angle, dist, evt);

                _lastAngle = angle;
                _isTrigger = true;
                if (movementEvent != null)
                {
                    movementEvent(new JoystickEvent() { angle = angle });
                }
            }
        }

        private void JoystickSprite_mouseDown(Augite.Events.MouseEvent evt)
        {
            //Console.WriteLine("JoystickSprite_mouseDown");
            _isPress = true;
          
        }

        private Texture2D createPointTex()
        {
            var bmp = new System.Drawing.Bitmap(64, 64);

            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(51, 255, 255, 255));

                g.FillEllipse(brush, new System.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height));
            }

            return game.graphicsDevice.texFromBitmap(bmp);
        }

        private Texture2D createTex(int w, int h)
        {
            var bmp = new System.Drawing.Bitmap(w, h);

            using(var g = System.Drawing.Graphics.FromImage(bmp))
            {
               // g.Clear(System.Drawing.Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(51, 0, 0, 0));

                g.FillEllipse(brush, new System.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height));
            }

            return game.graphicsDevice.texFromBitmap(bmp);
        }


    }
}
