
//
//  Regrowth Brick Class
//
//  Defines a brick that adds itself back into the game
//  after a specified time period with a leafy texture.
//

using Breakout.Utility;
using System.Collections.Generic;

namespace Breakout.GameObjects
{
    class RegrowthBrick : Brick
    {
        // constants
        private const int TEXTURE = 52;
        private const int REGROWTH_DELAY = Time.SECOND * 10;

        // fields
        private bool regrow;
        private List<Brick> bricks;

        // properties
        public bool Regrow
        {
            get => regrow;
            set => regrow = value;
        }

        public RegrowthBrick(float x, float y, List<Brick> bricks)
            : base(x, y, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(TEXTURE), 1, 0, 1)
        {
            regrow = true;
            this.bricks = bricks;
        }

        public override void OnAddGameObject()
        {
            if (regrow)
            {
                sourceRect = BreakoutGame.Tileset.GetTile(TEXTURE + 1);
                BreakoutGame.QueueTask(Time.TENTH_SECOND, () => sourceRect = BreakoutGame.Tileset.GetTile(TEXTURE));
            }
        }

        public override void OnFreeGameObject()
        {
            sourceRect = BreakoutGame.Tileset.GetTile(TEXTURE + 1);
            BreakoutGame.QueueTask(REGROWTH_DELAY, () =>
            {
                // reinitalize the explosion
                InitalizeExplosion();

                // add the brick back in if the games still running
                if (BreakoutGame.LevelRunning && regrow)
                    bricks.Add((Brick)BreakoutGame.AddGameObject(this));
            });
        }
    }
}
