
//
//  Tileset Class
//
//  Allow a single image to be treated as a tileset with
//  get tile methods using Rectangles.
//

using System;
using System.Drawing;

namespace Breakout.Render
{
    class Tileset
    {
        // fields
        private int width;

        private int tileWidth;
        private int tileHeight;

        private Image texture;

        // the tilset texture
        public Image Texture 
        { 
            get => texture; 
            set => texture = value; 
        }

        // constructor
        public Tileset(Image texture, int width, int tileWidth, int tileHeight)
        {
            this.texture    = texture;
            this.width      = width;
            this.tileWidth  = tileWidth;
            this.tileHeight = tileHeight;
        }

        // creates a source rectangle from the given indx and span variables
        public Rectangle GetTile(int index, int spanX = 1, int spanY = 1)
            => new Rectangle
            {
                X       = index % (width / tileWidth) * tileWidth,
                Y       = (int)Math.Floor((float)index / (width / tileWidth)) * tileHeight,
                Width   = tileWidth * spanX,
                Height  = tileHeight * spanY
            };
    }
}
