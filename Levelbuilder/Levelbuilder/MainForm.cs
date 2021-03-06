﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Levelbuilder
{
    public partial class MainForm : Form
    {
        const int ROWS = 22;
        const int COLUMNS = 215;
        Node[][] level;
        List<Node> selectedNodes = new List<Node>();
        int viewColumn = 1;
        int viewRow = 1;
        int visibleColumns = 215;
        int visibleRows = 22;
        bool shiftKeyIsDown = false;
        bool HeroIsSet = false;
        Point lastSelectedNode = new Point(-1,-1);
        Enemy selectedEnemy = null;

        public MainForm()
        {
            InitializeComponent();

            level = new Node[COLUMNS][];
            for (int i = 0; i < COLUMNS; i++)
            {
                level[i] = new Node[ROWS];
            }

            for (int x = 0; x < COLUMNS; x++)
            {
                for (int y = 0; y < ROWS; y++)
                {
                    level[x][y] = new Node();
                }
            }

            comboBox_Theme.SelectedIndex = 0;

            for (int i = 0; i < ROWS; i++)
            {
                level[0][i].gameObject = new Block() { isFixed = true };
                level[COLUMNS - 3][i].gameObject = new Block() { isFixed = true };
            }

            setupScreen();
        }

        private void setupScreen()
        {
            int panelWidth = this.Width - gb_Tiles.Width - vScrollBar.Width - 40;
            int panelHeight = this.Height - hScrollBar.Height - 60;
            panelHeight = panelHeight - (panelHeight % 32) + 1;

            panel.Width = panelWidth - (panelWidth % 32) + 1;
            if ((ROWS * 32 + 1) > panelHeight)
                panel.Height = panelHeight;
            else
                panel.Height = ROWS * 32 + 1;

            visibleColumns = panel.Width / 32;
            visibleRows = panel.Height / 32;

            gb_Tiles.Height = panel.Height;

            hScrollBar.Maximum = 59;
            hScrollBar.LargeChange = 3;
            hScrollBar.SmallChange = 1;
            hScrollBar.Location = new Point(panel.Location.X, panel.Location.Y + panel.Height);
            hScrollBar.Width = panel.Width;
            hScrollBar.Maximum = COLUMNS - visibleColumns;

            vScrollBar.Height = panel.Height;
            vScrollBar.Location = new Point(panel.Location.X + panel.Width, panel.Location.Y);
            vScrollBar.LargeChange = 3;
            vScrollBar.SmallChange = 1;
            vScrollBar.Maximum = ROWS - visibleRows;

            while (viewRow + visibleRows > ROWS + 1)
            {
                viewRow--;
            }
            while (viewColumn + visibleColumns > COLUMNS + 1)
            {
                viewColumn--;
            }
        }

        private void comboBox_Theme_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Theme.SelectedItem.ToString())
            {
                case "Landscape":
                    panel.BackgroundImage = Properties.Resources.backgroundSky;
                    break;

                case "Dungeon":
                    panel.BackgroundImage = Properties.Resources.backgroundCave;
                    break;

                case "Sky":

                    break;

                case "Water":

                    break;
            }
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.Black);
            Brush b = new SolidBrush(Color.FromArgb(150, Color.Gray));

            vScrollBar.Value = viewRow - 1;
            hScrollBar.Value = viewColumn - 1;

            int maxX = 0;
            if (visibleColumns + viewColumn > COLUMNS)
                maxX = COLUMNS - 1;
            else
                maxX = visibleColumns + viewColumn;

            for (int x = viewColumn; x < maxX; x++)
            {
                for (int y = viewRow; y < (visibleRows + viewRow); y++)
                {
                    level[x - 1][y - 1].drawGameObject(e.Graphics, x - viewColumn, y - viewRow);
                    
                    if (selectedNodes.Contains(level[x - 1][y - 1]))
                    {
                        if (selectedEnemy != null)
                        {
                            b = new SolidBrush(Color.FromArgb(150, Color.Green));
                            e.Graphics.FillRectangle(b, (x - viewColumn) * 32, (y - viewRow) * 32, 32, 32);
                        }
                        else
                        {
                            if (level[x - 1][y - 1].gameObject != null && level[x - 1][y - 1].gameObject == selectedEnemy)
                                b = new SolidBrush(Color.FromArgb(150, Color.Green));
                            e.Graphics.FillRectangle(b, (x - viewColumn) * 32, (y - viewRow) * 32, 32, 32);
                        }
                    }
                }
            }

            for (int i = 0; i <= COLUMNS; i++)
            {
                e.Graphics.DrawLine(p, i * 32, 0, i * 32, ROWS * 32);
            }

            for (int i = 0; i <= ROWS; i++)
            {
                e.Graphics.DrawLine(p, 0, i * 32, COLUMNS * 32, i * 32);
            }
        }

        private void panel_MouseClick(object sender, MouseEventArgs e)
        {
            int x = (e.X / 32) + viewColumn;
            int y = (e.Y / 32) + viewRow;

            if (shiftKeyIsDown)
            {
                selectedNodes.Clear();

                if (x <= COLUMNS || y <= ROWS)
                {
                    if (selectedEnemy != null)
                        selectedEnemy.endPoint = new Point(x - 1, y - 1);

                    if (x < lastSelectedNode.X)
                    {
                        for (int i = x - 1; i <= lastSelectedNode.X; i++)
                        {
                            if (y < lastSelectedNode.Y)
                            {
                                for (int j = y - 1; j <= lastSelectedNode.Y; j++)
                                {
                                    selectedNodes.Add(level[i][j]);
                                }
                            }
                            else if (y >= lastSelectedNode.Y)
                            {
                                for (int j = lastSelectedNode.Y; j < y; j++)
                                {
                                    selectedNodes.Add(level[i][j]);
                                }
                            }
                        }
                    }
                    else if (x >= lastSelectedNode.X)
                    {
                        for (int i = lastSelectedNode.X; i < x; i++)
                        {
                            if (y < lastSelectedNode.Y)
                            {
                                for (int j = y; j <= lastSelectedNode.Y; j++)
                                {
                                    selectedNodes.Add(level[i][j]);
                                }
                            }
                            else if (y >= lastSelectedNode.Y)
                            {
                                for (int j = lastSelectedNode.Y; j < y; j++)
                                {
                                    selectedNodes.Add(level[i][j]);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                selectedNodes.Clear();
                if (x <= COLUMNS || y <= ROWS)
                {
                    selectedNodes.Add(level[x - 1][y - 1]);
                    if (level[x - 1][y - 1].gameObject != null && level[x - 1][y - 1].gameObject.GetType().BaseType == typeof(Enemy))
                    {
                        selectedEnemy = (Enemy)level[x - 1][y - 1].gameObject;
                        if (selectedEnemy.endPoint.X > -1 && selectedEnemy.endPoint.Y > -1)
                        {
                            if (x < selectedEnemy.endPoint.X)
                            {
                                for (int i = x - 1; i <= selectedEnemy.endPoint.X; i++)
                                {
                                    if (y < selectedEnemy.endPoint.Y)
                                    {
                                        for (int j = y - 1; j <= selectedEnemy.endPoint.Y; j++)
                                        {
                                            selectedNodes.Add(level[i][j]);
                                        }
                                    }
                                    else if (y >= selectedEnemy.endPoint.Y)
                                    {
                                        for (int j = selectedEnemy.endPoint.Y; j < y; j++)
                                        {
                                            selectedNodes.Add(level[i][j]);
                                        }
                                    }
                                }
                            }
                            else if (x >= selectedEnemy.endPoint.X)
                            {
                                for (int i = selectedEnemy.endPoint.X; i < x; i++)
                                {
                                    if (y < selectedEnemy.endPoint.Y)
                                    {
                                        for (int j = y; j <= selectedEnemy.endPoint.Y; j++)
                                        {
                                            selectedNodes.Add(level[i][j]);
                                        }
                                    }
                                    else if (y >= selectedEnemy.endPoint.Y)
                                    {
                                        for (int j = selectedEnemy.endPoint.Y; j < y; j++)
                                        {
                                            selectedNodes.Add(level[i][j]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                        selectedEnemy = null;

                    lastSelectedNode = new Point(x - 1, y - 1);
                }
            }
            panel.Invalidate();
        }

        private void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            
            if (e.OldValue > e.NewValue)
            {
                int diff = e.OldValue - e.NewValue;
                //Left
                viewColumn -= 3 * diff;
                if (viewColumn < 1)
                    viewColumn = 1;
            }
            else if(e.OldValue < e.NewValue)
            {
                int diff = e.NewValue - e.OldValue;
                //Right
                viewColumn += 3 * diff;
                if (viewColumn > (COLUMNS - visibleColumns))
                    viewColumn = COLUMNS - visibleColumns;
            }

            panel.Invalidate();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            if (selectedNodes.Count > 0)
            {
                bool errorMessageShown = false;
                foreach (Node selectedNode in selectedNodes)
                {
                    if (selectedNode.gameObject != null && selectedNode.gameObject.GetType().Name == "Hero")
                        HeroIsSet = false;

                    switch (((PictureBox)sender).Name)
                    {
                        case "pictureBox_Ground_TopLeft":
                            selectedNode.gameObject = new Ground() { groundType = 1 };
                            break;

                        case "pictureBox_Ground_TopCenter":
                            selectedNode.gameObject = new Ground() { groundType = 2 };
                            break;

                        case "pictureBox_Ground_TopRight":
                            selectedNode.gameObject = new Ground() { groundType = 3 };
                            break;

                        case "pictureBox_Ground_CenterLeft":
                            selectedNode.gameObject = new Ground() { groundType = 4 };
                            break;

                        case "pictureBox_Ground_CenterCenter":
                            selectedNode.gameObject = new Ground() { groundType = 5 };
                            break;

                        case "pictureBox_Ground_CenterRight":
                            selectedNode.gameObject = new Ground() { groundType = 6 };
                            break;

                        case "pictureBox_Ground_BottomLeft":
                            selectedNode.gameObject = new Ground() { groundType = 7 };
                            break;

                        case "pictureBox_Ground_BottomCenter":
                            selectedNode.gameObject = new Ground() { groundType = 8 };
                            break;

                        case "pictureBox_Ground_BottomRight":
                            selectedNode.gameObject = new Ground() { groundType = 9 };
                            break;

                        case "pictureBox_Block":
                            selectedNode.gameObject = new Block() { isSpecial = false };
                            break;

                        case "pictureBox_Block_Special":
                            selectedNode.gameObject = new Block() { isSpecial = true };
                            break;

                        case "pictureBox_Block_Fixed":
                            selectedNode.gameObject = new Block() { isFixed = true };
                            break;

                        case "pictureBox_Block_Coin":
                            selectedNode.gameObject = new Block() { gadget = new Coin() { amount = 1 } };
                            break;

                        case "pictureBox_Block_Coin5":
                            selectedNode.gameObject = new Block() { gadget = new Coin() { amount = 5 } };
                            break;

                        case "pictureBox_Block_PowerUp":
                            selectedNode.gameObject = new Block() { gadget = new PowerUp() };
                            break;

                        case "pictureBox_Block_LiveUp":
                            selectedNode.gameObject = new Block() { gadget = new LiveUp() };
                            break;

                        case "pictureBox_Block_Special_Coin":
                            selectedNode.gameObject = new Block() { isSpecial = true, gadget = new Coin() { amount = 1 } };
                            break;

                        case "pictureBox_Block_Special_Coin5":
                            selectedNode.gameObject = new Block() { isSpecial = true, gadget = new Coin() { amount = 5 } };
                            break;

                        case "pictureBox_Block_Special_PowerUp":
                            selectedNode.gameObject = new Block() { isSpecial = true, gadget = new PowerUp() };
                            break;

                        case "pictureBox_Block_Special_LiveUp":
                            selectedNode.gameObject = new Block() { isSpecial = true, gadget = new LiveUp() };
                            break;

                        case "pictureBox_Empty":
                            selectedNode.gameObject = null;
                            break;

                        case "pictureBox_Mario":
                            if (selectedNodes.Count == 1 && !HeroIsSet)
                            {
                                selectedNode.gameObject = new Hero() { character = "Mario" };
                                HeroIsSet = true;
                            }
                            else
                            {
                                if (!errorMessageShown)
                                {
                                    MessageBox.Show("This world is not big enough for more than one hero!");
                                    errorMessageShown = true;
                                }
                            }
                            break;

                        case "pictureBox_Pipe_BottomLeft":
                            selectedNode.gameObject = new Pipe() { pipeType = 4 };
                            break;

                        case "pictureBox_Pipe_BottomCenter":
                            selectedNode.gameObject = new Pipe() { pipeType = 5 };
                            break;

                        case "pictureBox_Pipe_BottomRight":
                            selectedNode.gameObject = new Pipe() { pipeType = 6 };
                            break;

                        case "pictureBox_Pipe_TopLeft":
                            selectedNode.gameObject = new Pipe() { pipeType = 1 };
                            break;

                        case "pictureBox_Pipe_TopCenter":
                            selectedNode.gameObject = new Pipe() { pipeType = 2 };
                            break;

                        case "pictureBox_Pipe_TopRight":
                            selectedNode.gameObject = new Pipe() { pipeType = 3 };
                            break;

                        case "pictureBox_Goomba":
                            selectedNode.gameObject = new Goomba() { endPoint = new Point(-1, -1) };
                            break;

                        case "pictureBox_Troopa":
                            selectedNode.gameObject = new Koopa() { endPoint = new Point(-1, -1) };
                            break;

                        case "pictureBox_Castle_Battlement":
                            selectedNode.gameObject = new Castle() { castleType = 1 };
                            break;

                        case "pictureBox_Castle_Battlement_Wall":
                            selectedNode.gameObject = new Castle() { castleType = 2 };
                            break;

                        case "pictureBox_Castle_Wall":
                            selectedNode.gameObject = new Castle() { castleType = 3 };
                            break;

                        case "pictureBox_Castle_Door":
                            selectedNode.gameObject = new Castle() { castleType = 4 };
                            break;

                        case "pictureBox_Castle_LeftGap":
                            selectedNode.gameObject = new Castle() { castleType = 5 };
                            break;

                        case "pictureBox_Castle_Gap":
                            selectedNode.gameObject = new Castle() { castleType = 6 };
                            break;

                        case "pictureBox_Castle_RightGap":
                            selectedNode.gameObject = new Castle() { castleType = 7 };
                            break;
                    }
                }
                panel.Invalidate();
                selectedNodes.Clear();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift)
                shiftKeyIsDown = true;
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            shiftKeyIsDown = false;
        }

        private void button_CreateXML_Click(object sender, EventArgs e)
        {
            button_CreateXML.Enabled = false;
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "XML|*.xml|All files|*.*";
                sfd.ShowDialog();

                if (sfd.FileName != "")
                {
                    XmlTextWriter writer = new XmlTextWriter(sfd.FileName, null);
                    //Opening the document
                    writer.WriteStartDocument();
                    writer.WriteWhitespace("\n");

                    //Opening the root node
                    writer.WriteStartElement("superMario");
                    writer.WriteWhitespace("\n");

                    //Creating the factory node
                    writer.WriteWhitespace("\t");
                    writer.WriteStartElement("factory");
                    writer.WriteAttributeString("name", comboBox_Theme.SelectedItem.ToString().ToLower());
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    //Creating the hero node
                    writer.WriteWhitespace("\t");
                    writer.WriteStartElement("hero");
                    for (int i = 0; i < COLUMNS; i++)
                    {
                        for (int j = 0; j < ROWS; j++)
                        {
                            if (level[i][j].gameObject != null && level[i][j].gameObject.GetType().Name == "Hero")
                            {
                                writer.WriteAttributeString("character", ((Hero)level[i][j].gameObject).character.ToString().ToLower());
                                writer.WriteAttributeString("x", i.ToString());
                                writer.WriteAttributeString("y", j.ToString());
                                writer.WriteAttributeString("coins", "0");
                                writer.WriteAttributeString("points", "0");
                                writer.WriteAttributeString("lives", "5");
                            }
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    //Opening the enemies node
                    writer.WriteWhitespace("\t");
                    writer.WriteStartElement("enemies");
                    writer.WriteWhitespace("\n");

                    for (int i = 0; i < COLUMNS; i++)
                    {
                        for (int j = 0; j < ROWS; j++)
                        {
                            if (level[i][j].gameObject != null)
                            {
                                
                                switch(level[i][j].gameObject.GetType().Name)
                                {
                                    case "Goomba":
                                    case "Koopa":
                                        Enemy enemy = (Enemy)level[i][j].gameObject;
                                        //Opening the enemy node
                                        writer.WriteWhitespace("\t\t");
                                        writer.WriteStartElement("enemy");
                                        writer.WriteAttributeString("character", enemy.GetType().Name.ToLower());
                                        writer.WriteWhitespace("\n");

                                        //Creating the location node
                                        writer.WriteWhitespace("\t\t\t");
                                        writer.WriteStartElement("location");
                                        writer.WriteAttributeString("x", i.ToString());
                                        writer.WriteAttributeString("y", j.ToString());
                                        writer.WriteEndElement();
                                        writer.WriteWhitespace("\n");

                                        //Creating the endPath node
                                        writer.WriteWhitespace("\t\t\t");
                                        writer.WriteStartElement("endPath");
                                        writer.WriteAttributeString("x", enemy.endPoint.X.ToString());
                                        writer.WriteAttributeString("y", enemy.endPoint.Y.ToString());
                                        writer.WriteEndElement();
                                        writer.WriteWhitespace("\n");

                                        //Closing the enemy node
                                        writer.WriteWhitespace("\t\t");
                                        writer.WriteEndElement();
                                        writer.WriteWhitespace("\n");
                                        break;
                                }
                            }
                        }
                    }

                    //Closing the enemies node
                    writer.WriteWhitespace("\t");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    //Opening the level node
                    writer.WriteWhitespace("\t");
                    writer.WriteStartElement("level");
                    writer.WriteAttributeString("width", COLUMNS.ToString());
                    writer.WriteAttributeString("height", ROWS.ToString());
                    writer.WriteAttributeString("nr", "" + levelNumber.Value);
                    writer.WriteWhitespace("\n");

                    //Opening the blocks node
                    writer.WriteWhitespace("\t\t");
                    writer.WriteStartElement("blocks");
                    writer.WriteWhitespace("\n");

                    for (int i = 0; i < COLUMNS; i++)
                    {
                        for (int j = 0; j < ROWS; j++)
                        {
                            if (level[i][j].gameObject != null && level[i][j].gameObject.GetType().Name == "Block")
                            {
                                //Opening a block node
                                writer.WriteWhitespace("\t\t\t");
                                writer.WriteStartElement("block");
                                writer.WriteAttributeString("isSpecial", ((Block)level[i][j].gameObject).isSpecial.ToString().ToLower());
                                writer.WriteAttributeString("isFixed", ((Block)level[i][j].gameObject).isFixed.ToString().ToLower());
                                writer.WriteWhitespace("\n");

                                //Creating a location node
                                writer.WriteWhitespace("\t\t\t\t");
                                writer.WriteStartElement("location");
                                writer.WriteAttributeString("x", i.ToString());
                                writer.WriteAttributeString("y", j.ToString());
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");

                                //Opening a gadget node
                                writer.WriteWhitespace("\t\t\t\t");
                                writer.WriteStartElement("gadget");
                                writer.WriteWhitespace("\n");

                                if (((Block)level[i][j].gameObject).gadget != null && ((Block)level[i][j].gameObject).gadget.GetType() == typeof(Coin))
                                {
                                    for(int n = 0; n < ((Coin)((Block)level[i][j].gameObject).gadget).amount; n++)
                                    {
                                        //Creating a coin node
                                        writer.WriteWhitespace("\t\t\t\t\t");
                                        writer.WriteStartElement("coin");
                                        writer.WriteEndElement();
                                        writer.WriteWhitespace("\n");
                                    }
                                }
                                else if (((Block)level[i][j].gameObject).gadget != null && ((Block)level[i][j].gameObject).gadget.GetType() == typeof(LiveUp))
                                {
                                    //Creating a liveup node
                                    writer.WriteWhitespace("\t\t\t\t\t");
                                    writer.WriteStartElement("liveup");
                                    writer.WriteEndElement();
                                    writer.WriteWhitespace("\n");
                                }
                                else if (((Block)level[i][j].gameObject).gadget != null && ((Block)level[i][j].gameObject).gadget.GetType() == typeof(PowerUp))
                                {
                                    //Creating a powerup node
                                    writer.WriteWhitespace("\t\t\t\t\t");
                                    writer.WriteStartElement("powerup");
                                    writer.WriteEndElement();
                                    writer.WriteWhitespace("\n");
                                }
                                //Closing a gadget node
                                writer.WriteWhitespace("\t\t\t\t");
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");

                                //Closing a block node
                                writer.WriteWhitespace("\t\t\t");
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");
                            }
                        }
                    }

                    //Closing the blocks node
                    writer.WriteWhitespace("\t\t");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    //Opening the pipes node
                    writer.WriteWhitespace("\t\t");
                    writer.WriteStartElement("pipes");
                    writer.WriteWhitespace("\n");

                    for (int i = 0; i < COLUMNS; i++)
                    {
                        for (int j = 0; j < ROWS; j++)
                        {
                            if (level[i][j].gameObject != null && level[i][j].gameObject.GetType().Name == "Pipe")
                            {
                                //Opening a pipe node
                                writer.WriteWhitespace("\t\t\t");
                                writer.WriteStartElement("pipe");
                                writer.WriteAttributeString("type", ((Pipe)level[i][j].gameObject).pipeType.ToString());
                                writer.WriteWhitespace("\n");

                                //Creating a location node
                                writer.WriteWhitespace("\t\t\t\t");
                                writer.WriteStartElement("location");
                                writer.WriteAttributeString("x", i.ToString());
                                writer.WriteAttributeString("y", j.ToString());
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");

                                //Closing a pipe node
                                writer.WriteWhitespace("\t\t\t");
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");
                            }
                        }
                    }

                    //Closing the pipes node
                    writer.WriteWhitespace("\t\t");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    //Opening the grounds node
                    writer.WriteWhitespace("\t\t");
                    writer.WriteStartElement("grounds");
                    writer.WriteWhitespace("\n");

                    for (int i = 0; i < COLUMNS; i++)
                    {
                        for (int j = 0; j < ROWS; j++)
                        {
                            if (level[i][j].gameObject != null && level[i][j].gameObject.GetType().Name == "Ground")
                            {
                                //Opening a ground node
                                writer.WriteWhitespace("\t\t\t");
                                writer.WriteStartElement("ground");
                                writer.WriteAttributeString("type", ((Ground)level[i][j].gameObject).groundType.ToString());
                                writer.WriteWhitespace("\n");

                                //Creating a location node
                                writer.WriteWhitespace("\t\t\t\t");
                                writer.WriteStartElement("location");
                                writer.WriteAttributeString("x", i.ToString());
                                writer.WriteAttributeString("y", j.ToString());
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");

                                //Closing a ground node
                                writer.WriteWhitespace("\t\t\t");
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");
                            }
                        }
                    }

                    //Closing the grounds node
                    writer.WriteWhitespace("\t\t");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    //Opening the castles node
                    writer.WriteWhitespace("\t\t");
                    writer.WriteStartElement("castles");
                    writer.WriteWhitespace("\n");

                    for (int i = 0; i < COLUMNS; i++)
                    {
                        for (int j = 0; j < ROWS; j++)
                        {
                            if (level[i][j].gameObject != null && level[i][j].gameObject.GetType().Name == "Castle")
                            {
                                //Opening a castle node
                                writer.WriteWhitespace("\t\t\t");
                                writer.WriteStartElement("castle");
                                writer.WriteAttributeString("type", ((Castle)level[i][j].gameObject).castleType.ToString());
                                writer.WriteWhitespace("\n");

                                //Creating a location node
                                writer.WriteWhitespace("\t\t\t\t");
                                writer.WriteStartElement("location");
                                writer.WriteAttributeString("x", i.ToString());
                                writer.WriteAttributeString("y", j.ToString());
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");

                                //Closing a castle node
                                writer.WriteWhitespace("\t\t\t");
                                writer.WriteEndElement();
                                writer.WriteWhitespace("\n");
                            }
                        }
                    }

                    //Closing the castles node
                    writer.WriteWhitespace("\t");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    //Closing the level node
                    writer.WriteWhitespace("\t");
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");

                    //Closing the root node
                    writer.WriteEndElement();
                    writer.WriteWhitespace("\n");
                    //Closing the document
                    writer.WriteEndDocument();
                    //Closing the writer
                    writer.Close();
                }
                else
                {
                    MessageBox.Show("There is no file selected");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            button_CreateXML.Enabled = true;
        }

        private void button_Open_Level_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "XML|*.xml|All files|*.*";
                ofd.ShowDialog();

                if (ofd.FileName != "")
                {
                    foreach (Node[] nodes in level)
                        foreach (Node node in nodes)
                            node.gameObject = null;

                    using (XmlReader reader = XmlReader.Create(ofd.FileName))
                    {
                        reader.ReadStartElement("superMario");
                        while (reader.Read())
                        {
                            XElement el = null;
                            int xLocation = -1;
                            int yLocation = -1;

                            switch (reader.Name)
                            {
                                case "level":
                                    el = (XElement)XNode.ReadFrom(reader);
                                    levelNumber.Value = Int32.Parse(el.Attribute("nr").Value);
                                    foreach (XElement levelChild in el.Elements())
                                    {
                                        switch (levelChild.Name.LocalName)
                                        {
                                            case "blocks":
                                                foreach (XElement block in levelChild.Elements())
                                                {
                                                    switch (block.Name.LocalName)
                                                    {
                                                        case "block":
                                                            bool isFixed = false;
                                                            bool isSpecial = false;
                                                            Gadget gadget = null;

                                                            if (block.Attribute("isSpecial").Value == "true")
                                                                isSpecial = true;
                                                            else if (block.Attribute("isFixed").Value == "true")
                                                                isFixed = true;

                                                            XElement gadgets = block.Element("gadget");

                                                            if (gadgets.HasElements)
                                                            {
                                                                foreach (XElement gadgetChild in gadgets.Elements())
                                                                {
                                                                    if (gadgetChild.Name == "coin")
                                                                    {
                                                                        if (gadget != null && gadget.GetType() == typeof(Coin))
                                                                            ((Coin)gadget).amount++;
                                                                        gadget = new Coin() { amount = 1 };
                                                                    }
                                                                    else if (gadgetChild.Name == "liveup")
                                                                    {
                                                                        gadget = new LiveUp();
                                                                    }
                                                                    else if (gadgetChild.Name == "powerup")
                                                                    {
                                                                        gadget = new PowerUp();
                                                                    }
                                                                }
                                                            }

                                                            XElement blockLocation = block.Element("location");
                                                            xLocation = Int32.Parse(blockLocation.Attribute("x").Value);
                                                            yLocation = Int32.Parse(blockLocation.Attribute("y").Value);

                                                            level[xLocation][yLocation].gameObject = new Block() { isSpecial = isSpecial, isFixed = isFixed, gadget = gadget };
                                                            break;
                                                    }
                                                }
                                                break;

                                            case "pipes":
                                                foreach (XElement pipe in levelChild.Elements())
                                                {
                                                    switch (pipe.Name.LocalName)
                                                    {
                                                        case "pipe":
                                                            XElement pipeLocation = pipe.Element("location");

                                                            xLocation = Int32.Parse(pipeLocation.Attribute("x").Value);
                                                            yLocation = Int32.Parse(pipeLocation.Attribute("y").Value);

                                                            level[xLocation][yLocation].gameObject = new Pipe() { pipeType = Int32.Parse(pipe.Attribute("type").Value) };
                                                            break;
                                                    }
                                                }
                                                break;

                                            case "castles":
                                                foreach (XElement castle in levelChild.Elements())
                                                {
                                                    switch (castle.Name.LocalName)
                                                    {
                                                        case "castle":
                                                            XElement castleLocation = castle.Element("location");

                                                            xLocation = Int32.Parse(castleLocation.Attribute("x").Value);
                                                            yLocation = Int32.Parse(castleLocation.Attribute("y").Value);

                                                            level[xLocation][yLocation].gameObject = new Castle() { castleType = Int32.Parse(castle.Attribute("type").Value) };
                                                            break;
                                                    }
                                                }
                                                break;

                                            case "grounds":
                                                foreach (XElement ground in levelChild.Elements())
                                                {
                                                    switch (ground.Name.LocalName)
                                                    {
                                                        case "ground":
                                                            XElement groundLocation = ground.Element("location");

                                                            xLocation = Int32.Parse(groundLocation.Attribute("x").Value);
                                                            yLocation = Int32.Parse(groundLocation.Attribute("y").Value);

                                                            level[xLocation][yLocation].gameObject = new Ground() { groundType = Int32.Parse(ground.Attribute("type").Value) };
                                                            break;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    break;

                                case "hero":
                                    el = (XElement)XNode.ReadFrom(reader);
                                    xLocation = Int32.Parse(el.Attribute("x").Value);
                                    yLocation = Int32.Parse(el.Attribute("y").Value);
                                    level[xLocation][yLocation].gameObject = new Hero(){character = el.Attribute("character").Value};
                                    HeroIsSet = true;
                                    break;

                                case "enemy":
                                    el = (XElement)XNode.ReadFrom(reader);

                                    XElement enemyLocation = el.Element("location");
                                    XElement enemyEndPosition = el.Element("endPath");

                                    xLocation = Int32.Parse(enemyLocation.Attribute("x").Value);
                                    yLocation = Int32.Parse(enemyLocation.Attribute("y").Value);

                                    switch (el.Attribute("character").Value)
                                    {
                                        case "goomba":
                                            level[xLocation][yLocation].gameObject = new Goomba() { endPoint = new Point(Int32.Parse(enemyEndPosition.Attribute("x").Value), Int32.Parse(enemyEndPosition.Attribute("y").Value)) };
                                            break;

                                        case "koopa":
                                            level[xLocation][yLocation].gameObject = new Koopa() { endPoint = new Point(Int32.Parse(enemyEndPosition.Attribute("x").Value), Int32.Parse(enemyEndPosition.Attribute("y").Value)) };
                                            break;
                                    }
                                    break;
                            }
                        }
                    }

                    panel.Invalidate();
                }
                else
                {
                    MessageBox.Show("There is no file selected!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            setupScreen();
        }

        private void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue > e.NewValue)
            {
                int diff = e.OldValue - e.NewValue;
                //Up
                viewRow -= 3 * diff;
                if (viewRow < 1)
                    viewRow = 1;
            }
            else if (e.OldValue < e.NewValue)
            {
                int diff = e.NewValue - e.OldValue;
                //Down
                viewRow += 3 * diff;
                if (viewRow > (ROWS - visibleRows))
                    viewRow = ROWS - visibleRows + 1;
            }

            panel.Invalidate();
        }
    }
}
