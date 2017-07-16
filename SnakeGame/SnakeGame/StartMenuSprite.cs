using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using System.Drawing;
    using Augite;

    class StartMenuSprite :Sprite
    {

        Sprite _titleSprite;
        Button _startButton;
        GameMain _game;

        public StartMenuSprite(GameMain game)
            :base(game)
        {
            _game = game;

            _titleSprite = game.newSprite<Sprite>(_createTitleTex());

            _titleSprite.x = game.gameWindow.width / 2;
            _titleSprite.y = 100;
            addChild(_titleSprite);


            _startButton = new Button(game);
            _startButton.text = "PRESS START";
            _startButton.x = game.gameWindow.width / 2;
            _startButton.y = game.gameWindow.height / 2;
            addChild(_startButton);

            _startButton.mouseClick += _startButton_mouseClick;

            game.gameUpdateEvent += Game_gameUpdateEvent;
        }

        TimeSpan _startButtonToggleExpire = TimeSpan.MinValue;

        private void Game_gameUpdateEvent(GameTime gameTime)
        {
            if (gameTime.TotalGameTime > _startButtonToggleExpire)
            {
                
                _startButton.visible = !_startButton.visible;

                if(_startButton.visible == false)
                {
                    _startButtonToggleExpire = gameTime.TotalGameTime + TimeSpan.FromSeconds(0.3f);
                } else
                {
                    _startButtonToggleExpire = gameTime.TotalGameTime + TimeSpan.FromSeconds(1.0f);
                }
            }
        }

        private void _startButton_mouseClick(Augite.Events.MouseEvent evt)
        {
            _game.playNewGame();
        }

        private Texture2D _createTitleTex()
        {
            StringFormat strFmtCenter = new StringFormat();
            strFmtCenter.Alignment = StringAlignment.Center;
            strFmtCenter.LineAlignment = StringAlignment.Center;


            var _titleRect = new RectangleF(0, 0, 520, 128);
            var _gameTitle = new Bitmap((int)_titleRect.Width, (int)_titleRect.Height);
            _gameTitle.MakeTransparent();


            System.Drawing.Drawing2D.LinearGradientBrush linearBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
              new Point(0, 0),
              new Point(0, 100),
              Color.FromArgb(255, 248, 153, 0),
              Color.FromArgb(255, 231, 56, 40)
              );
            using (var g = Graphics.FromImage(_gameTitle))
            {
                var titleFont = new System.Drawing.Font(_game.fontManager.titleFont, 48);
                // g.Clear(Color.DarkBlue);
                g.DrawString("SNAKE GAME", titleFont, linearBrush, new System.Drawing.Rectangle(0, 0, _gameTitle.Width, _gameTitle.Height), strFmtCenter);

            }

            return game.graphicsDevice.texFromBitmap(_gameTitle);
        }

    

    }
}
