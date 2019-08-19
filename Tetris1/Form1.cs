using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris1
{
    public partial class Form1 : Form
    {
        private static Panel menu;
        private static Button button1;
        private static Button button2;
        private static Button button3;
        private Size buttonSize = new Size(180, 80);
        private int buttonX = 280;
        private int buttonY = 90;

        // Tetris
        TetrisGame game;

        public Form1()
        {
            InitializeComponent();
            //this.Size = new Size(600, 300);
            this.WindowState = FormWindowState.Maximized;
            KeyPreview = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buttonX = this.Width / 2 - 60;
            buttonY = this.Height / 5;
            InitMenu();
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.Controls.Add(menu);
            game = new TetrisGame();
        }

        private void InitMenu()
        {
            menu = new Panel();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();

            menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            menu.Controls.Add(button1);
            menu.Controls.Add(button2);
            menu.Controls.Add(button3);
            menu.Location = new System.Drawing.Point(0, 0);
            menu.Name = "menu";
            menu.Size = new System.Drawing.Size(353, 225);
            menu.TabIndex = 0;
            menu.Dock = DockStyle.Fill;
            menu.BackColor = Color.LightGray;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(buttonX, buttonY);
            button1.Name = "button1";
            button1.Size = buttonSize;
            button1.TabIndex = 0;
            button1.Text = "Start";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(buttonX, 2 * buttonY);
            button2.Name = "button2";
            button2.Size = buttonSize;
            button2.TabIndex = 1;
            button2.Text = "Options";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(buttonX, 3 * buttonY);
            button3.Name = "button3";
            button3.Size = buttonSize;
            button3.TabIndex = 2;
            button3.Text = "Exit";
            button3.UseVisualStyleBackColor = true;
            button3.Click += Button3_Click;
            this.ResumeLayout(false);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Lol you wish", "Hahaha", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            menu.Hide();
            game.Setup();
            this.Controls.Add(game.Canvas);
            
            game.Start();
        }


        // key event handler
        // up down left right space and hold button
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up)
            {
                game.RotatePiece();
                return true;
            }
            if (keyData == Keys.Down)
            {
                game.DropDown();
                return true;
            }
            if (keyData == Keys.Left)
            {
                game.MoveLeft();
                return true;
            }
            if (keyData == Keys.Right)
            {
                game.MoveRight();
                return true;
            }
            if (keyData == Keys.Space)
            {
                if (game.IsGameOver()) game.RestartGame();
                else
                    game.HardDrop();
                return true;
            }
            if ( keyData == Keys.S)
            {
                game.HoldPiece();
                return true;
            }

            if (keyData == Keys.P)
            {
                if (game.IsPaused()) game.ResumeGame();
                else
                    game.PauseGame();
            }
            // For debug purposes
            if( keyData == Keys.D)
            {
                return true;
            }

            if (keyData == Keys.Escape)
            {
                game.Canvas.Hide();
                game.StopGame();
                menu.Show();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            game.StopGame();  // Allow thread to come to an end
        }
    }
}
