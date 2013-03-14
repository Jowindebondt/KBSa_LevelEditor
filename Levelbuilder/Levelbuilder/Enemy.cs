using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levelbuilder
{
    abstract class Enemy : Gameobject
    {
        public Point endPoint { get; set; }
    }

    class Goomba : Enemy
    {
        public override Image getImage()
        {
            return Properties.Resources.Goomba;
        }
    }

    abstract class Koopa : Enemy
    {

    }

    class Troopa : Koopa
    {
        public override Image getImage()
        {
            return Properties.Resources.Troopa;
        }
    }

    class ParaTroopa : Koopa
    {
        public override Image getImage()
        {
            return Properties.Resources.ParaTroopa;
        }
    }
}
