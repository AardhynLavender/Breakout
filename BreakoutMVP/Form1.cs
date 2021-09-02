
//
//  Program name:           Atari Breakout - Minimum Viable Product
//
//  Project file name:      BreakoutMVP
//  Author:                 Aardhyn Lavender
//  Date:                   02/09/2021
//
//  Language:               C#
//  Platform:               Microsoft Visual Studio 2019
//  Purpose:                Demonstrate understanding of event driven programming and the object orientated paradigm.
//
//  Description:            Create an Atari Breakout like game modeling its physics, abstract design, and general
//                          gameplay logic using Microsoft WinForms.
//                          Demonstrate understanding of the object orientated paradigm and event driven programming.
//
//  Known Bugs:             ball travels slightly off screen; paddle can go off screen slightly.
//  Additional Features:    paddle changes angular velocity of ball
//

using System.Windows.Forms;

namespace BreakoutMVP
{
    public partial class Form1 : Form
    {
        // fields and constants
        private const int TICKRATE = 17;
        private Game game;

        // constructor
        public Form1()
        {
            InitializeComponent();
            game = new Game(Width, Height, CreateGraphics());

            // setup the timer
            timer.Interval = TICKRATE;
            timer.Start();
        }

        // run the game loop every timer tick
        private void timer_Tick(object sender, System.EventArgs e)
        {
            game.GameLoop();
        }

        // tell the game where the mouse is located on the x axis
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            game.MousePosX = e.X;
        }
    }
}
