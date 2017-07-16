using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using System.Drawing;
    using Augite;
    using Augite.Events;

    class GamePlay :Sprite
    {
        private GameMain _game;
        private GameScene _scene;
        private Sprite _uiContainer;
        private Sprite _topContainer;

        private int _scoreValue = 0;

        private System.Drawing.Bitmap _scoreLabelBitmap;
        private System.Drawing.Bitmap _scoreBitmap;
        private JoystickSprite _joystick;
        private Sprite _joystickDebugSprite;

        private SnakeSprite _player;
        private List<SnakeNPC> _npcs;

        private List<Color> _allSnakeColors = new List<Color>();

        public GameScene scene
        {
            get
            {
                return _scene;
            }
        }

        private ScaleMapRenderer _scaleMapRenderer;

        public GamePlay(GameMain game):base(game)
        {
            
            _game = game;
            _scene = game.newSprite<GameScene>();
            _uiContainer = game.newSprite<Sprite>();
            _topContainer = game.newSprite<Sprite>();
            addChild(_scene);
            addChild(_uiContainer);
            addChild(_topContainer);

            _scoreLabelBitmap = _newLableBitmap("SCORE");
            _scoreBitmap = new Bitmap(320, 50);

            _joystick = game.newSprite<JoystickSprite>();

            _joystick.x = 150;
            _joystick.y = game.gameWindow.height - 150;

            _uiContainer.addChild(_joystick);

            _joystick.movementEvent += _joystick_movementEvent;

            _joystickDebugSprite = game.newSprite<Sprite>(TextureManager.sharedInst["apple"]);

            _scaleMapRenderer = new ScaleMapRenderer(this);
            
            this.game.keyPressEvent += Game_keyPressEvent;
            this.game.keyDownEvent += Game_keyDownEvent;
            // _playerContainer.addChild(_joystickDebugSprite);
            this.game.gameUpdateEvent += Game_gameUpdateEvent;

            _npcs = new List<SnakeNPC>();

            _setupPlayer(); 

            _scene.scoreAddEvent += _scene_scoreAddEvent;
            _scene.snakeDeadEvent += _scene_snakeDeadEvent;

            _initBaseFoods();
        }

        private Dictionary<int, SnakeNPC> _getNpcById = new Dictionary<int, SnakeNPC>();
             
        private void _initBaseFoods()
        {
            
            if (_scene.foods.children.Count < 500)
            {
                var food = _scene.spawnFood(Game.random.Next(10, 100));

                int centerX = game.gameWindow.width / 2 + _scene.boundRect.X;
                int centerY = game.gameWindow.height / 2 + _scene.boundRect.Y;


                food.x = centerX + Game.random.Next( - 1024, 1024);
                food.y = centerY + Game.random.Next( - 1024, 1024);
            }
        }

        private void _setupPlayer()
        {
            _scoreValue = 0;
            _updateScoreBitmap();

           // int centerX = game.gameWindow.width / 2;
           // int centerY = game.gameWindow.height / 2;

            _player = _scene.newSnake();

            _player.x = 0;
            _player.y = 0;
            //_player.x = _rand.Next(centerX - 10, centerX + 20);
            //_player.y = _rand.Next(centerY - 10, centerY + 20);


            _player.colors.Add(Color.FromArgb(255, 231, 56, 40));
            _player.colors.Add(Color.FromArgb(255, 234, 84, 20));
            _player.colors.Add(Color.FromArgb(255, 248, 182, 44));
            _player.colors.Add(Color.FromArgb(255, 0, 255, 0));
            _player.colors.Add(Color.FromArgb(255, 0, 0, 255));
            _player.colors.Add(Color.FromArgb(255, 18, 13, 105));
            _player.colors.Add(Color.FromArgb(255, 83, 62, 124));
            //_player.color = Color.FromArgb(255, 255, 255, 255);

            _player.setup();
        }

        private void _scene_snakeDeadEvent(SnakeSprite snake)
        {
            //Console.WriteLine("_scene_snakeDeadEvent {0}", snake);


            int counter = 0;
            foreach(var rect in snake.iterBodyHitRects())
            {
                if(counter > 1)
                {
                    counter = 0;

                    if(Game.random.Next(0, 100) > 30)
                    {
                        int bodySize = (rect.Width + rect.Height) / 2;
                        int minSize = bodySize - 10;
                        if (minSize < 16)
                        {
                            minSize = 16;
                        }

                        int maxSize = minSize + 10;

                        var food = _scene.spawnFood(Game.random.Next(minSize, maxSize));

                        food.x = rect.X;
                        food.y = rect.Y;
                   
                    }

                   
                }
               

                counter++;
            }


         
            /*
            int centerX = game.gameWindow.width / 2 + _scene.boundRect.X;
            int centerY = game.gameWindow.height / 2 + _scene.boundRect.Y;


            food.x = centerX + Game.random.Next(-512, 512);
            food.y = centerY + Game.random.Next(-512, 512);*/

            if(snake == _player)
            {
                _player = null;
                _topContainer.addChild(game.newSprite<GameOverSprite>(this));
            }

            if (_getNpcById.ContainsKey(snake.id))
            {
                var npc = _getNpcById[snake.id];
                _getNpcById.Remove(snake.id);
                _npcs.Remove(npc);
            }

        }

        public void restart()
        {
            _setupPlayer();

            _topContainer.children.Clear();


        }

        private IEnumerable<Color> _iterAllSnakeColors()
        {
            yield return Color.FromArgb(255, 0, 0, 0);
            yield return Color.FromArgb(255, 255, 255, 255);
            yield return Color.FromArgb(255, 255, 0, 0);
            yield return Color.FromArgb(255, 0, 255, 0);
            yield return Color.FromArgb(255, 0, 0, 255);

            yield return Color.FromArgb(255, 142, 195, 30);
            yield return Color.FromArgb(255, 255, 153, 0);
            yield return Color.FromArgb(255, 3, 110, 184);
            yield return Color.FromArgb(255, 126, 48, 141);            
          
            yield return Color.FromArgb(255, 255, 153, 255);
            yield return Color.FromArgb(255, 10, 48, 143);
            yield return Color.FromArgb(255, 105, 57, 5);
            yield return Color.FromArgb(255, 163, 147, 197);
            yield return Color.FromArgb(255, 240, 143, 126);
            yield return Color.FromArgb(255, 232, 145, 0);
            yield return Color.FromArgb(255, 59, 93, 102);
            yield return Color.FromArgb(255, 156, 133, 106);
            yield return Color.FromArgb(255, 99, 122, 134);
            yield return Color.FromArgb(255, 136, 80, 122);
            yield return Color.FromArgb(255, 184, 202, 116);
            yield return Color.FromArgb(255, 16, 50, 43);
            yield return Color.FromArgb(255, 250, 201, 85);
            yield return Color.FromArgb(255, 228, 0, 126);
        }        

        private void Game_gameUpdateEvent(GameTime gameTime)
        {
            _updateViewport();
            SnakeNPC npc;     

            if (_npcs.Count < 100)
            {
                var snake = _scene.newSnake();

                npc = new SnakeNPC(this, snake);
               

                _getNpcById[snake.id] = npc;


                int colorCount = Game.random.Next(1, 8);

                for(int i=0; i< colorCount; ++i)
                {

                    if (_allSnakeColors.Count < 1)
                    {
                        _allSnakeColors.AddRange((from x in _iterAllSnakeColors() orderby Game.random.Next() select x));
                    }

                    if (_allSnakeColors.Count > 0)
                    {
                        snake.colors.Add(_allSnakeColors[0]);
                        _allSnakeColors.RemoveAt(0);
                    }
                }
                           

                snake.setup();

                int centerX = (_scene.gameRect.X + _scene.gameRect.Width / 2);
                int centerY = (_scene.gameRect.Y + _scene.gameRect.Height / 2);

                snake.x = centerX + Game.random.Next(-_scene.gameRect.Width/3, _scene.gameRect.Width/3);
                snake.y = centerY + Game.random.Next(-_scene.gameRect.Height/3, _scene.gameRect.Height / 3);
             
                _npcs.Add(npc);

            }   
            
            
            for(int i=0; i<_npcs.Count; ++i)
            {
                npc = _npcs[i];
                npc.update(gameTime);
            }

            _updateVisibleInViewport(gameTime);
        }

        private void _updateVisibleInViewport(GameTime gameTime)
        {
            _scene.visibleTestWithRect(_viewportDrawBounds);

        }


        private System.Drawing.Rectangle _viewportDrawBounds = new Rectangle();

        private void _updateViewport()
        {
            if (_player == null) return;

            int startX = -_player.x + game.gameWindow.width / 2;
            int startY = -_player.y + game.gameWindow.height / 2;

            _scene.x = startX;
            _scene.y = startY;

            _viewportDrawBounds.Width = game.gameWindow.width + 256;
            _viewportDrawBounds.Height = game.gameWindow.height + 256;

            _viewportDrawBounds.X = _player.x - _viewportDrawBounds.Width / 2;
            _viewportDrawBounds.Y = _player.y - _viewportDrawBounds.Height / 2;
          

        }


        private void _joystick_movementEvent(JoystickEvent evt)
        {
             //Console.WriteLine("_joystick_movementEvent {0}", evt);

            if (_player == null) return;

            var transform = new System.Drawing.Drawing2D.Matrix();
            transform.Rotate(evt.angle);
            var pts = new System.Drawing.PointF[] { new PointF(0, 64) };
            transform.TransformPoints(pts);
            var pt2 = pts[0];

            //Console.WriteLine("_joystick_movementEvent angle={0} pt2={1}", evt.angle, pt2);
            _player.updateRotationWithRelatedPoint(pt2);

            _joystickDebugSprite.x = _player.x + (int)pt2.X;
            _joystickDebugSprite.y = _player.y + (int)pt2.Y;
        }



        private void Game_keyDownEvent(Augite.Events.KeyEvent evt)
        {
            if (_player == null) return;

            if (evt.keyCode == System.Windows.Forms.Keys.Left)
            {
                _player.turn(EDirection.Left);
            }

            if (evt.keyCode == System.Windows.Forms.Keys.Right)
            {
                _player.turn(EDirection.Right);
            }

            if (evt.keyCode == System.Windows.Forms.Keys.Up)
            {
                _player.turn(EDirection.Up);
            }

            if (evt.keyCode == System.Windows.Forms.Keys.Down)
            {
                _player.turn(EDirection.Down);
            }

        }

        private void Game_keyPressEvent(Augite.Events.KeyEvent evt)
        {
            if (_player == null) return;

            if (evt.keyCode == System.Windows.Forms.Keys.Up)
            {
                _player.turn(EDirection.Up);
            }

            if (evt.keyCode == System.Windows.Forms.Keys.Down)
            {
                _player.turn(EDirection.Down);
            }
        }



        public override void drawGDI(Graphics g)
        {
            
            int startY = 0;

            g.DrawImage(_scoreLabelBitmap, new System.Drawing.Rectangle(620, startY + 10, _scoreLabelBitmap.Width, _scoreLabelBitmap.Height), 0, 0, _scoreLabelBitmap.Width, _scoreLabelBitmap.Height, System.Drawing.GraphicsUnit.Pixel, null);

            g.DrawImage(_scoreBitmap, new System.Drawing.Rectangle(620, startY + 40, _scoreBitmap.Width, _scoreBitmap.Height), 0, 0, _scoreBitmap.Width, _scoreBitmap.Height, System.Drawing.GraphicsUnit.Pixel, null);


            g.DrawString(string.Format("GAME TIME = {0}", game.gameTime.TotalGameTime), System.Drawing.SystemFonts.DefaultFont, Brushes.White, new System.Drawing.Point(10, 10));

            g.DrawString(string.Format("NPC COUNT={0}", _npcs.Count), System.Drawing.SystemFonts.DefaultFont, Brushes.Green, new System.Drawing.Point(10, 30));


            g.ResetTransform();
            g.TranslateTransform(game.gameWindow.width - 240, game.gameWindow.height - 240);

            _scaleMapRenderer.render(g);
            g.ResetTransform();


            base.drawGDI(g);
        }


        class ScaleMapRenderer
        {
            private System.Drawing.Brush _bgBrush;
            private System.Drawing.Brush _ptBrush;
            private System.Drawing.Brush _playerBrush;
            private GamePlay _gamePlay;
            private System.Drawing.Size _size;

            public ScaleMapRenderer(GamePlay gamePlay)
            {
                _gamePlay = gamePlay;
                _size = new Size(200, 200);
                _bgBrush = new SolidBrush(Color.FromArgb(5, 255, 255, 255));
                _ptBrush = new SolidBrush(Color.FromArgb(153, 255, 0, 0));
                _playerBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 255));
            }

            public void render(System.Drawing.Graphics g)
            {
                g.FillRectangle(_bgBrush, new RectangleF(0, 0, _size.Width, _size.Height));

                float widthRate = _size.Width / (float)_gamePlay.scene.gameRect.Width;
                float heightRate = _size.Height / (float)_gamePlay.scene.gameRect.Height;

                float centerX = _gamePlay.scene.gameRect.Width / 2;
                float centerY = _gamePlay.scene.gameRect.Height / 2;

                for (int i=0; i<_gamePlay.scene.players.Count; ++i)
                {
                    var snake = _gamePlay.scene.players[i];

                    int x2 = (int)((snake.x + centerX) * widthRate);
                    int y2 = (int)((snake.y + centerY) * heightRate);

                    g.FillEllipse(_ptBrush, new RectangleF(x2-1, y2-1, 2, 2));
                }

                if(_gamePlay._player != null)
                {

                    int x2 = (int)((_gamePlay._player.x + centerX) * widthRate);
                    int y2 = (int)((_gamePlay._player.y + centerY) * heightRate);

                    g.FillRectangle(_playerBrush, new RectangleF(x2 - 2, y2 - 2, 4, 4));
                }

            }
        }

        private void _scene_scoreAddEvent(SnakeSprite snake, int score)
        {
            if(snake == _player)
            {
                _scoreValue += score;
                _updateScoreBitmap();
            }
          
        }

        private System.Drawing.Bitmap _newLableBitmap(string text)
        {
            var bmp = new Bitmap(320, 48);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                var renderer = new TextRenderer();
                renderer.stringFormat.Alignment = StringAlignment.Far;
                renderer.fontSize = 20;
                renderer.borderWidth = 1.0f;
                renderer.font = _game.fontManager.defaultFont;

                renderer.render(g, text, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            return bmp;
        }


        private void _updateScoreBitmap()
        {
            _updateGradientBitmap(_scoreBitmap, string.Format("{0}", _scoreValue));

        }


        private void _updateGradientBitmap(System.Drawing.Bitmap bmp, string text)
        {
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.Clear(Color.Transparent);
                //g.Clear(Color.White);

                var renderer = new GradientTextRenderer();
                renderer.fontSize = 32;
                renderer.textColor = Color.FromArgb(255, 243, 151, 0);
                renderer.borderColor = Color.FromArgb(255, 255, 255, 255);
                renderer.borderWidth = 2.0f;
                renderer.stringFormat.Alignment = StringAlignment.Far;
                renderer.font =  _game.fontManager.labelFont;               

                renderer.render(g, text, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));

            }
        }


        class GradientTextRenderer : TextRenderer
        {
            /*
            public override Brush createBrush()
            {
                var brush2 = new System.Drawing.Drawing2D.LinearGradientBrush(new PointF(8, 8), new PointF(16, 16), Color.FromArgb(255, 234, 84, 20), Color.FromArgb(255, 243, 151, 0));

                return brush2;
            }*/
        }


    }
}
