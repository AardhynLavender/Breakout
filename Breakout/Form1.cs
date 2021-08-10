
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Breakout.Render;

namespace Breakout
{
    public partial class Form1 : Form
    {
        BreakoutGame breakout;

        public Form1()
        {
            InitializeComponent();
            breakout = new BreakoutGame(new Render.Screen(CreateGraphics(), Width, Height));
            ticker.Start();
        }

        private void ticker_Tick(object sender, EventArgs e)
        {
            breakout.GameLoop();
        }
    }
}
