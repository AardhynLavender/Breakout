
//
//  Second Level Class
//
//  Extends the functionality of the base level, adding
//  regrowing bricks along the bottom of the brick matrix.
//

using Breakout.GameObjects;
using Breakout.Render;
using System;
using System.Collections.Generic;

namespace Breakout.Utility.levels
{
    class SecondLevel : Level
    {
        private const int REGROW_ROWS = 1;

        private List<RegrowthBrick> RegrowthBricks;

        // dont count regrow bricks as 'real' bricks
        public override int BrickCount
            => base.BrickCount - Bricks.FindAll(brick => brick is RegrowthBrick).Count;

        public SecondLevel(int rows, int widthPixels, Tileset tileset, int rangeStart, int rangeEnd)
            : base(rows, widthPixels, tileset, rangeStart, rangeEnd)
        {
            RegrowthBricks = new List<RegrowthBrick>();
        }

        public override void Build()
        {
            base.Build();

            // calculate starting positon and amount of bricks to add
            int startY = Ceiling + rows * TILE_SIZE;
            int regrowthBrickCount = widthPixels / TILE_SIZE * REGROW_ROWS;

            // add regrowth bricks
            for (int i = 0; i < regrowthBrickCount; i++)
            {
                // calculate positon
                float x = i * TILE_SIZE % widthPixels;
                float y = startY + (float)Math.Floor((float)i / TILE_SIZE) * TILE_SIZE;

                // create regrowth brick
                RegrowthBrick regrowthBrick = new RegrowthBrick(x, y, Bricks);
                Bricks.Add(regrowthBrick);
                RegrowthBricks.Add(regrowthBrick);

                // add to game after the main bricks have been added
                BreakoutGame.QueueTask(Bricks.Count * Time.HUNDREDTH_SECOND, () => 
                    BreakoutGame.AddGameObject(regrowthBrick));
            }
        }

        public override void Free()
        {
            base.Free();

            // delete regrowth bricks perminantly
            RegrowthBricks.ForEach(rb =>
            {
                rb.Regrow = false;
                BreakoutGame.QueueFree(rb);
            });
        }
    }
}
