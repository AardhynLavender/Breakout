using Breakout.Render;
using Breakout.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.GameObjects
{
    class RegrowthBrick : Brick
    {
        private const int TEXTURE = 52;
        private const int REGROWTH_DELAY = Time.SECOND * 10;

        private bool regrow;
        private List<Brick> bricks;

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
            sourceRect = BreakoutGame.Tileset.GetTile(TEXTURE + 1);
            BreakoutGame.QueueTask(Time.TENTH_SECOND, () => sourceRect = BreakoutGame.Tileset.GetTile(TEXTURE));
        }

        public override void OnFreeGameObject()
        {
            if (regrow)
            {
                sourceRect = BreakoutGame.Tileset.GetTile(TEXTURE + 1);
                BreakoutGame.QueueTask(REGROWTH_DELAY, () =>
                {
                    if (BreakoutGame.LevelRunning)
                        bricks.Add((Brick)BreakoutGame.AddGameObject(this));
                });
            }
        }
    }
}
