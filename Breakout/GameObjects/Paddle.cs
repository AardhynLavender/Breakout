
//
//  Paddle Class
//
//  A Game object that follows the mouses Y position and
//  takes part in the games physics simulation.
//

using Breakout.Render;
using System.Collections.Generic;
using System.Drawing;

namespace Breakout.GameObjects
{
    class Paddle : GameObject
    {
        // constants

        private const int Z_INDEX = 50;
        private const int PADDLE = 36;
        private const int PADDLE_TILES = 3;
        private const int PADDLE_FLOOR = 13;

        // fields

        private Animation animation;

        public Animation Animation
        {
            get => animation;
            set => animation = value;
        }

        // constructor

        public Paddle()
            : base (0, 0, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(PADDLE), Z_INDEX, PADDLE_TILES)
        {
            // place paddle above the screen bottom
            Y = Screen.HeightPixels - PADDLE_FLOOR;

            // add an animation to flash the paddle
            animation = BreakoutGame.AddAnimation(new Animation(
                BreakoutGame,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Tileset.GetTile(PADDLE + PADDLE_TILES, PADDLE_TILES),
                    BreakoutGame.Tileset.GetTile(PADDLE + PADDLE_TILES * 2, PADDLE_TILES)
                },
                BreakoutGame.Tileset,
                50
            ));
        }

        // called to update the paddles position
        public override void Update()
        {
            // update paddle position preventing offscreen translation
            if (Screen.MouseX / BreakoutGame.Scale - Width / 2 < 0)
                X = 0;
            else if (Screen.MouseX / BreakoutGame.Scale + Width / 2 > Screen.WidthPixels)
                X = Screen.WidthPixels - Width;
            else 
                X = Screen.MouseX / BreakoutGame.Scale - Width / 2;
        }
    }
}
