
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
        private int width;

        private int tileWidth;
        private int tileHeight;

        private Image texture;

        public Image Texture 
        { 
            get => texture; 
            set => texture = value; 
        }

        public Tileset(Image texture, int width, int tileWidth, int tileHeight)
        {
            this.texture    = texture;
            this.width      = width;
            this.tileWidth  = tileWidth;
            this.tileHeight = tileHeight;
        }
        
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
