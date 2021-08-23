using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout;
using Breakout.Render;
using Breakout.GameObjects;

namespace Breakout.Utility
{
    class Level
    {
        public const int TILE_SIZE = 16;
        public const int CEILING = 3;

        private Random random;
        private Tileset tileset;

        private int brickCount;
        private List<Brick> bricks;

        private int rangeStart;
        private int rangeEnd;
        private int widthPixels;
        private int width;

        public List<Brick> Bricks 
        { 
            get => bricks; 
            set => bricks = value; 
        }

        public Level(Random random, int rows, int widthPixels, Tileset tileset, int rangeStart, int rangeEnd)
        {
            // initalize fields
            this.random         = random;
            this.widthPixels    = widthPixels;
            this.tileset        = tileset;
            this.rangeStart     = rangeStart;
            this.rangeEnd       = rangeEnd;

            // calculate fields
            width = widthPixels / TILE_SIZE;
            brickCount = width * rows;
        }

        public void InitalizeLevel() 
        {
            bricks = new List<Brick>(brickCount);
            for (int i = 0; i < brickCount; i++)
            {
                // calculate postion of the brick
                float x = i * TILE_SIZE % widthPixels;
                float y = CEILING * TILE_SIZE + (float)Math.Floor((float)i / TILE_SIZE) * TILE_SIZE;

                // randomise a tile
                int id = random.Next(rangeStart, rangeEnd);

                // span the "brick" tile
                int span = 1;
                if (id == 7)
                {
                    span = 2;
                    if (x + TILE_SIZE < widthPixels) i++;
                }

                // calculate value and density
                int density = Brick.Map[id];
                int value = 12; 

                // add bricks
                bricks.Add(new Brick(x, y, tileset.Texture, tileset.GetTile(id), span, value, density, random));
            }
        }
    }
}
