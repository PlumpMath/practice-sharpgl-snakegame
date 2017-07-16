using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using System.Drawing;
    using Augite;

    class SnakeSprite : Sprite
    {
        private GameMain _game;
        private Sprite _container;
        private Sprite _headContainer;
        private Sprite _bodyContainer;
        private List<Color> _colors;
        private Sprite _head;
        private Sprite _arrowSprite;
        private Sprite _arrowSprite2;
        private System.Drawing.Drawing2D.Matrix _rotTransform;
        private System.Drawing.PointF _speedDelta = new PointF(0, 0);
        private int _id;
        private System.Drawing.Rectangle _hitBounds;
        private System.Drawing.PointF _moveDelta = new PointF(0, 0);

        private float _x;
        private float _y;
        private BodyUpdate _bodyUpdate;
        private float _rotation2 = 0;
        private float _baseMoveSpeedValue;
        private float _moveSpeedValue;
        private System.Drawing.Size _baseJointSize;
        private System.Drawing.Size _jointSize;

        public int id
        {
            get
            {
                return _id;
            }
        }

        private TimeSpan _spawnTime;
        public TimeSpan spawnTime
        {
            get
            {
                return _spawnTime;
            }
        }


        public SnakeSprite(GameMain game, int id) : base(game)
        {
            _id = id;
            _game = game;
            _spawnTime = game.gameTime.TotalGameTime;
            _hitBounds = new Rectangle();
            _container = game.newSprite<Sprite>();

            addChild(_container);

            _colors = new List<Color>();
           

            _baseJointSize = new Size(12, 12);
            _jointSize = _baseJointSize;

            _baseMoveSpeedValue = 3.0f;
            _moveSpeedValue = _baseMoveSpeedValue;

            _headContainer = game.newSprite<Sprite>();
            _bodyContainer = game.newSprite<Sprite>();
            _container.addChild(_bodyContainer);
            _container.addChild(_headContainer);
            _bodyUpdate = new BodyUpdate(this);
           
        }

        public System.Drawing.Rectangle getHeadBounds()
        {
            return new System.Drawing.Rectangle(this.x + _head.x - _head.width/2, this.y + _head.y - _head.height / 2, _head.width, _head.height);
        }

        public System.Drawing.Rectangle getEatBounds()
        {
            return _eatBounds;
     
        }

        System.Drawing.Rectangle _eatBounds = new Rectangle();
        System.Drawing.Rectangle _eatMagnetBounds = new Rectangle();
        System.Drawing.Rectangle _headPreventCollisionRect = new Rectangle();

        public System.Drawing.Rectangle getEatMagnetBounds()
        {
            return _eatMagnetBounds;
        }


        public System.Drawing.Rectangle headPreventCollisionRect
        {
            get
            {
                return _headPreventCollisionRect;
            }
        }


        public List<Color> colors
        {
            get
            {
                return _colors;
            }           
        }


        public override string ToString()
        {
            return string.Format("<Snake id={0}>", _id);
        }

        public System.Drawing.Rectangle calcHitBounds()
        {
            return _hitBounds;
        }

        private int _lastBodyDelta = 0;

        public void addBodyLen()
        {
            if (_bodyUpdate.appendBodyMaxLen < 1024)
            {
                _bodyUpdate.appendBodyMaxLen++;

                int delta = _bodyUpdate.appendBodyMaxLen / 4;

                if(delta > 256)
                {
                    delta = 256;
                }
                
                if(delta != _lastBodyDelta)
                {
                    _lastBodyDelta = delta;                  

                    _moveSpeedValue = _baseMoveSpeedValue + delta / 8;

                    // Console.WriteLine("_moveSpeedValue={0} delta={1}", _moveSpeedValue, delta);

                    int joinSizeDelta = delta / 2;

                    _jointSize.Width = _baseJointSize.Width + joinSizeDelta;
                    _jointSize.Height = _baseJointSize.Height + joinSizeDelta;
                    _bodyUpdate.updateJointSize();
                }              
            }
        } 

        private void _updateRotTransform()
        {
            _rotTransform = new System.Drawing.Drawing2D.Matrix();
            _rotTransform.Rotate(_head.rotation);

            var pts = new System.Drawing.PointF[] { new System.Drawing.PointF(0, _moveSpeedValue) };
            _rotTransform.TransformPoints(pts);

            var pt = pts[0];
            _speedDelta.X = pt.X;
            _speedDelta.Y = pt.Y;
        }

        private System.Drawing.PointF _transRotPt(float angle, System.Drawing.PointF pt)
        {
            var trans1 = new System.Drawing.Drawing2D.Matrix();
            trans1.Rotate(angle);

            var pts1 = new System.Drawing.PointF[] { pt };
            trans1.TransformPoints(pts1);

            return pts1[0];
        }


        private bool _calcTurnRot(System.Drawing.PointF pt2, out float rotDelta)
        {
            rotDelta = 0;

            float rotSpeed = 5.0f;
            float targetRot = _head.rotation;


            var pt1 = _transRotPt(targetRot, new PointF(0, 64));
            //var pt1 = new PointF(0, 0);          


            _arrowSprite.x = (int)pt1.X;
            _arrowSprite.y = (int)pt1.Y;

            _arrowSprite2.x = (int)pt2.X;
            _arrowSprite2.y = (int)pt2.Y;

            int pt2Angle = (int)(Math.Atan2(pt2.Y - pt1.Y, pt2.X - pt1.X) * 180.0f / Math.PI);

            int pt2Angle2 = pt2Angle;



            var angle2Delta = (targetRot - pt2Angle) % 360;

           // Console.WriteLine("angle2Delta={3} pt2Angle={0} pt2Angle2={1} rot={2}", pt2Angle, pt2Angle2, targetRot, angle2Delta);

            if (angle2Delta < 0)
            {
                if (angle2Delta < -270)
                {
                    rotDelta = -rotSpeed;
                    return true;
                }

                rotDelta = rotSpeed;
                return true;
            }

            if (angle2Delta > 0)
            {
                if (angle2Delta > 90)
                {
                    rotDelta = rotSpeed;
                    return true;
                }
                rotDelta = -rotSpeed;
                return true;
            }


            return false;
        }



        public void turn(EDirection newDir)
        {

            System.Drawing.PointF pt2 = new PointF(0, 0);
            switch (newDir)
            {
                case EDirection.Up:
                    pt2 = new PointF(0, -64);
                    break;
                case EDirection.Down:
                    pt2 = new PointF(0, 64);
                    break;

                case EDirection.Left:
                    pt2 = new PointF(-64, 0);
                    break;

                case EDirection.Right:
                    pt2 = new PointF(64, 0);
                    break;
            }

            updateRotationWithRelatedPoint(pt2);
        }

        public class PreventRectOrdering
        {
            public System.Drawing.Rectangle rect;
            public double dist;

            public override string ToString()
            {
                return string.Format("<PreventRectOrdering {0},{1} dist={2}>", rect.X, rect.Y, dist);
            }
        }


        public IEnumerable<PreventRectOrdering> iterPreventRectsOrdering2(System.Drawing.Rectangle rect)
        {
            foreach (var preventRect in iterPreventRects())
            {
                double dist = Math.Sqrt(Math.Pow(rect.Y - preventRect.Y , 2) - Math.Pow(rect.X - preventRect.X, 2));
               // var a = Point.Subtract(new Point(rect.X, rect.Y), new Point(preventRect.X, preventRect.Y));
                var item = new PreventRectOrdering()
                {
                    rect = preventRect,
                    dist = dist,
                };

                yield return item;

            }
        }

        public IEnumerable<System.Drawing.Rectangle> iterPreventRects()
        {
            var transform = new System.Drawing.Drawing2D.Matrix();
            var pts = new System.Drawing.Point[1];

            int circleCount = 16;
            float angleDelta = 360 / circleCount;
            float angle = 0;
            for (int i = 0; i < circleCount; ++i)
            {

                transform.Reset();
                transform.Rotate(angle);

                pts[0] = new System.Drawing.Point(0, 96);

                transform.TransformPoints(pts);

                var pt = pts[0];

                var hitRect = new System.Drawing.Rectangle(this.x + pt.X - 48, this.y+ pt.Y - 48, 96, 96);
               // bool isCollision = _gamePlay.scene.hitTestCollision(snake.headPreventCollisionRect);

                //Console.WriteLine("{0} Prevent angle={1} hit={2}", snake, angle, isCollision);

                if(_headPreventCollisionRect.IntersectsWith(hitRect) == false)
                {
                    yield return hitRect;
                }

               

                angle += angleDelta;


            }

        }

        public void updateRotationWithRelatedPoint(System.Drawing.PointF pt)
        {
            float rotDelta;

            if (_calcTurnRot(pt, out rotDelta))
            {
                _rotation2 += rotDelta;
                int rotResult = (int)_rotation2;

                if (rotResult != 0)
                {
                    _rotation2 -= rotResult;
                    _head.rotation += rotResult;
                    _head.rotation = _head.rotation % 360;

                }

                _updateRotTransform();
            }
        }


        public System.Drawing.Point calcForwardPt()
        {
            float x1 = _x;
            float y1 = _y;
            int x3 = this.x;
            int y3 = this.y;

            for (int i=0; i<2;i++)
            {
                x1 += _speedDelta.X;
                y1 += _speedDelta.Y;

                int x2 = (int)_x;
                int y2 = (int)_y;

              

                if (x2 != 0)
                {
                    x1 -= x2;
                    x3 += x2;
                }

                if (y2 != 0)
                {
                    y1 -= y2;
                    y3 += y2;
                }
            }
            return new Point(x3, y3);

        }

        public void update()
        {
            //Console.WriteLine("_speedDelta {0}", _speedDelta);
            _moveDelta.X += _speedDelta.X;
            _moveDelta.Y += _speedDelta.Y;

            _x += _speedDelta.X;
            _y += _speedDelta.Y;


            int x2 = (int)_x;
            int y2 = (int)_y;

            if (x2 != 0)
            {
                _x -= x2;
                x += x2;
            }

            if (y2 != 0)
            {
                _y -= y2;
                y += y2;
            }

            _bodyUpdate.update();

            _eatBounds = new System.Drawing.Rectangle(this.x + _head.x - _head.width / 2 - 4, this.y + _head.y - _head.height / 2 - 4, _head.width+8, _head.height+8);
            _eatMagnetBounds = new System.Drawing.Rectangle(_eatBounds.X - 16, _eatBounds.Y - 16, _eatBounds.Width+32, _eatBounds.Height+32);

            float preventX = this.x + _head.x;
            float preventY = this.y + _head.y;
          

            for(int i=0; i<12; ++i)
            {
                preventX += _speedDelta.X;
                preventY += _speedDelta.Y;
            }

            _headPreventCollisionRect.X = (int)preventX - 50;
            _headPreventCollisionRect.Y = (int)preventY - 50;

            _headPreventCollisionRect.Width = 100;
            _headPreventCollisionRect.Height = 100;
            //_updateBody();
        }




        public IEnumerable<System.Drawing.Rectangle> iterBodyHitRects()
        {
            for(int i=0; i<_bodyContainer.children.Count;++i)
            {
                var body = _bodyContainer.children[i];
                var rect = new System.Drawing.Rectangle(this.x + body.x - body.width/2, this.y + body.y - body.height/2, body.width, body.height);

                yield return rect;
            }

            yield return new Rectangle(this.x + _head.x - _head.width / 2, this.y + _head.y - _head.height / 2, _head.width, _head.height);
        }


        class BodyUpdate
        {
            private SnakeSprite _snake;
            private List<Sprite> _bodyTempList;
            public int _appendBodyCurLen = 0;
            public int _appendBodyMaxLen = 0;

            private List<float> _bodyRotations = new List<float>();
            private List<System.Drawing.Point> _bodyPositions = new List<System.Drawing.Point>();

            private List<System.Drawing.Point> _prevPts;
            private int _appendBodyCounter = 0;

            public int appendBodyMaxLen
            {
                get
                {
                    return _appendBodyMaxLen;
                }
                set
                {
                    _appendBodyMaxLen = value;
                }
            }

            public BodyUpdate(SnakeSprite snake)
            {
                _snake = snake;
                _bodyTempList = new List<Sprite>();
                _prevPts = new List<Point>();
                _appendBodyCounter = 0;
            }

            public void updateJointSize()
            {
                var _jointSize = _snake._jointSize;
                _snake._head.width = _jointSize.Width;
                _snake._head.height = _jointSize.Height;

                Sprite bodySprite;

                for (int i = 0; i < _snake._bodyContainer.children.Count; ++i)
                {
                    bodySprite = _snake._bodyContainer.children[i];
                    bodySprite.width = _jointSize.Width;
                    bodySprite.height = _jointSize.Height;
                }

                for (int i = 0; i < _bodyTempList.Count; i++)
                {
                    bodySprite = _bodyTempList[i];
                    bodySprite.width = _jointSize.Width;
                    bodySprite.height = _jointSize.Height;
                }
            }

           

            public void update()
            {
                if(_appendBodyCurLen < _appendBodyMaxLen)
                {
                    _appendBodyCounter++;
                    if (_appendBodyCounter > 8)
                    {
                        _appendBodyCounter = 0;
                        var body = _snake._createBodySprite();
                        _bodyTempList.Add(body);
                        _appendBodyCurLen++;
                    }
                }

                appendBody();
                _updatePosition();
            }


            private System.Drawing.Rectangle _updateBounds = new Rectangle();

            private void _updatePosition()
            {
                _updateBounds.X = 0;
                _updateBounds.Y = 0;
                _updateBounds.Width = 0;
                _updateBounds.Height = 0;

                if (_prevPts.Count > 2)
                {
                    var prevPtStartIndex = _prevPts.Count - 2;
                    int ptIdx = prevPtStartIndex;
                    for (int i= _snake._bodyContainer.children.Count-1; i >= 0; i--)
                    {                    
                        //int ptIdx = prevPtStartIndex - (int)(i);

                        if (ptIdx < 0)
                        {
                            break;
                        }

                        var prevPt = _prevPts[ptIdx];

                        var bodySprite = _snake._bodyContainer.children[i];
                        int prevPtX = prevPt.X - _snake.x;
                        int prevPtY = prevPt.Y - _snake.y;

                        //Console.WriteLine("prevPtX={0},{1}", prevPtX, prevPtY);

                        bodySprite.x = prevPtX;
                        bodySprite.y = prevPtY;


                        _updateBounds = System.Drawing.Rectangle.Union(_updateBounds, bodySprite.bounds);

                        ptIdx -= 2;
                    }
                }

                int headX = _snake.x + _snake._head.x;
                int headY = _snake.y + _snake._head.y;

               // Console.WriteLine("headX={0},{1}", headX, headY);               
                _prevPts.Add(new Point(headX, headY));

                if(_prevPts.Count > 2048)
                {
                    _prevPts.RemoveRange(0, 1024);
                }

                _updateBounds.X += _snake.x;
                _updateBounds.Y += _snake.y;

                _snake._hitBounds = _updateBounds;
            }

            public GameMain game
            {
                get
                {
                    return _snake._game;
                }
            }

            public void init()
            {
                _bodyTempList.Clear();
                for (int i = 0; i < 16; ++i)
                {
                    var body = _snake._createBodySprite();
                    _snake._bodyContainer.addChild(body);
                   
                }
            }

            public void appendBody()
            {
                if (_bodyTempList.Count > 0)
                {
                    var body = _bodyTempList[0];
                    _bodyTempList.RemoveAt(0);

                    int idx = _snake._bodyContainer.children.Count;
                    _snake._bodyContainer.children.Insert(0, body);
                    body.parent = _snake._bodyContainer;
                    //_bodyRotations.Insert(0, _lastAppendRot);
                }
            }            
        }

  
        private Sprite _createBodySprite()
        {
           
            var body = game.newSprite<Sprite>(TextureManager.sharedInst.getSnakeBodyOrCreateWithColor(_getNextColor()));
    
            body.width = _jointSize.Width;
            body.height = _jointSize.Height;
            return body;
        }

        private int _colorOffset = 0;


        private Color _getNextColor()
        {
            if (_colors.Count < 1)
            {
                _colors.Add(Color.FromArgb(255, 108, 187, 87));
            }
            _colorOffset = (_colorOffset + 1) % _colors.Count;

            return _colors[_colorOffset];
        }


        public void setup()
        {

            _headContainer.clear();
            _bodyContainer.clear();

            _init();

            _head = game.newSprite<Sprite>(TextureManager.sharedInst.getSnakeHeadOrCreateWithColor(_getNextColor()));
            _container.addChild(_head);

            _arrowSprite = game.newSprite<Sprite>(TextureManager.sharedInst.getSnakeBodyOrCreateWithColor(Color.Red));
            _arrowSprite2 = game.newSprite<Sprite>(TextureManager.sharedInst.getSnakeBodyOrCreateWithColor(Color.Red));
            //_container.addChild(_arrowSprite);
            // _container.addChild(_arrowSprite2);      

            _updateRotTransform();
            _bodyUpdate.init();
            _bodyUpdate.updateJointSize();
        }

        private void _init()
        {
           
        }

        enum ERoundType
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
        }
    

        private ERoundType _calcRoundType(EDirection dir, EDirection dir2)
        {
            switch (dir)
            {
                case EDirection.Up:
                    switch (dir2)
                    {
                        case EDirection.Left:
                           return ERoundType.TopRight;
                        case EDirection.Right:
                           return ERoundType.TopLeft;
                    }
                    break;
                case EDirection.Down:
                    switch (dir2)
                    {
                        case EDirection.Left:
                            return ERoundType.BottomRight;
                        case EDirection.Right:
                            return ERoundType.BottomLeft;
                    }
                    break;
                case EDirection.Left:
                    switch (dir2)
                    {
                        case EDirection.Up:
                            return ERoundType.BottomLeft;
                        case EDirection.Down:
                            return ERoundType.TopLeft;
                    }
                    break;
                case EDirection.Right:
                    switch (dir2)
                    {
                        case EDirection.Up:
                            return ERoundType.BottomRight;
                        case EDirection.Down:
                            return ERoundType.TopRight;
                    }
                    break;
            }

            return ERoundType.None;
        }

     
    }
}
