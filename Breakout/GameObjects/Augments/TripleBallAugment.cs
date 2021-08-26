
//
//  TripleBallAugment:Augment Class
//
//
//

using System;
using System.Drawing;

using Breakout.Utility;
using Breakout.Render;
using System.Collections.Generic;

namespace Breakout.GameObjects.Augments
{
    class TripleBallAugment : Augment
    {
        private const int EXTRA_BALLS = 2;
        private Animation animation;

        public TripleBallAugment(BreakoutGame game)
            : base(game, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(18))
        {
            animation = game.AddAnimation(new Animation(
                breakout,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Tileset.GetTile(18),
                    BreakoutGame.Tileset.GetTile(19),
                    BreakoutGame.Tileset.GetTile(20),
                    BreakoutGame.Tileset.GetTile(21)
                },
                BreakoutGame.Tileset,
                50,
                loop: true
            ));

            animation.Animating = true;
        }

        protected override void apply()
        {
            Console.WriteLine("Augment Applied!");

            int x = (int)breakout.BallPosition.X;
            int y = (int)breakout.BallPosition.Y;

            Ball a = new Ball(x, y, 0, 0) { Velocity = new Vector2D(-5, -5) };
            Ball b = new Ball(x, y, 0, 0) { Velocity = new Vector2D(5, -5) };

            // add to game and balls list
            breakout.Balls.Add((Ball)breakout.AddGameObject(a));
            breakout.Balls.Add((Ball)breakout.AddGameObject(b));
        }

        protected override void reject()
        {
            Console.WriteLine("rejected!");
            breakout.ClearAugment();
        }

        protected override bool condition()
            => breakout.BallCount == 1;
    }
}
