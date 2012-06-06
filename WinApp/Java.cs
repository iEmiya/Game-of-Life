///////////////////////////////////////////////////////////////////
/* Bezier Surface, Copyright 2001-2010 Ryoichi Mizuno            */
/* ryoichi[at]mizuno.org                                         */
/* Dept. of Complexity Science and Engineering                   */
/* at The University of Tokyo                                    */
///////////////////////////////////////////////////////////////////
// http://www.mizuno.org/applet/gameOfLife/gameOfLife.java.shtml //
///////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;
using GameOfLife.Core;

namespace GameOfLife.WinApp
{
    internal sealed partial class Java : Form, IView
    {
        private IGame _game;

        public Java()
        {
            InitializeComponent();
        }

        private void Java_Load(object sender, EventArgs e)
        {
            _game = Game.Create(this, pb1.Size);
            pb1.MouseMove += new MouseEventHandler(MouseMoved);
        }

        private void Java_FormClosed(object sender, FormClosedEventArgs e)
        {
            _game.Destroy();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            pb1.Image = _game.PaintImage();
        }

        #region Events

        private void bt1_Click(object sender, EventArgs e)
        {
            _game.StartStopGame();
        }

        private void bt2_Click(object sender, EventArgs e)
        {
            _game.ClearCell();
        }

        private void bt3_Click(object sender, EventArgs e)
        {
            _game.RandomStart();
        }

        private void bt4_Click(object sender, EventArgs e)
        {
            _game.GliderStart();
        }

        private void cb1_CheckedChanged(object sender, EventArgs e)
        {
            _game.SelectOrDeselectNoise(cb1.Checked);
        }

        private void MouseClicked(object sender, MouseEventArgs e)
        {
            _game.AddDelLife(e.X, e.Y);
        }

        private void MouseMoved(object sender, MouseEventArgs e)
        {
            _game.ShowCellStatus(e.X, e.Y);
        }

        #endregion

        public void InvokeRefresh()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(InvokeRefresh));
                return;
            }
            this.Refresh();
        }

        public void ShowCellStatus(string msg)
        {
            return;
            throw new NotImplementedException();
        }

        public void SetGameStatus(bool start)
        {
            if (start)
                bt1.Text = @"stop";
            else
                bt1.Text = @"start";
        }
    }
}
