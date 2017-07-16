using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnakeGame
{
    using Augite;

    class FoodList
    {

        public virtual List<Sprite> children { get; }

        public virtual bool exists(int id)
        {
            throw new NotImplementedException();
        }
    }
}
