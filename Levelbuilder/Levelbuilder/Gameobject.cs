using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Levelbuilder
{
    abstract class Gameobject
    {
        public abstract Image getImage();
    }
    
    class Ground : Gameobject
    {
        public int groundType { get; set; }

        public override Image getImage()
        {
            Image returnImage = null;
            switch (groundType)
            {
                case 1:
                    returnImage = Properties.Resources.groundTopLeft;
                    break;
                case 2:
                    returnImage = Properties.Resources.groundTopCenter;
                    break;
                case 3:
                    returnImage = Properties.Resources.groundTopRight;
                    break;
                case 4:
                    returnImage = Properties.Resources.groundCenterLeft;
                    break;
                case 5:
                    returnImage = Properties.Resources.groundCenterCenter;
                    break;
                case 6:
                    returnImage = Properties.Resources.groundCenterRight;
                    break;
                case 7:
                    returnImage = Properties.Resources.groundBottomLeft;
                    break;
                case 8:
                    returnImage = Properties.Resources.groundBottomCenter;
                    break;
                case 9:
                    returnImage = Properties.Resources.groundBottomRight;
                    break;
            }
            return returnImage;
        }
    }

    class Pipe : Gameobject
    {
        public int pipeType { get; set; }
        public override Image getImage()
        {
            Image returnImage = null;
            switch (pipeType)
            {
                case 1:
                    returnImage = Properties.Resources.pipeTopLeft;
                    break;

                case 2:
                    returnImage = Properties.Resources.pipeTopCenter;
                    break;

                case 3:
                    returnImage = Properties.Resources.pipeTopRight;
                    break;

                case 4:
                    returnImage = Properties.Resources.pipeBottomLeft;
                    break;

                case 5:
                    returnImage = Properties.Resources.pipeBottomCenter;
                    break;

                case 6:
                    returnImage = Properties.Resources.pipeBottomRight;
                    break;
            }

            return returnImage;
        }
    }

    class Block : Gameobject
    {
        public bool isSpecial { get; set; }
        public override Image getImage()
        {
            if (isSpecial)
                return Properties.Resources.SpecialBlock;
            return Properties.Resources.Block;
        }

    }

    class Hero : Gameobject
    {
        public string character { get; set; }
        public override Image getImage()
        {
            Image returnImage = null;
            switch (character.ToLower())
            {
                case "mario":
                    returnImage = Properties.Resources.Mario;
                    break;
            }

            return returnImage;
        }
    }

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
}
