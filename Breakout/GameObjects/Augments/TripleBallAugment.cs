
//
//  TripleBallAugment:Augment Class
//
//  Resets the balls position and adds two additional balls to the
//  game rejecting the augment when there is only one ball left.
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
        private const int TEXTURE = 18;
        private const int ANIMATION_SPEED = 50;
        private Animation animation;

        public TripleBallAugment(BreakoutGame game)
            : base(game, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(TEXTURE))
        {
            animation = game.AddAnimation(new Animation(
                breakout,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Tileset.GetTile(TEXTURE),
                    BreakoutGame.Tileset.GetTile(TEXTURE + 1),
                    BreakoutGame.Tileset.GetTile(TEXTURE + 2),
                    BreakoutGame.Tileset.GetTile(TEXTURE + 3)
                },
                BreakoutGame.Tileset,
                ANIMATION_SPEED,
                loop: true
            ));

            animation.Animating = true;
        }

        protected override void apply()
        {
            Console.WriteLine("Augment Applied!");
            breakout.PlaySound(Properties.Resources.powerup);
            breakout.PaddleAugmentEffect.Animating = true;

            breakout.StartBall();
            int x = (int)breakout.BallPosition.X;
            int y = (int)breakout.BallPosition.Y;

            Ball a = new Ball(x - breakout.Ball.Width, y, 0, 0);
            Ball b = new Ball(x + breakout.Ball.Width, y, 0, 0);

            breakout.Balls.Add((Ball)breakout.AddGameObject(a));
            breakout.Balls.Add((Ball)breakout.AddGameObject(b));

            // add to game and balls list
            breakout.QueueTask(1000, () =>
            {
                a.Velocity = new Vector2D(1, 5);
                b.Velocity = new Vector2D(-1, 5);
            });
        }

        protected override void reject() 
        {
            Console.WriteLine("rejected!");
            breakout.PaddleAugmentEffect.Animating = false;
            breakout.ClearAugment();
        }

        protected override bool condition()
            => breakout.BallCount == 1;
    }
}
