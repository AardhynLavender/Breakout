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
        Game breakout;

        public Form1()
        {
            InitializeComponent();
            //breakout = new Game(new Render.Screen(CreateGraphics()));
        }
    }
}
