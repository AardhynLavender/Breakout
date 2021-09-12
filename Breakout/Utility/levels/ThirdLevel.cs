

using Breakout.GameObjects;
using Breakout.Render;

namespace Breakout.Utility.levels
{
    class ThirdLevel : SecondLevel
    {
        // constants
        const int WORM_COUNT = 3;


        // fields
        private Worm[] worms;
        // For reference the worms have the names
        // Alice, Speedy, and Winnie.
        // (in no particular order)

        // properties
        public Worm[] Worms
        {
            get => worms;
            set => worms = value;
        }

        // constructor
        public ThirdLevel(int rows, int widthPixels, Tileset tileset, int rangeStart, int rangeEnd)
            : base(rows, widthPixels, tileset, rangeStart, rangeEnd)
        {
            worms = new Worm[WORM_COUNT];
        }

        public override void Build()
        {
            // build on previous level's functionality
            base.Build();

            // add worms
            for (int i = 0; i < WORM_COUNT; i++)
                worms[i] = (Worm)BreakoutGame.AddGameObject(
                    new Worm(Ceiling + rows * TILE_SIZE + (TILE_SIZE * i))
                );
        }

        public override void Free()
        {
            base.Free();
            foreach (Worm worm in worms)
                BreakoutGame.QueueFree(worm);
        }
    }
}
