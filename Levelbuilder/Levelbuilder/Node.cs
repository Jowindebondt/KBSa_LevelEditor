using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Levelbuilder
{
    class Node
    {
        public Gameobject gameObject { get; set; }

        public void drawGameObject(Graphics graphics, int x, int y)
        {
            if (gameObject != null)
            {
                graphics.DrawImage(gameObject.getImage(), x * 32, y * 32, 32, 32);
            }
        }
    }
}
