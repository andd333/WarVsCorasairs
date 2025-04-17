using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace WarVsCorasairs
{
    public partial class Form1 : Form
    {
        int playerSpeed;

        private const int FIRE_COOLDOWN_MS = 10;
        private const int MAX_BALLS = 1;
        private List<Ball> activeBalls = new List<Ball>();
        private Bitmap ballImage;
        private DateTime lastFireTime = DateTime.MinValue;


        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                        ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.UserPaint, true);

        }

        private void Form1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            playerSpeed = 2;
            Player1.Visible = true;
            Player2.Visible = false;

            ballImage = new Bitmap(@"sprites1\ball.png");
            ballImage = new Bitmap(ballImage, new Size(20, 20));
            ballImage.MakeTransparent();


        }

        private class Ball
        {
            public Point Position;
            public bool IsActive;
        }

        private void Fire()
        {
            if ((DateTime.Now - lastFireTime).TotalMilliseconds < FIRE_COOLDOWN_MS)
                return;

            lastFireTime = DateTime.Now;

            var inactiveBall = activeBalls.Find(b => !b.IsActive);
            if (inactiveBall == null && activeBalls.Count < MAX_BALLS)
            {
                inactiveBall = new Ball();
                activeBalls.Add(inactiveBall);
            }

            if (inactiveBall != null)
            {
                var player = Player1.Visible ? Player1 : Player2;
                inactiveBall.Position = new Point(
                    player.Left + (player.Width - ballImage.Width) / 2,
                    player.Top - ballImage.Height / 3);
                inactiveBall.IsActive = true;
            }
        }

        private void MoveBallsTimer_Tick(object sender, EventArgs e)
        {
            for (int i = activeBalls.Count - 1; i >= 0; i--)
            {
                if (activeBalls[i].IsActive)
                {
                    activeBalls[i].Position.Y -= 9;
                    if (activeBalls[i].Position.Y + ballImage.Height < 0)
                    {
                        activeBalls[i].IsActive = false;
                    }
                }
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var ball in activeBalls)
            {
                if (ball.IsActive)
                {
                    e.Graphics.DrawImage(ballImage, ball.Position);
                }
            }
        }

        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player1.Left > 4)
            {
                Player1.Left -= playerSpeed;
                Player1.Visible = true;
                Player2.Visible = false;
                Player2.Left = Player1.Left;
            }
        }

        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player2.Right < 630)
            {
                Player2.Left += playerSpeed;
                Player2.Visible = true;
                Player1.Visible = false;
                Player1.Left = Player2.Left;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                RightMoveTimer.Start();
            }
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                LeftMoveTimer.Start();
            }
            if (e.KeyCode == Keys.Space)
            {
                Fire();
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                RightMoveTimer.Stop();
            }
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                LeftMoveTimer.Stop();
            }
        }
    }
}