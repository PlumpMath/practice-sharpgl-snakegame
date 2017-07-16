using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{

    using Augite;

    class FoodSprite : Sprite
    {
        private int _id;
        private int _score;
        public int id { get { return _id; } }

        public int score
        {
            get
            {
                return _score;
            }
        }

        public FoodSprite(Game game, int id, int score) : base(game)
        {
            _id = id;
            _score = score;
        }
    }
}
