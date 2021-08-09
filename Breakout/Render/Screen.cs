using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Breakout.Render
{
    public sealed class Screen
    {
        public Graphics Buffer;

        private Image BufferImage;
        private Graphics Display;

        public int Width;
        public int Height; 

        public Screen(Graphics display)
        {
            BufferImage = new Bitmap(Width, Height);
            Buffer = Graphics.FromImage(BufferImage);
            Display = display;
        }

        public void RenderClear()
            => Buffer.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height));

        public void RenderPresent()
            => Display.DrawImage(BufferImage, 0, 0);
    }
}
