using Breakout.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.GameObjects
{
    class Paddle : GameObject
    {
        private const int PADDLE = 36;
        private const int PADDLE_TILES = 3;
        private const int PADDLE_FLOOR = 13;

        private Animation animation;

        public Animation Animation
        {
            get => animation;
            set => animation = value;
        }

        public Paddle()
            : base (0, 0, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(PADDLE), PADDLE_TILES)
        {
            Y = Screen.HeightPixels - PADDLE_FLOOR;

            animation = BreakoutGame.AddAnimation(new Animation(
                BreakoutGame,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Tileset.GetTile(PADDLE + PADDLE_TILES, PADDLE_TILES),
                    BreakoutGame.Tileset.GetTile(PADDLE + PADDLE_TILES * 2, PADDLE_TILES)
                },
                BreakoutGame.Tileset,
                50,
                BreakoutGame.Tileset.GetTile(PADDLE, PADDLE_TILES)
            ));
        }

        public override void Update()
        {
            // update paddle position
            if (Screen.MouseX / BreakoutGame.Scale - Width / 2 < 0)
                X = 0;
            else if (Screen.MouseX / BreakoutGame.Scale + Width / 2 > Screen.WidthPixels)
                X = Screen.WidthPixels - Width;
            else 
                X = Screen.MouseX / BreakoutGame.Scale - 24;
        }
    }
}
