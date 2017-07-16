using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{

    using System.Drawing;
    using Augite;

    class TextureManager
    {

        private static object _syncInst = new object();
        private static TextureManager _inst;
        public static TextureManager sharedInst
        {
            get
            {
                if(_inst == null)
                {
                    lock (_syncInst)
                    {
                        if (_inst == null)
                        {
                            _inst = new TextureManager();
                        }
                    }
                }

                return _inst;
            }
        }


        private Dictionary<string, Texture2D> _texs;

        public TextureManager()
        {
            _texs = new Dictionary<string, Texture2D>();
           
        }


        public Texture2D this[string key]
        {
            get
            {
                return _texs[key];
            }
        }

        private GameMain _game;

        public Texture2D texFromBitmap(System.Drawing.Bitmap bitmap)
        {
            return _game.graphicsDevice.texFromBitmap(bitmap);
        }

        public void load(GameMain game)
        {
            _game = game;
            _texs["button"] = texFromBitmap(Resource1.green_button01);
            _texs["apple"] = texFromBitmap(ImageUtils.thumbImage( Resource1.apple, 64, 64));
            

            _loadBG();
            //_loadPanelBitmap();
            _loadLevelComplete();
            /*
            _loadAnimalTex(gl, "animal_01", ResourceAnimals.elephant);
            _loadAnimalTex(gl, "animal_02", ResourceAnimals.giraffe);
            _loadAnimalTex(gl, "animal_03", ResourceAnimals.hippo);
            _loadAnimalTex(gl, "animal_04", ResourceAnimals.monkey);
            _loadAnimalTex(gl, "animal_05", ResourceAnimals.panda);
            _loadAnimalTex(gl, "animal_06", ResourceAnimals.parrot);
            _loadAnimalTex(gl, "animal_07", ResourceAnimals.penguin);
            _loadAnimalTex(gl, "animal_08", ResourceAnimals.pig);
            _loadAnimalTex(gl, "animal_09", ResourceAnimals.rabbit);
            _loadAnimalTex(gl, "animal_10", ResourceAnimals.snake);*/
        }

        public System.Drawing.Bitmap floorBitmap;


        private Dictionary<int, Texture2D> _snakeHeadTexByColor = new Dictionary<int, Texture2D>();
        private Dictionary<int, Texture2D> _snakeBodyTexByColor = new Dictionary<int, Texture2D>();


        public Texture2D getSnakeBodyOrCreateWithColor(Color color)
        {
            int colorKey = color.ToArgb();
            Texture2D tex;

            if (!_snakeBodyTexByColor.ContainsKey(colorKey))
            {
               
                var _bmpBodyRound = new Bitmap(128, 128);
                var _bodyBrush = new SolidBrush(Color.FromArgb(255, color.R, color.G, color.B));

                using (var g = System.Drawing.Graphics.FromImage(_bmpBodyRound))
                {
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.FillEllipse(_bodyBrush, new Rectangle(0, 0, _bmpBodyRound.Width, _bmpBodyRound.Height));
                }

                tex = texFromBitmap(_bmpBodyRound);
                _snakeBodyTexByColor[colorKey] = tex;

            }

            tex = _snakeBodyTexByColor[colorKey];

            return tex;
        }

        public Texture2D getSnakeHeadOrCreateWithColor(Color color)
        {
            int colorKey = color.ToArgb();
            Texture2D tex;

            if (!_snakeHeadTexByColor.ContainsKey(colorKey))
            {
                Bitmap _head = new Bitmap(128, 128);
                _head.MakeTransparent();

                using (var g2 = System.Drawing.Graphics.FromImage(_head))
                {
                    // g2.Clear(Color.Blue);
                    g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g2.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    //g2.Clear(Color.Black);

                    float eyeRadius = _head.Width * 0.23f;
                    float eyeBlackRadius = _head.Width * 0.18f;

                    float eyeY1 = _head.Height / 2;
                    float eyeX1 = eyeRadius;
                    float eyeX2 = _head.Width - eyeRadius;

                    float radius = 16;
                    float headWidth = _head.Width * 0.7f;
                    float headHeight = _head.Height;
                    float size = radius * 2;

                    g2.FillEllipse(new SolidBrush(color), _head.Width / 2 - headWidth / 2, 0, headWidth, headHeight);

                    _drawCircle(g2, Brushes.White, eyeX1, eyeY1, eyeRadius);
                    _drawCircle(g2, Brushes.Black, eyeX1, eyeY1, eyeBlackRadius);

                    _drawCircle(g2, Brushes.White, eyeX2, eyeY1, eyeRadius);
                    _drawCircle(g2, Brushes.Black, eyeX2, eyeY1, eyeBlackRadius);
                }

                var _headWrap = new Bitmap(128, 128);
                using (var g2 = System.Drawing.Graphics.FromImage(_headWrap))
                {
                    // g2.Clear(Color.Red);
                    g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g2.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g2.DrawImage(_head, new PointF(_headWrap.Width / 2 - _head.Width / 2, 0));
                }

                tex = texFromBitmap(_headWrap);
                _snakeHeadTexByColor[colorKey] = tex;
            }

            tex = _snakeHeadTexByColor[colorKey];

            return tex;

         
        }

        private void _drawCircle(System.Drawing.Graphics g, Brush brush, float x, float y, float radius)
        {
            float size = radius * 2;

            g.FillEllipse(brush, x - radius, y - radius, size, size);
        }

        private void _loadBG()
        {
          

            var tileSize = new System.Drawing.Size(32, 32);

            var destBmp = new Bitmap(256, 256);

            var allTileBmps = new List<System.Drawing.Bitmap>() {
                Resource1.floor_1,
                Resource1.floor_2,
                Resource1.floor_3,
                Resource1.floor_4,
                Resource1.floor_5,
                Resource1.floor_6,
                Resource1.floor_7,
                Resource1.floor_8,
                Resource1.floor_9,
                Resource1.floor_10,
                Resource1.floor_11,
                Resource1.floor_12,
                Resource1.floor_13,
                Resource1.floor_14,
                Resource1.floor_15,
             };

            var randTileIndex = new List<int>();
           


            using (var g = System.Drawing.Graphics.FromImage(destBmp))
            {
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                g.Clear(Color.Black);
                //g.Clear(Color.FromArgb(255, 30, 30, 30));

                int yCount = (int)(Math.Ceiling(destBmp.Height / (float)tileSize.Height));
                int xCount = (int)(Math.Ceiling(destBmp.Width / (float)tileSize.Width));

                for (int y=0; y< yCount; ++y)
                {
                    for (int x = 0; x < xCount; ++x)
                    {

                        if (randTileIndex.Count < 1)
                        {
                            for (int i = 0; i < allTileBmps.Count; ++i)
                            {
                                randTileIndex.Add(i);
                            }

                            randTileIndex = (from ii in randTileIndex orderby Game.random.Next() select ii).ToList();
                        }

                        int tileIdx = randTileIndex[0];
                        randTileIndex.RemoveAt(0);
                        var tileBmp = allTileBmps[tileIdx];

                        

                        g.DrawImage(tileBmp, new System.Drawing.Rectangle(x*tileSize.Width, y*tileSize.Height, tileSize.Width, tileSize.Height), 0, 0, tileBmp.Width, tileBmp.Height, System.Drawing.GraphicsUnit.Pixel, null);
                    }
                }

                g.FillRectangle(new SolidBrush(Color.FromArgb(153, 0, 0, 0)), new RectangleF(0, 0, destBmp.Width, destBmp.Height));
               

            }

            floorBitmap = destBmp;

            var tex = texFromBitmap(floorBitmap);

            _texs["bg_grid"] = tex;
        }


        private void _loadLevelComplete()
        {
            var bmp = new Bitmap(520, 96);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                var renderer = new GradientTextRenderer();
                renderer.fontSize = 50;
                renderer.borderWidth = 3.0f;
                renderer.borderColor = Color.White;
                renderer.font = _game.fontManager.defaultFont;

                renderer.render(g, "COMPLETE", new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height));
            }


            _texs["level_complete"] = texFromBitmap(bmp);
        }

        class GradientTextRenderer : TextRenderer
        {

            public override Brush createBrush()
            {
                var brush2 = new System.Drawing.Drawing2D.LinearGradientBrush(new PointF(8, 8), new PointF(16, 16), Color.FromArgb(255, 234, 84, 20), Color.FromArgb(255, 255, 0, 0));

                return brush2;
            }
        }
        



    }
}
