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
