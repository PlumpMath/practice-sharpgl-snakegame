using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using Augite;
    using Augite.Events;

    class Button :Sprite
    {

        private Sprite _bgSprite; 
        private Sprite _textContainer;

        private string _text;

        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                _updateText();
            }
        }

        public Button(Game game)
            :base(game)
        {
            _bgSprite = new Sprite(game, TextureManager.sharedInst["button"]);
            _textContainer = new Sprite(game);

            this.addChild(_bgSprite);
            this.addChild(_textContainer);

            _bgSprite.x = 0;
            _bgSprite.y = 0;

            _textContainer.x = _bgSprite.x;
            _textContainer.y = _bgSprite.y;
            

            this.width = _bgSprite.width;
            this.height = _bgSprite.height;
        }

        public override int dispatchMouseClick(MouseEventArgs args, int x, int y)
        {

            Console.WriteLine("onMouseClick pt={0},{1} bounds={2}", x, y, bounds);

            return base.dispatchMouseClick(args, x, y);
        }

        private void _updateText()
        {
            _textContainer.children.Clear();

            var bmp = new System.Drawing.Bitmap(320, 64);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var renderer = new TextRenderer();
                renderer.borderColor = System.Drawing.Color.Black;
                renderer.borderWidth = 1.0f;
                renderer.render(g, _text, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));
            }

            var textTex = TextureManager.sharedInst.texFromBitmap(bmp);

            var sprite = game.newSprite<Sprite>(textTex);

            _textContainer.addChild(sprite);
        }



    }
}
