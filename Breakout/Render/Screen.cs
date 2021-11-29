
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
        // fields

        private Image bufferImage;

        private Graphics buffer;
        private Graphics display;

        private Rectangle src;
        private Rectangle dest;

        // properties

        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public bool MouseDown { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int WidthPixels
        { 
            get => Width / Scale; 
        }

        public Graphics Buffer 
        { 
            get => buffer; 
            set => buffer = value;
        }

        public int HeightPixels 
        { 
            get => Height / Scale; 
        }

        public int Scale    { get; set; }

        // constructor
        public Screen(Graphics display, int scale, int width, int height)
        {
            // initalize fields
            Width   = width;
            Height  = height;
            Scale   = scale;

            src = new Rectangle(0, 0, width, height);
            Console.WriteLine(Scale);
            dest = new Rectangle(0, 0, width * Scale, Height * Scale);

            // create buffers
            bufferImage     = new Bitmap(Width, Height);
            Buffer          = Graphics.FromImage(bufferImage);
            this.display    = display;

            // configure buffer
            display.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            display.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        }

        // clears the screen
        public void RenderClear()
            => Buffer.FillRectangle(Brushes.Black, new Rectangle(0, 0, Width, Height));

        // copies the buffer to the screen
        public void RenderCopy(Image texture, Rectangle src, Rectangle dest)
        {
            // draw the scalled image to the buffer
            Buffer.DrawImage(texture, dest, src, GraphicsUnit.Pixel);
        }

        // presents the buffer to the display
        public void RenderPresent()
            => display.DrawImage(bufferImage, dest, src, GraphicsUnit.Pixel);
    }
}
