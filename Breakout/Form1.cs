
//
//  Program name:           Atari Breakout
//  Project file name:      Breakout
//
//  Author:                 Aardhyn Lavender
//  Date:                   10/08/2021
//
//  Language:               C#
//  Platform:               Microsoft Visual Studio 2019
//
//  Purpose:                Demonstrate understanding of event driven programming and the object orientated paradigm.
//
//  Description:            An adapation of Atari Breakout wherin the user controls a paddle to destroy 'bricks' along 
//                          the screen top for points without loosing the ball.
//
//  Known Bugs:             None
//
//  Additional Features:  * Lorem Ipsum Dolor sit Amet 
//

using System;
using System.Media;
using System.Windows.Forms;

using Breakout.Render;

namespace Breakout
{
    partial class Form1 : Form
    {
        private static BreakoutGame breakout;

        #region Move Window without Title bar -- Thanks to: https://www.codeproject.com/Articles/11114/Move-window-form-without-Titlebar-in-C

        // keycodes
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        // extern functions
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        public Form1()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;

            Width = (16 * 4) * 12;
            Height = (16 * 4) * 12;

            Breakout = new BreakoutGame(new Render.Screen(CreateGraphics(), Width, Height), new SoundPlayer(), ticker)
            { Quit = () => Application.Exit() };

            ticker.Start();
        }

        public static BreakoutGame Breakout
        {
            get => breakout;
            set => breakout = value;
        }

        private void ticker_Tick(object sender, EventArgs e)
        {
            breakout.GameLoop();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            breakout.Screen.MouseX = e.X;
            breakout.Screen.MouseY = e.Y;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            breakout.Screen.MouseDown = true;

            if (e.Button == MouseButtons.Left && e.Y < 50 && e.X < Width - 40)
            {
                breakout.Screen.MouseDown = false;
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            breakout.Screen.MouseDown = false;
        }
    }
}
