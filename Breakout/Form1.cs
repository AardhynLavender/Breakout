
//
//  Program name:           Atari Breakout
//  Project file name:      Breakout
//
//  Author:                 Aardhyn Lavender
//  Date:                   17/09/2021
//
//  Language:               C#
//  Platform:               Microsoft Visual Studio 2019
//
//  Purpose:                Demonstrate understanding of event driven programming and the object orientated paradigm.
//
//  Description:            Create an Atari Breakout like game modelling its physics, abstract design, and general gameplay logic.
//                          Design the software using Microsoft WinForms demonstrating understanding of the object orientated
//                          paradigm and event driven programming.
//
//  Known Bugs:             Occasional Collison tunneling with ball and worm due to the collective speed.
//
//  Additional Features:
//  
//  * 10 points changed to 12 and further multiples.
//  * Menu system with Guide, Options, and Credits.
//  * Levels with extra functionality such as regrowth bricks and worms.
//  * Augmentation with 'powerups' - Triple ball powerup, and random brick exploding.
//  * Player looses the game after 3 attempts rather than one
//  * Options menu allows the user to configure some aspects of the game:
//      > infinite lives -- player can loose the ball without loosing a life
//      > floor mode -- a floor is added below the paddle to prevent loosing the ball (mostly for debugging)
//      > Augmentation toggle -- prevents augments from spawning
//      > sfx toggle
//      > ceiling mode to remove the gap above the bricks 
//      > levels mode to add additional levels to the game 
//

using System;
using System.Media;
using System.Windows.Forms;

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
            // initalize components and window
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            Cursor.Hide();

            // calculate window size that fits in all the tiles
            Width = (16 * 4) * 12;
            Height = (16 * 4) * 12;

            // create brekout and assign it a quit method
            breakout = new BreakoutGame(new Render.Screen(CreateGraphics(), Width, Height), new SoundPlayer(), ticker)
            { Quit = () => Application.Exit() };

            // start the game timer
            ticker.Start();
        }

        // called per time tick
        private void ticker_Tick(object sender, EventArgs e)
            => breakout.GameLoop();

        // pass the mouses postion onto the Games screen
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            breakout.Screen.MouseX = e.X;
            breakout.Screen.MouseY = e.Y;
        }

        // pass the mouse down event onto the games screen
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            breakout.Screen.MouseDown = true;

            #region check for conditions to move the window without a title bar
            if (e.Button == MouseButtons.Left && e.Y < 50 && e.X < Width - 40)
            {
                breakout.Screen.MouseDown = false;
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
            #endregion
        }

        // pass the mouse up event onto the games screen
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            breakout.Screen.MouseDown = false;
        }
    }
}
