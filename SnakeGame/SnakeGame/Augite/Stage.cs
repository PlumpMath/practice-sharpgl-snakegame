using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Augite
{
    using Augite.Events;

    class Stage
    {
        public virtual event MouseClickEvent mouseUp;
        public virtual event MouseClickEvent mouseMove;

        public virtual void addChild(Sprite sprite)
        { }

        public virtual List<Sprite> children { get { throw new NotImplementedException(); } }

    }
}
