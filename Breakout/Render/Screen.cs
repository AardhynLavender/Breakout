
//
//  Screen Class
//
//  an intermediary class that provides functionality to a
//  Game to render textures to a Form screen with scaling
//  and "pixel-perfect" "double buffered" rendering
//

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

        public int Width    { get; set; }
        public int Height   { get; set; }
        public int Scale    { get; set; }

        public Screen(Graphics display, int width, int height)
        {
            Width = width;
            Height = height;

            BufferImage = new Bitmap(Width, Height);
            Buffer = Graphics.FromImage(BufferImage);
            Display = display;

            Buffer.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            Buffer.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        }

        public void RenderClear()
            => Buffer.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height));

        public void RenderCopy(Image texture, float x, float y, int width, int height)
        {
            Buffer.DrawImage(texture, x * Scale, y * Scale, width * Scale, height * Scale);
        }

        public void RenderCopy(Image texture, Rectangle src, Rectangle dest)
        {
            dest.X *= Scale;
            dest.Y *= Scale;
            dest.Width *= Scale;
            dest.Height *= Scale;

            Buffer.DrawImage(texture ,dest, src, GraphicsUnit.Pixel);
        }

        public void RenderPresent()
            => Display.DrawImage(BufferImage, 1, 1, Width, Height);
    }
}
