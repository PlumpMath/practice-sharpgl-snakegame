using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using Augite;

    class SnakeNPC
    {
        public SnakeSprite snake;
        private GamePlay _gamePlay;

        private Pt _gotoPt;
        private System.Drawing.Rectangle _gotoBounds;
        private System.Drawing.Drawing2D.Matrix _transform;
        private UpdateWork _updateWork = null;
        private TimeSpan _updateExpire = TimeSpan.MinValue;

        public SnakeNPC(GamePlay gamePlay, SnakeSprite snake)
        {
            this.snake = snake;
            _gamePlay = gamePlay;
            _transform = new System.Drawing.Drawing2D.Matrix();

            _ignorePreventCollisionIds = new List<int>() { snake.id };
        }

        public override string ToString()
        {
            return string.Format("<SnakeNPC id={0}>", 0);
        }

        private FoodFindResult _findFoodPt()
        {

            var hitRect = new System.Drawing.Rectangle(snake.x-128, snake.y-128, 256, 256);

            var foodResults = new List<FoodFindOrderResult>();

            foreach(FoodSprite food in _gamePlay.scene.iterFoodsWithHitRect(hitRect))
            {
                double dist = Math.Sqrt(Math.Pow(snake.y - food.y, 2) + Math.Pow(snake.x - food.x, 2));
                var result = new FoodFindOrderResult()
                {
                    food = food,
                    dist = dist,
                };

                foodResults.Add(result);
            }
       

            if(foodResults.Count > 0)
            {
                foodResults = (from x in foodResults orderby x.dist select x).ToList();
                var first = foodResults.First();

                return new FoodFindResult() { foodId = first.food.id, x=first.food.x, y=first.food.y};
            }

            return null;
        }

        class FoodFindOrderResult
        {
            public FoodSprite food;
            public double dist;

            public override string ToString()
            {
                return string.Format("<FoodFindOrderResult snake={0} dist={1}>", food.id, dist);
            }
        }

        class FoodFindResult
        {
            public int foodId;
            public int x;
            public int y;


            public override string ToString()
            {
                return string.Format("<FoodFindResult snake={0}>", foodId);
            }
        }


        private System.Drawing.Point _randomGotoPt()
        {
            _transform.Reset();
            _transform.Rotate(Game.random.Next(0, 360));

            var pts = new System.Drawing.Point[] { new System.Drawing.Point(0, 96) };
            _transform.TransformPoints(pts);

            var pt = pts[0];

            return new System.Drawing.Point(snake.x + pt.X, snake.y + pt.Y);
        }

        class UpdateWork : IWork
        {

            private SnakeNPC _npc;

            public UpdateWork(SnakeNPC npc)
            {
                _npc = npc;
            }


            public void run(GameTime gameTime)
            {
                //Console.WriteLine("UpdateWork {0}", _npc.snake);
                _npc._doUpdateWork(gameTime);              
               
            }
        }      

        public void update(GameTime gameTime)
        {
            if (_updateWork == null)
            {
                GameMain gameMain = (GameMain)_gamePlay.game;
                _updateWork = new UpdateWork(this);
                gameMain.putWork(_updateWork);
            }
        }

        FoodFindResult _findFoodResult;
        TimeSpan _gotoExpire = TimeSpan.Zero;
        TimeSpan _collisionCheckExpire = TimeSpan.Zero;
        private List<int> _ignorePreventCollisionIds;

        private void _doUpdateWork(GameTime gameTime)
        {          

            if (_gotoPt == null)
            {
                if(_hasCollision)
                {
                    _findFoodResult = null;
                    _gotoPt = _calcPreventPt();
                    _hasCollision = false;
                }

                if (_gotoPt == null)
                {
                    _findFoodResult = _findFoodPt();

                    if (_findFoodResult != null)
                    {
                        _gotoPt = new Pt()
                        {
                            x = _findFoodResult.x,
                            y = _findFoodResult.y,
                        };
                    }
                }
                
                ///Console.WriteLine("{0} foodPt={1}", snake, _gotoPt);
               
                if(_gotoPt == null)
                {
                    var randPt = _randomGotoPt();
                    _gotoPt = new Pt();
                    _gotoPt.x = randPt.X;
                    _gotoPt.y = randPt.Y;
                }

                
                updateGotoBounds();               
            }           

            if (_gotoPt != null)
            {
                var x1 = snake.x;
                var y1 = snake.y;
                var x2 = _gotoPt.x;
                var y2 = _gotoPt.y;

                float dist = (float)(Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)));
                float angle = (float)(Math.Atan2(y2 - y1, x2 - x1) * 180.0f / Math.PI) - 90;

                
                _transform.Reset();
                _transform.Rotate(angle);

                var pts = new System.Drawing.PointF[] { new System.Drawing.PointF(0, dist) };
                _transform.TransformPoints(pts);

                var pt = pts[0];

                snake.updateRotationWithRelatedPoint(pt);

                _updateExpire = gameTime.TotalGameTime + TimeSpan.FromSeconds(0.3f);
            }

            if (_gotoPt != null)
            {
                if (_gotoBounds.Contains(snake.x, snake.y))
                {
                    _gotoPt = null;
                    _updateExpire = TimeSpan.Zero;
                    //Console.WriteLine("{0} goto hit", this);
                }

                if(gameTime.TotalGameTime > _gotoExpire)
                {
                    _gotoPt = null;

                    //Console.WriteLine(">> {0} gameTime.TotalGameTime > _gotoExpire", snake);
                }
            }

            if (_findFoodResult != null)
            {
                if (_gamePlay.scene.foods.exists(_findFoodResult.foodId))
                {
                    _findFoodResult = null;
                    _gotoPt = null;
                }
            }

            if(gameTime.TotalGameTime > _collisionCheckExpire)
            {
                _collisionCheckExpire = gameTime.TotalGameTime + TimeSpan.FromSeconds(0.1f);
                
                if (_gamePlay.scene.hitTestCollision(snake.headPreventCollisionRect, _ignorePreventCollisionIds ))
                {
                    //Console.WriteLine("{0} find hitTestCollision", snake);

                    _gotoPt = null;
                    _hasCollision = true;
                    //_preventCollision();                  
                }
            }

            _updateWork = null;
        }

        private void updateGotoBounds()
        {
            _gotoExpire = _gamePlay.game.gameTime.TotalGameTime + TimeSpan.FromSeconds(3);
            _gotoBounds = new System.Drawing.Rectangle(_gotoPt.x - 24, _gotoPt.y - 24, 48, 48);
        }

      

        private bool _hasCollision = false;

        class PreventRectOrdering
        {
            public int x;
            public int y;
            public double weighted;

            public override string ToString()
            {
                return string.Format("<PreventRectOrdering {0},{1} weighted={2}>", x, y, weighted);
            }
        }


        private Pt _calcPreventPt()
        {
            // Console.WriteLine("{0} _preventCollision", snake);

            List<PreventRectOrdering> rectResult = new List<PreventRectOrdering>();

            var headPreventCollisionRect = snake.headPreventCollisionRect;

            foreach (var preventRect in snake.iterPreventRects())
            {
                bool isCollision = _gamePlay.scene.hitTestCollision(preventRect, _ignorePreventCollisionIds);
                //

                if(isCollision == false)
                {

                    double dist = Math.Sqrt(Math.Pow(preventRect.X - headPreventCollisionRect.X, 2) + Math.Pow(preventRect.Y-headPreventCollisionRect.Y, 2) );

                    double weighted = dist;

                    var item = new PreventRectOrdering()
                    {
                        x = preventRect.X,
                        y = preventRect.Y,
                        weighted = dist,
                    };

                    rectResult.Add(item);
                }
            }

            if(rectResult.Count > 0)
            {
              // Console.WriteLine("{0} FindPrevent={1}", snake, rectResult.Count);

                var rndResult = (from x in rectResult orderby x.weighted descending select x).First();

                return new Pt() { x = rndResult.x, y = rndResult.y };
            }

            return null;
        }

        class Pt
        {
            public int x;
            public int y;
        }
     }
}
