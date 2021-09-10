

using Breakout.GameObjects;
using Breakout.Render;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Utility.levels
{
    class ThirdLevel : SecondLevel
    {
        // constants
        const int WORM_COUNT = 3;

        // fields
        private List<Worm> worms;

        // constructor
        public ThirdLevel(int rows, int widthPixels, Tileset tileset, int rangeStart, int rangeEnd)
            : base(rows, widthPixels, tileset, rangeStart, rangeEnd)
        {
            worms = new List<Worm>(WORM_COUNT);
        }

        public override void Build()
        {
            // build on previous level's functionality
            base.Build();

            // add worms
            for (int i = 0; i < WORM_COUNT; i++)
                worms.Add((Worm)BreakoutGame.AddGameObject(
                    new Worm(Ceiling + rows * TILE_SIZE + (TILE_SIZE * i)))
                );
        }
    }
}
