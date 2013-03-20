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

    class Castle : Gameobject
    {
        public int castleType { get; set; }

        public override Image getImage()
        {
            Image returnImage = null;
            switch (castleType)
            {
                case 1:
                    returnImage = Properties.Resources.Castle_Battlement;
                    break;
                case 2:
                    returnImage = Properties.Resources.Castle_Battlement_Wall;
                    break;
                case 3:
                    returnImage = Properties.Resources.Castle_Wall;
                    break;
                case 4:
                    returnImage = Properties.Resources.Castle_Door;
                    break;
                case 5:
                    returnImage = Properties.Resources.Castle_Wall_LeftGap;
                    break;
                case 6:
                    returnImage = Properties.Resources.Castle_Gap;
                    break;
                case 7:
                    returnImage = Properties.Resources.Castle_Wall_RightGap;
                    break;
            }
            return returnImage;
        }
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
        public bool isFixed { get; set; }
        public Gadget gadget { get; set; }
        public override Image getImage()
        {
            if (isSpecial)
            {
                if (gadget != null && gadget.GetType().Name == "Coin")
                {
                    Coin coin = (Coin)gadget;
                    if (coin.amount == 1)
                        return Properties.Resources.SpecialBlock_Coin;
                    else
                        return Properties.Resources.SpecialBlock_Coin5;
                }
                else if (gadget != null && gadget.GetType().Name == "PowerUp")
                    return Properties.Resources.SpecialBlock_PowerUp;
                else if (gadget != null && gadget.GetType() == typeof(LiveUp))
                    return Properties.Resources.SpecialBlock_LevelUp;
                return Properties.Resources.SpecialBlock;
            }
            else if (isFixed)
                return Properties.Resources.Block_Fixed;
            else if (gadget != null && gadget.GetType().Name == "Coin")
            {
                Coin coin = (Coin)gadget;
                if (coin.amount == 1)
                    return Properties.Resources.Block_Coin;
                else
                    return Properties.Resources.Block_Coin5;
            }
            else if (gadget != null && gadget.GetType().Name == "PowerUp")
                return Properties.Resources.Block_PowerUp;
            else if (gadget != null && gadget.GetType() == typeof(LiveUp))
                return Properties.Resources.Block_LevelUp;
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

    class Gadget
    {
        
    }

    class Coin : Gadget
    {
        public int amount { get; set; }
    }

    class PowerUp : Gadget
    {

    }

    class LiveUp : Gadget
    {

    }
}
