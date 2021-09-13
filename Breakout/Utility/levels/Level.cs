
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

using Breakout.Render;
using Breakout.GameObjects;
using Breakout.GameObjects.Augments;

namespace Breakout.Utility
{
    class Level : GameComponant
    {
        // constants
        public const int TILE_SIZE          = 16;
        private const int CEILING            = 2;
        private const int AUGMENT_TYPES     = 2;
        private const int AUGMENT_AMOUNT    = 10;
        private const int BRICK_TILE        = 7;
        private const int BASE_VALUE        = 12;

        // fields

        private Tileset tileset;

        private int brickCount;
        private List<Brick> bricks;
        private Action<int> onBrickHit;

        private List<Augment> augments;

        private readonly int rangeStart;
        private readonly int rangeEnd;
        private readonly int width;
        protected readonly int widthPixels;
        protected readonly int rows;

        // gets the bricks in the level
        public List<Brick> Bricks 
        { 
            get => bricks; 
            set => bricks = value; 
        }

        // called when a brick is hit
        // passing in the index of the collided brick
        public Action<int> OnBrickHit 
        { 
            get => onBrickHit;
            set => onBrickHit = value; 
        }

        // properties
        public int RowSize      => widthPixels / TILE_SIZE;
        public virtual int BrickCount   => bricks.Count;
        public int Ceiling      => BreakoutGame.HasCeiling ? CEILING * TILE_SIZE : 0;
        public int AugmentCount => augments.Count();

        // constructor
        public Level(int rows, int widthPixels, Tileset tileset, int rangeStart, int rangeEnd)
        {
            // initalize fields
            this.widthPixels    = widthPixels;
            this.tileset        = tileset;
            this.rangeStart     = rangeStart;
            this.rangeEnd       = rangeEnd - 1;
            this.rows           = rows;

            // add augments
            this.augments = new List<Augment>(AUGMENT_AMOUNT);
            for (int _ = 0; _ < AUGMENT_AMOUNT / AUGMENT_TYPES; _++)
            {
                augments.Add(new TripleBallAugment());
                augments.Add(new ExplodingBallAugment());
            }

            // calculate fields
            width = widthPixels / TILE_SIZE;
            brickCount = width * rows;

            // empty lambda
            onBrickHit = index => { };
        }

        // tries to drop an augment
        public bool DropAugment(out Augment augment, Brick brick)
        {
            // chooses wheather to drop or not
            bool drop = Random.Next(1, 11) == 1 && augments.Count > 0;

            // choose a random augment
            int index = Random.Next(0, augments.Count);
            augment = drop ? augments[index] : null;

            if (drop)
            {
                // set the augmnets postion and remove it from the pool
                augment.X = brick.X;
                augment.Y = brick.Y;
                augments.RemoveAt(index);
            }

            return drop;
        }

        // add the brick objects to the game
        public virtual void Build()
        {
            // create bricks
            bricks = new List<Brick>(brickCount);
            for (int i = 0; i < brickCount; i++)
            {
                // calculate postion of the brick
                float x = i * TILE_SIZE % widthPixels;
                float y = Ceiling + (float)Math.Floor((float)i / TILE_SIZE) * TILE_SIZE;

                // randomise a tile
                int id = Random.Next(rangeStart, rangeEnd);

                // span the "brick" tile
                int span = 1;
                if (id == BRICK_TILE)
                {
                    span = 2;
                    if (x + TILE_SIZE < widthPixels) i++;
                }

                // calculate value and density
                int density = Brick.Map[id];
                int value = BASE_VALUE;

                // add bricks
                bricks.Add(new Brick(x, y, tileset.Texture, tileset.GetTile(id), span, value, density));
            }

            // add the bricks to the game
            int count = 0;
            bricks.ForEach(b => 
            {
                count++;
                BreakoutGame.QueueTask(Time.HUNDREDTH_SECOND * count, () => BreakoutGame.AddGameObject(b));  
            });
        }

        public virtual void Free()
            => Bricks.ForEach(b => BreakoutGame.QueueFree(b));
    }
}
