
//
//  Level Class
//
//  Defines a level object that randomly generates bricks
//  from a provided range on a tileset with augmentation
//  dropping functionality.
//

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
    class Level : GameComponant
    {
        public const int TILE_SIZE = 16;
        public const int CEILING = 2;

        private Random random;
        private Tileset tileset;

        private int brickCount;
        private List<Brick> bricks;
        private Action<int> onBrickHit;

        private bool augmentActive;
        private List<Augment> augments;

        private readonly int rangeStart;
        private readonly int rangeEnd;
        private readonly int widthPixels;
        private readonly int width;

        public List<Brick> Bricks 
        { 
            get => bricks; 
            set => bricks = value; 
        }

        public Action<int> OnBrickHit 
        { 
            get => onBrickHit;
            set => onBrickHit = value; 
        }

        public int RowSize      => widthPixels / TILE_SIZE;
        public int BrickCount   => bricks.Count;
        public int Ceiling      => BreakoutGame.HasCeiling ? CEILING * TILE_SIZE : 0;
        public int AugmentCount => augments.Count();


        public Level(Random random, int rows, int widthPixels, Tileset tileset, int rangeStart, int rangeEnd, List<Augment> augments)
        {
            // initalize fields
            this.random         = random;
            this.widthPixels    = widthPixels;
            this.tileset        = tileset;
            this.rangeStart     = rangeStart;
            this.rangeEnd       = rangeEnd - 1;

            // add augments
            this.augments = new List<Augment>(augments.Count);
            foreach (Augment augment in augments)
                this.augments.Add(augment);

            // calculate fields
            width = widthPixels / TILE_SIZE;
            brickCount = width * rows;

            onBrickHit = index => { };
        }

        public void InitalizeLevel() 
        {
            bricks = new List<Brick>(brickCount);
            for (int i = 0; i < brickCount; i++)
            {
                // calculate postion of the brick
                float x = i * TILE_SIZE % widthPixels;
                float y = Ceiling + (float)Math.Floor((float)i / TILE_SIZE) * TILE_SIZE;

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

        public bool DropAugment(out Augment augment, Brick brick)
        {
            bool drop = random.Next(1, 2) == 1 && augments.Count > 0;

            // choose a random augment
            int index = random.Next(0, augments.Count);
            augment = drop ? augments[index] : null;

            // if drop
            if (drop)
            {
                augment.X = brick.X;
                augment.Y = brick.Y;
                augments.RemoveAt(index);
            }

            return drop;
        }
    }
}
