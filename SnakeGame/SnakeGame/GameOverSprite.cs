using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using System.Drawing;
    using Augite;

    class GameOverSprite :Sprite
    {
        private Bitmap _bmpGameOver;
        private GameMain _game;
        private GamePlay _gamePlay;
        private Button _restartButton;
     

        public GameOverSprite(GameMain game, GamePlay gamePlay):base(game)
        {
            _game = game;
            _gamePlay = gamePlay;
            init();


            _restartButton = game.newSprite<Button>();
            _restartButton.text = "PRESS RESTART";
            addChild(_restartButton);

            _restartButton.x = game.gameWindow.width / 2;
            _restartButton.y = game.gameWindow.height / 2;

            _restartButton.mouseClick += _restartButton_mouseClick;

            game.gameUpdateEvent += Game_gameUpdateEvent;
        }

        private void _restartButton_mouseClick(Augite.Events.MouseEvent evt)
        {
            _gamePlay.restart();
        }

        public void init()
        {
           
            _bmpGameOver = new Bitmap(400, 128);

            System.Drawing.StringFormat strFmt = new StringFormat();
            strFmt.Alignment = StringAlignment.Center;
            strFmt.LineAlignment = StringAlignment.Center;

            using (var g = System.Drawing.Graphics.FromImage(_bmpGameOver))
            {
                //g.Clear(Color.Black);

                System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

                System.Drawing.Font font = new Font(_game.fontManager.titleFont, 20);

                path.AddString("GAME OVER", font.FontFamily, (int)font.Style, 40, new Rectangle(0, 0, _bmpGameOver.Width, _bmpGameOver.Height), strFmt);
                Brush borderBrush = Brushes.White;
                Pen borderPen = new Pen(borderBrush, 8);

                Brush brush = Brushes.Red;

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                g.DrawPath(borderPen, path);
                g.FillPath(brush, path);
            }


        }

        bool _isShowStartButton = false;
        private TimeSpan _showStartButtonExpire = TimeSpan.MinValue;


        private void Game_gameUpdateEvent(GameTime gameTime)
        {
            if (gameTime.TotalGameTime > _showStartButtonExpire)
            {
                _showStartButtonExpire = gameTime.TotalGameTime + TimeSpan.FromSeconds( 0.3f);
                _isShowStartButton = !_isShowStartButton;
            }

        }


        public override void drawGDI(Graphics g)
        {
            int y1 = (int)(game.gameWindow.height * 0.3f) - _bmpGameOver.Height / 2;

            g.DrawImage(_bmpGameOver, (int)(game.gameWindow.width / 2 - _bmpGameOver.Width / 2), y1);

          
        }


  

    }
}
