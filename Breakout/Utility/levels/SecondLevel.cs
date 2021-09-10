
//
//  Second Level Class
//
//  Extends the functionality of the base level, adding
//  regrowing bricks along the bottom of the brick matrix.
//

using System;
using System.Collections.Generic;
using System.Linq;

using Breakout.GameObjects;
using Breakout.Render;

namespace Breakout.Utility.levels
{
    class SecondLevel : Level
    {
        private const int REGROW_ROWS = 1;

        // dont count regrow bricks as 'real' bricks
        public override int BrickCount
            => base.BrickCount - Bricks.FindAll(brick => brick is RegrowthBrick).Count;

        public SecondLevel(int rows, int widthPixels, Tileset tileset, int rangeStart, int rangeEnd)
            : base(rows, widthPixels, tileset, rangeStart, rangeEnd)
        {  }

        public override void Build()
        {
            base.Build();

            // calculate starting positon and amount of bricks to add
            int startY = Ceiling + rows * TILE_SIZE;
            int regrowthBricks = (widthPixels / TILE_SIZE) * REGROW_ROWS;

            // add regrowth bricks
            for (int i = 0; i < regrowthBricks; i++)
            {
                // calculate positon
                float x = i * TILE_SIZE % widthPixels;
                float y = startY + (float)Math.Floor((float)i / TILE_SIZE) * TILE_SIZE;

                // create regrowth brick
                RegrowthBrick regrowthBrick = new RegrowthBrick(x, y, Bricks);
                Bricks.Add(regrowthBrick);

                // add to game after the main bricks have been added
                BreakoutGame.QueueTask(Bricks.Count * Time.HUNDREDTH_SECOND, () => 
                    BreakoutGame.AddGameObject(regrowthBrick));
            }
        }
    }
}
