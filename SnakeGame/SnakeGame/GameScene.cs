using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using System.Drawing;
    using Augite;
    using Augite.Events;

    class GameScene : Sprite
    {
        public delegate void ScoreAddEvent(SnakeSprite snake, int score);
        public delegate void SnakeDeadEvent(SnakeSprite snake);

        public event ScoreAddEvent scoreAddEvent;
        public event SnakeDeadEvent snakeDeadEvent;

        private GameMain _game;

        private Sprite _container;
        private Sprite _bgContainer;
        private Sprite _foodContainer;
        private Sprite _foodEatContainer;
        private Sprite _playerContainer;

        private EGameState _state;
        private Rectangle _boundRect;
        private Rectangle _gameRect;

        private Random _rand = new Random();
        private GameArgs _args;

        StringFormat _scoreStringFormat;
        Font _scoreFont;
        private int _lastGameObjId = 0;

        private SnakeCollisionUpdate _snakeCollisionUpdate;
        private FoodCollision _foodCollision;
        private bool _isDrawHitBounds = false;
        private bool _isDrawHitBodyBounds = false;
        private bool _isDrawFoodHitBounds = false;
        private bool _isDrawSnakePreventRects = false;

    
        private Dictionary<int, SnakeSprite> _snakeById = new Dictionary<int, SnakeSprite>();

        private _FoodList _foodList;

        public System.Drawing.Rectangle gameRect
        {
            get
            {
                return _gameRect;
            }
        }

        public int newGameObjId()
        {
            return ++_lastGameObjId;
        }

        public GameScene(GameMain game) : base(game)
        {
            _game = game;


            var sceneSize = new System.Drawing.Size(4096, 4096);

            _gameRect = new Rectangle(-sceneSize.Width/2, -sceneSize.Height/2, sceneSize.Width, sceneSize.Height);

            _bgContainer = game.newSprite<Sprite>();
            _container = game.newSprite<Sprite>();

            addChild(_bgContainer);        
            addChild(_container);

            _foodContainer = game.newSprite<Sprite>();
            _foodEatContainer = game.newSprite<Sprite>();            
            _playerContainer = game.newSprite<Sprite>();

          
            _container.addChild(_foodContainer);
            _container.addChild(_foodEatContainer);
            _container.addChild(_playerContainer);


            init();
           
            _initBg();

            _initCollisionGroups();
            _snakeCollisionUpdate = new SnakeCollisionUpdate(this);
            _foodList = new _FoodList(this);
            _foodCollision = new FoodCollision(this);

            this.game.gameUpdateEvent += Game_gameUpdateEvent;
        }

        public List<Sprite> players
        {
            get
            {
                return _playerContainer.children;
            }
        }

    
        private void _initBg()
        {
            _bgContainer.children.Clear();
            
            //var sprite = game.newSprite<TileRepeatSpriteNew>(TextureManager.sharedInst.floorBitmap);
            var sprite = game.newSprite<TileRepeatSprite>(TextureManager.sharedInst["bg_grid"]);
            sprite.x = _gameRect.X;
            sprite.y = _gameRect.Y;
            sprite.width = _gameRect.Width;
            sprite.height = _gameRect.Height;

            _bgContainer.addChild(sprite);

        }


        public override void drawGDI(Graphics g)
        {
            base.drawGDI(g);
            int x2 = this.x;
            int y2 = this.y;


            if (_isDrawFoodHitBounds)
            {
                for (int j = 0; j < _foodContainer.children.Count; ++j)
                {
                    var food = (_FoodSprite)_foodContainer.children[j];
                    var foodBounds = food.getHitBounds();

                    foodBounds.X = foodBounds.X + x2;
                    foodBounds.Y = foodBounds.Y + y2;
                    g.DrawRectangle(Pens.Orange, foodBounds);
                }

            }


            if (_isDrawHitBodyBounds)
            {
                for (int i = 0; i < _playerContainer.children.Count; ++i)
                {
                    var snake = (SnakeSprite)_playerContainer.children[i];
                    var bodyRects = snake.iterBodyHitRects().ToList();
                    for (int j = 0; j < bodyRects.Count; ++j)
                    {
                        var rect = bodyRects[j];
                        rect.X = rect.X + x2;
                        rect.Y = rect.Y + y2;
                        g.DrawRectangle(Pens.Red, rect);
                    }
                }
            }

            if(_isDrawSnakePreventRects)
            {
                for (int i = 0; i < _playerContainer.children.Count; ++i)
                {
                    var snake = (SnakeSprite)_playerContainer.children[i];

                    var headPreventCollisionRect = snake.headPreventCollisionRect;
                    headPreventCollisionRect.X = headPreventCollisionRect.X + x2;
                    headPreventCollisionRect.Y = headPreventCollisionRect.Y + y2;

                    g.DrawRectangle(Pens.White, headPreventCollisionRect);

                    //var rects = snake.iterPreventRects().ToList();
                    var items = snake.iterPreventRectsOrdering2(snake.headPreventCollisionRect).ToList();
                    for (int j = 0; j < items.Count; j++)
                    {
                        var item = items[j];
                        var rect = item.rect;

                        rect.X = rect.X + x2;
                        rect.Y = rect.Y + y2;

                        g.DrawRectangle(Pens.Yellow, rect);
                    } 

                    if(items.Count > 0)
                    {
                        var mostDist = (from x in items orderby x.dist descending select x).First();

                        var mostRect = mostDist.rect;
                        mostRect.X = mostRect.X + x2;
                        mostRect.Y = mostRect.Y + y2;

                        g.DrawRectangle(Pens.Blue, mostRect);
                    }

                   
                }
                    
            }

            if (_isDrawHitBounds)
            {
                for (int i = 0; i < _playerContainer.children.Count; ++i)
                {
                    var snake = (SnakeSprite)_playerContainer.children[i];


                    var bounds = snake.calcHitBounds();
                    var snakeHeadBounds = snake.getHeadBounds();

                    var snakeEatBounds = snake.getEatBounds();
                    var snakeEatMagnetBounds = snake.getEatMagnetBounds();
                   

                    bounds.X = bounds.X + x2;
                    bounds.Y = bounds.Y + y2;

                    snakeHeadBounds.X = snakeHeadBounds.X + x2;
                    snakeHeadBounds.Y = snakeHeadBounds.Y + y2;

                    snakeEatBounds.X = snakeEatBounds.X + x2;
                    snakeEatBounds.Y = snakeEatBounds.Y + y2;

                    snakeEatMagnetBounds.X = snakeEatMagnetBounds.X + x2;
                    snakeEatMagnetBounds.Y = snakeEatMagnetBounds.Y + y2;

          
                    g.DrawRectangle(Pens.White, bounds);

                    g.DrawRectangle(Pens.Blue, snakeHeadBounds);
                    g.DrawRectangle(Pens.Red, snakeEatBounds);
                    g.DrawRectangle(Pens.Green, snakeEatMagnetBounds);
       

                }
            }

            g.DrawString(string.Format("FOOD COUNT={0}", _foodContainer.children.Count), System.Drawing.SystemFonts.DefaultFont, Brushes.Green, new System.Drawing.Point(10, 50));
            g.DrawString(string.Format("COLLISION SNAKE={0}", _collisionSnakeDelta), System.Drawing.SystemFonts.DefaultFont, Brushes.Green, new System.Drawing.Point(10, 70));
            g.DrawString(string.Format("COLLISION SNAKE LOOP={0}", _snakeCollisionUpdate.totalLoopCounter), System.Drawing.SystemFonts.DefaultFont, Brushes.Green, new System.Drawing.Point(10, 90));
            g.DrawString(string.Format("COLLISION FOOD={0}", _collisionFoodDelta), System.Drawing.SystemFonts.DefaultFont, Brushes.Green, new System.Drawing.Point(10, 110));


           
        }

     


      

        public SnakeSprite newSnake()
        {
            var snake = game.newSprite<SnakeSprite>(newGameObjId());

            //_player.x = centerX;
            //_player.y = centerY;
            //_player.x = _rand.Next(centerX - 10, centerX + 20);
            //_player.y = _rand.Next(centerY - 10, centerY + 20);

            addPlayer(snake);
            return snake;
        }


        public void addPlayer(SnakeSprite snake)
        {
            _playerContainer.addChild(snake);

            _snakeById[snake.id] = snake;
        }


        private void Game_gameUpdateEvent(GameTime gameTime)
        {
            if (_state == EGameState.Start)
            {
                updateGameLogic(gameTime);
            }

        }

        public bool hitTestCollision(System.Drawing.Rectangle rect, List<int> ignoreIds)
        {
            if(!gameRect.Contains(rect.X, rect.Y))
            {
                return true;
            }
           
            if(_snakeCollisionUpdate.hitTestCollision(rect, ignoreIds))
            {
                return true;
            }

            return false;
        }

        public virtual void updateGameLogic(GameTime gameTime)
        {
            if (_checkGameOver())
            {
                _state = EGameState.GameOver;
                // _uiContainer.addChild(game.newSprite<GameOverSprite>());
                // game.addComponent(new GameOverComp());
                return;
            }           

            // Console.WriteLine("updateGameLogic");
            var t1 = DateTime.Now;
            for (int i = 0; i < _playerContainer.children.Count; ++i)
            {
                var snake = (SnakeSprite)_playerContainer.children[i];

                snake.update();
            }

            var delta1 = DateTime.Now - t1;


            if(gameTime.TotalGameTime > _removeOutsideGameRectFoodExpire)
            {
                _removeOutsideGameRectFoodExpire = gameTime.TotalGameTime + TimeSpan.FromSeconds(10.0f);

                int removeCounter = 0;
                for (int i = 0; i < foods.children.Count;)
                {
                    var food = (FoodSprite)foods.children[i];

                    var drawBounds = food.drawBounds;
                    drawBounds.X += food.x;
                    drawBounds.Y += food.y;
                    // Console.WriteLine("{0} drawBounds={1}", food, drawBounds);
                    if (gameRect.IntersectsWith(drawBounds) == false)
                    {
                        removeCounter++;
                       
                        _foodList.remove(food);
                        continue;
                    }
                    ++i;
                }

                Console.WriteLine("OutSide GameRect Foods={0}", removeCounter);
            }

            if(gameTime.TotalGameTime > _updateCollisionExpire)
            {

                _updateCollisionExpire = gameTime.TotalGameTime + TimeSpan.FromSeconds(0.2f);
                var t2 = DateTime.Now;


                updateCollision(gameTime);

                var delta2 = DateTime.Now - t2;

                // _updateViewportObjVisible(gameTime);
                // Console.WriteLine("updateGameLogic snake={0} collision={1}", delta1, delta2);

                _updateCollisionGroups(gameTime);

                if (_foodList.changedIds.Count > 0)
                {

                    //Console.WriteLine("changedIds count={0} [{1}]", _foodList.changedIds.Count, String.Join(", ", (from x in _foodList.changedIds select x.ToString()).ToArray()));

                    _foodList.changedIds.Clear();
                }

                if (_foodList.removeIds.Count > 0)
                {
                    _foodList.removeIds.Clear();
                }
            }

            //alpha anim
            _updateEated(gameTime);
            //
        }

        private TimeSpan _updateCollisionExpire = TimeSpan.Zero;
        private TimeSpan _removeOutsideGameRectFoodExpire = TimeSpan.Zero;

        public void visibleTestWithRect(System.Drawing.Rectangle viewportRect)
        {

            int visibleCount = 0;

            var t1 = DateTime.Now;

            for (int i = 0; i < foods.children.Count; ++i)
            {
                var food = foods.children[i];

                var drawBounds = food.drawBounds;
                drawBounds.X += food.x;
                drawBounds.Y += food.y;
                // Console.WriteLine("{0} drawBounds={1}", food, drawBounds);
                if (viewportRect.IntersectsWith(drawBounds))
                {
                    food.visible = true;
                    visibleCount++;
                }
                else
                {
                    food.visible = false;
                }
            }

            var delta = DateTime.Now - t1;

            for (int i = 0; i < _playerContainer.children.Count; ++i)
            {
                var snake = (SnakeSprite)_playerContainer.children[i];

                var drawBounds = snake.calcHitBounds();
                if(viewportRect.IntersectsWith(drawBounds))
                {
                    snake.visible = true;
                } else
                {
                    snake.visible = false;
                }
            }

            //Console.WriteLine("food {0} visibleCount={1}/{2}", delta, visibleCount, _scene.foods.children.Count);
        }


        private void _updateCollisionGroups(GameTime gameTime)
        {

            //Console.WriteLine("_updateCollisionGroups changedIds count={0}", _foodList.changedIds.Count);

            if (_foodList.changedIds.Count > 0)
            {
                foreach (int id in _foodList.changedIds)
                {
                    bool exists = _foodList.exists(id);

                   // Console.WriteLine("exists={0} = {1}", id, exists);

                    if (exists)
                    {
                        var food = (_FoodSprite)_foodList.getById(id);
                        var foodBounds = food.getHitBounds();

                        for (int i = 0; i < _collisionGroups.Count; ++i)
                        {
                            var g = (CollisionGroup)_collisionGroups[i];
                            if (g.rect.IntersectsWith(foodBounds))
                            {
                                if (!g.foodIds.Contains(food.id))
                                {
                                    g.foodIds.Add(food.id);
                                }

                            }
                        }
                    }
                }

            }

            if (_foodList.removeIds.Count > 0)
            {
                foreach (int id in _foodList.removeIds)
                {
                   // Console.WriteLine("RemoveFoodId={0}", id);

                }
            }
        }


        TimeSpan _collisionSnakeDelta = TimeSpan.Zero;
        TimeSpan _collisionFoodDelta = TimeSpan.Zero;


        

        public virtual void updateCollision(GameTime gameTime)
        {
            int testCount = _playerContainer.children.Count;
            int testCount2 = (from x in _playerContainer.children select ((SnakeSprite)x).id).Distinct().ToList().Count;


            if (testCount != testCount2)
            {
                throw new Exception("Repeat player!");
            }

            

            var t1 = DateTime.Now;

            _snakeCollisionUpdate.update(gameTime);
            _collisionSnakeDelta = DateTime.Now - t1;
            //Console.WriteLine("updateCollisionSnake={0}", delta1);

            var t2 = DateTime.Now;

            _foodCollision.update(gameTime);         

            _collisionFoodDelta = DateTime.Now - t2;
         
        }



        private IEnumerable<System.Drawing.Rectangle> _iterHitTestGroupRects()
        {
           
            for (int y = gameRect.Y; y < gameRect.Height;)
            {
                for (int x = gameRect.X; x < gameRect.Width;)
                {
                    var rect = new System.Drawing.Rectangle(x - 32, y - 32, 128 + 64, 128 + 64);

                    yield return rect;

                    x += 128;
                }

                y += 128;
            }
        }

        class SnakeCollisionUpdate
        {

            private GameScene _scene;

            public SnakeCollisionUpdate(GameScene scene)
            {
                _scene = scene;
                _groupRects = _iterGroupRects().ToList();               
            }


            class GroupSet
            {
                public System.Drawing.Rectangle bounds;
                public List<SnakeSprite> snakes = new List<SnakeSprite>();

                public override string ToString()
                {
                    return string.Format("<GroupSet snakes={0}>", snakes.Count);
                }
            }

            private List<System.Drawing.Rectangle> _groupRects;

            private IEnumerable<System.Drawing.Rectangle> _iterGroupRects()
            {
                var gameRect = _scene.gameRect;

                for (int y = gameRect.Y; y < gameRect.Height;)
                {
                    for (int x = gameRect.X; x < gameRect.Width;)
                    {
                        var rect = new System.Drawing.Rectangle(x-32, y-32, 128 + 64, 128 + 64);

                        yield return rect;

                        x += 128;
                    }

                    y += 128;
                }
            }

      
            private void dispatchDeadEvent(SnakeSprite snake)
            {
                if (_scene.snakeDeadEvent != null)
                {
                    _scene.snakeDeadEvent(snake);
                }
            }

            public int totalLoopCounter = 0;


            public bool hitTestCollision(System.Drawing.Rectangle rect, List<int> ignoreIds)
            {
                var gameTime = _scene.game.gameTime;
                var activeTime = gameTime.TotalGameTime + TimeSpan.FromSeconds(6);

                for (int j = 0; j < _hitGroups.Count; j++)
                {
                    var g = _hitGroups[j];

                    if (g.snakes.Count > 0)
                    {
                        if(g.bounds.IntersectsWith(rect))
                        {
                            for(int i=0; i<g.snakes.Count; ++i)
                            {
                                var snake = g.snakes[i];

                                if(ignoreIds != null)
                                {
                                    if (ignoreIds.Contains(snake.id))
                                    {
                                        continue;
                                    }
                                }

                                if(activeTime < snake.spawnTime)
                                {
                                    continue;
                                }
                                

                                foreach (var bodyRect in snake.iterBodyHitRects())
                                {
                                    if (bodyRect.IntersectsWith(rect))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                return false;
            }

            private List<GroupSet> _hitGroups = new List<GroupSet>();


            public void update(GameTime gameTime)
            {
             
               
                var _playerContainer = _scene._playerContainer;

                _hitGroups.Clear();

                var gameRect = _scene.gameRect;

                for (int i=0; i< _groupRects.Count; i++)
                {
                    var rect = _groupRects[i];
                    var group = new GroupSet();
                    group.bounds = rect;
                    _hitGroups.Add(group);

                }

                for (int i = 0; i < _playerContainer.children.Count;)
                {
                    var snake = (SnakeSprite)_playerContainer.children[i];                   

                    var snakeHeadBounds = snake.getHeadBounds();
                    //  Console.WriteLine("{0} snakeBounds={1}", snake, snakeBounds);

                    if (!_scene._gameRect.IntersectsWith(snakeHeadBounds))
                    {
                        _playerContainer.children.RemoveAt(i);

                        dispatchDeadEvent(snake);                       
                        continue;
                    }

                    for (int j=0; j< _hitGroups.Count;j ++)
                    {
                        var g = _hitGroups[j];

                        if(g.bounds.IntersectsWith(snake.calcHitBounds()))
                        {
                            g.snakes.Add(snake);
                        }
                    }

                    i++;
                }

                List<int> allDeadList = new List<int>();

                totalLoopCounter = 0;
                var activeTime = gameTime.TotalGameTime - TimeSpan.FromSeconds(3);

                for (int j = 0; j < _hitGroups.Count; j++)
                {
                    var g = _hitGroups[j];

                    if(g.snakes.Count > 0)
                    {
                        List<int> deadResultList = new List<int>();
                        int loopCounter;
                        hitTestList(activeTime, g.snakes, allDeadList, deadResultList, out loopCounter);

                        totalLoopCounter += loopCounter;

                        if (deadResultList.Count > 0)
                        {
                            string deadText = String.Join(",", (from x in deadResultList select x.ToString()).ToArray());

                          //  Console.WriteLine("GROUP loopCounter={3} snake={0} rect={1} deadList={2}", g.snakes.Count, g.bounds, deadText, loopCounter);


                            allDeadList.AddRange(deadResultList);
                        }                       
                    }                   
                }

              // Console.WriteLine("Snake totalLoopCounter={0} ", totalLoopCounter);
                if (allDeadList.Count > 0)
                {
                    allDeadList = allDeadList.Distinct().ToList();

                    for (int i = 0; i < _playerContainer.children.Count;)
                    {
                        var snake = (SnakeSprite)_playerContainer.children[i];

                        if (allDeadList.Contains(snake.id))
                        {
                            _playerContainer.children.RemoveAt(i);
                            _scene._snakeById.Remove(snake.id);
                            dispatchDeadEvent(snake);
                            continue;  
                        }

                        ++i;
                    }
                }

            }

            private void hitTestList(TimeSpan activeTime, List<SnakeSprite> snakes, List<int> allDeadList, List<int> deadResultList, out int loopCounter)
            {
                loopCounter = 0;
                for (int i = 0; i < snakes.Count; i++)
                {
                    var snake = snakes[i];

                    if (allDeadList.Contains(snake.id)) continue;
                    if (activeTime < snake.spawnTime) continue;

                    var snakeHeadBounds = snake.getHeadBounds();
                    //  Console.WriteLine("{0} snakeBounds={1}", snake, snakeBounds);
                    bool isHeadHit = false;

                    for (int j = 0; j < snakes.Count; ++j)
                    {
                        loopCounter++;

                        var snake2 = snakes[j];
                        if (allDeadList.Contains(snake2.id)) continue;

                        if (activeTime < snake2.spawnTime) continue;

                        if (snake.id != snake2.id)
                        {

                            var snake2Bounds = snake2.calcHitBounds();

                            if (snake2Bounds.IntersectsWith(snakeHeadBounds))
                            {
                                foreach (var bodyRect in snake2.iterBodyHitRects())
                                {
                                    if (bodyRect.IntersectsWith(snakeHeadBounds))
                                    {
                                        isHeadHit = true;
                                        break;
                                    }
                                }

                                if (isHeadHit)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (isHeadHit)
                    {
                        allDeadList.Add(snake.id);
                        deadResultList.Add(snake.id);
                    }
                }
            }     
        }

        class CollisionGroup
        {
            public System.Drawing.Rectangle rect;
            public List<int> foodIds = new List<int>();
            public List<int> snakeIds = new List<int>();
        }

        


        private List<CollisionGroup> _collisionGroups;

        private void _initCollisionGroups()
        {
            _collisionGroups = new List<CollisionGroup>();

            foreach (var rect in _iterCollisionGroupRects())
            {
                var g = new CollisionGroup();
                g.rect = rect;

                _collisionGroups.Add(g);
            }
        }

        private IEnumerable<System.Drawing.Rectangle> _iterCollisionGroupRects()
        {      

            for (int y = gameRect.Y; y < gameRect.Height;)
            {
                for (int x = gameRect.X; x < gameRect.Width;)
                {
                    var rect = new System.Drawing.Rectangle(x - 32, y - 32, 128 + 64, 128 + 64);

                    yield return rect;

                    x += 128;
                }

                y += 128;
            }
        }


        public IEnumerable<FoodSprite> iterFoodsWithHitRect(System.Drawing.Rectangle rect)
        {
            for (int i = 0; i < _collisionGroups.Count; ++i)
            {
                var g = (CollisionGroup)_collisionGroups[i];
                if (g.rect.IntersectsWith(rect))
                {
                    foreach(int foodId in g.foodIds)
                    {
                        if (_foodList.exists(foodId))
                        {
                            var food = _foodList.getById(foodId);
                            if(rect.Contains(food.x, food.y))
                            {
                                yield return food;
                            }
                        }
                    }
                }
            }
        }

        private void _updateEated(GameTime gameTime)
        {
            for (int i = 0; i < _foodEatContainer.children.Count;)
            {
                var food = (FoodSprite)_foodEatContainer.children[i];
                food.alpha -= 0.05f;

                if (food.alpha < 0)
                {
                    _foodEatContainer.children.RemoveAt(i);

                    if (_eatSnakeIdByFoodId.ContainsKey(food.id))
                    {
                        _eatSnakeIdByFoodId.Remove(food.id);
                    }

                    // Console.WriteLine("_foodEatContainer.children.RemoveAt(i);");
                    continue;
                }

                if (_eatSnakeIdByFoodId.ContainsKey(food.id))
                {
                    int snakeId = _eatSnakeIdByFoodId[food.id];
                    if (_snakeById.ContainsKey(snakeId))
                    {
                        var eatSnake = _snakeById[snakeId];
                        var snakeForwardPt = eatSnake.calcForwardPt();

                        // Console.WriteLine("_foodEatContainer {0} Tweener {1}", food, eatSnake);
                        /*
                         Tweener.add(food, 0.3f, 0.0f, Tweener.ETransition.Linear, new Dictionary<string, object>()
                                 {
                                    // {"x", snakeForwardPt.X },
                                    // {"y", snakeForwardPt.Y },
                                      {"x", eatSnake.x },
                                     {"y", eatSnake.y }
                                 });*/
                    }
                }

                i++;
            }
        }



        class FoodCollision
        {
            private GameScene _scene;

            public FoodCollision(GameScene scene)
            {
                _scene = scene;               
           }

            public void update(GameTime gameTime)
            {
                _updateCollision(gameTime);
               
            }

         
            private void _updateCollision(GameTime gameTime)
            {
                var groups = _scene._collisionGroups;

                List<int> _takeFoodIds = new List<int>();
                Dictionary<int, SnakeSprite> _eatSnakeByTakeFoodId = new Dictionary<int, SnakeSprite>();                

                for (int i = 0; i < _scene._playerContainer.children.Count; ++i)
                {
                    var snake = (SnakeSprite)_scene._playerContainer.children[i];

                    var snakeEatBounds = snake.getEatBounds();
                    var snakeEatMagnetBounds = snake.getEatMagnetBounds();
                    //var playerLargeBounds = new System.Drawing.Rectangle(_player.x - 200, _);

                    foreach (var group in groups)
                    {
                        if (group.rect.IntersectsWith(snakeEatBounds))
                        {
                            for (int j = 0; j < group.foodIds.Count;)
                            {
                                int foodId = group.foodIds[j];

                                if (_takeFoodIds.Contains(foodId))
                                {
                                    group.foodIds.RemoveAt(j);
                                    continue;
                                }

                                if(!_scene._foodList.exists(foodId))
                                {
                                    group.foodIds.RemoveAt(j);
                                    continue;
                                }

                                var food = (_FoodSprite)_scene._foodList.getById(foodId);
                                var foodBounds = food.getHitBounds();

                                if (snakeEatBounds.IntersectsWith(foodBounds))
                                {
                                    snake.addBodyLen();

                                    _takeFoodIds.Add(foodId);

                                    _eatSnakeByTakeFoodId[foodId] = snake;

                                    group.foodIds.RemoveAt(j);

                                    if (_scene.scoreAddEvent != null)
                                    {
                                        _scene.scoreAddEvent(snake, food.score);
                                    }
                                    continue;
                                }

                                j++;
                            }
                        }
                    }
                }

                //Console.WriteLine("_updateCollision _takeFoodIds={0}", _takeFoodIds.Count);


                for (int i=0; i< _takeFoodIds.Count; ++i)
                {
                    int foodId = _takeFoodIds[i];                    

                    if (_scene._foodList.exists(foodId))
                    {
                        var food = _scene._foodList.getById(foodId);
                        var eatSnake = _eatSnakeByTakeFoodId[foodId];

                        _scene._foodList.remove(food);
                        //Console.WriteLine("_scene._foodList.remove({0});", food.id);

                        food.alpha = 1.0f;
                        _scene._foodEatContainer.addChild(food);

                        if (eatSnake != null)
                        {
                            _scene._eatSnakeIdByFoodId[food.id] = eatSnake.id;

                            var snakeForwardPt = eatSnake.calcForwardPt();

                            // Console.WriteLine("_foodEatContainer {0} Tweener {1}", food, eatSnake);

                            Tweener.add(food, 0.3f, 0.0f, Tweener.ETransition.Linear, new Dictionary<string, object>()
                                {
                                    {"x", snakeForwardPt.X },
                                   {"y", snakeForwardPt.Y },
                                    // {"x", eatSnake.x },
                                   // {"y", eatSnake.y }
                                });

                        }
                    }
                }


            }


           
        }


        private Dictionary<int, int> _eatSnakeIdByFoodId = new Dictionary<int, int>();



        private void _initAsset()
        {
            FontFamily ff = _game.fontManager.messageFont;
            _scoreFont = new Font(ff, 20);

            _scoreStringFormat = new StringFormat();
            _scoreStringFormat.LineAlignment = StringAlignment.Center;
            _scoreStringFormat.Alignment = StringAlignment.Near;
        }


        class _FoodList : FoodList
        {

            private GameScene _scene;
            private HashSet<int> _changedIds = new HashSet<int>();
            private HashSet<int> _removeIds = new HashSet<int>();

            public HashSet<int> changedIds
            {
                get
                {
                    return _changedIds;
                }
            }

            public HashSet<int> removeIds
            {
                get
                {
                    return _removeIds;
                }
            }


            private Dictionary<int, _FoodSprite> _getById = new Dictionary<int, _FoodSprite>();

            public _FoodList(GameScene scene)
            {
                _scene = scene;
            }

            public FoodSprite spawn(int score)
            {
                int id = _scene.newGameObjId();
                _FoodSprite foodSprite = _scene.game.newSprite<_FoodSprite>(id, this, score);

                _scene._foodContainer.addChild(foodSprite);
                _changedIds.Add(id);

               // Console.WriteLine("SPAWN {0}, {1}", foodSprite.id, String.Join(", ", (from x in _changedIds select x.ToString()).ToArray()));

                _getById[id] = foodSprite;

                //Console.WriteLine("SPAWN {0}", foodSprite);

                return foodSprite;
            }

            public override List<Sprite> children
            {
                get
                {
                    return _scene._foodContainer.children;
                }
            }

            public override bool exists(int id)
            {
                return _getById.ContainsKey(id);
            }

            public FoodSprite getById(int id)
            {
                if(_getById.ContainsKey(id))
                {
                    return _getById[id];
                }
                return null;
            }

            public void remove(FoodSprite food)
            {
                //Console.WriteLine("remove food={0}", food.id);

                if(_getById.ContainsKey(food.id))
                {
                    _getById.Remove(food.id);
                }
               
                _scene._foodContainer.children.Remove(food);

                _removeIds.Add(food.id);
            }

        }


        public FoodSprite spawnFood(int score)
        {
            return _foodList.spawn(score);   
        }


        public FoodList foods { get { return _foodList; } }


        class _FoodSprite : FoodSprite
        {           
            public Sprite sprite;         
            private int _baseSize;

            public override float alpha
            {
                get
                {
                    return base.alpha;
                }
                set
                {
                    base.alpha = value;
                    sprite.alpha = base.alpha;
                }
            }

            public override int x
            {
                get
                {
                    return base.x;
                }
                set
                {
                    base.x = value;
                    if (_list != null)
                    {
                        _list.changedIds.Add(id);
                    }
                }
            }

            public override int y
            {
                get
                {
                    return base.y;
                }
                set
                {
                    base.y = value;
                    if(_list != null)
                    {
                        _list.changedIds.Add(id);
                    }                   
                }
            }

            private _FoodList _list;

            public _FoodSprite(Game game, int id, _FoodList list, int score) :base(game, id, score)
            {
                _list = list;

                sprite = game.newSprite<Sprite>(TextureManager.sharedInst["apple"]);

                _baseSize = 16 + score / 8;

                sprite.width = _baseSize;
                sprite.height = _baseSize;

                addChild(sprite); 
            }

            public System.Drawing.Rectangle getHitBounds()
            {
                return new System.Drawing.Rectangle(this.x - _baseSize/2, this.y - _baseSize/2, _baseSize, _baseSize);
            }

            public override string ToString()
            {
                return string.Format("<_FoodSprite id={0}>", id);
            }
        }

        public void init()
        {          
            _initAsset();
            _args = new GameArgs();

            _boundRect = new Rectangle(0, 10, 90, 60);

            _state = EGameState.Start;
        }

        public System.Drawing.Rectangle boundRect
        {
            get
            {
                return _boundRect;
            }
        }

        private bool _checkGameOver()
        {

            return false;
            /*
            if (!_boundRect.Contains(_player.x, _player.y))
            {
                return true;
            }*/
            /*
            if (_player.hitTest(_player.x, _player.y))
            {
                return true;
            }*/

            return false;
        }
    }

}
