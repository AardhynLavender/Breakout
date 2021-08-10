
//
//  Tileset Class
//
//  Provides image postition calculations for 'tiles' on a single Image acting as a tileset


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public Rectangle GetTile(int index)
            => new Rectangle
            {
                X = index % (width / tileWidth) * tileWidth,
                Y = (int)Math.Floor((float)index / (width / tileWidth)) * tileHeight,
                Width = tileWidth,
                Height = tileHeight
            };
    }
}
