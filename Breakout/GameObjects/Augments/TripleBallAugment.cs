
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
        private const int EXTRA_BALLS       = 2;
        private const int TEXTURE           = 18;
        private const int ANIMATION_SPEED   = 50;

        private Animation animation;
        private Animation[] ballAnimations;

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

            // create animations for them
            ballAnimations = new Animation[EXTRA_BALLS + 1];
            animation.Animating = true;
        }

        protected override void apply()
        {
            Console.WriteLine("Augment Applied!");

            // play sound
            breakout.PlaySound(Properties.Resources.powerup);
            breakout.PaddleAugmentEffect.Animating = true;

            // reset ball
            breakout.StartBall();
            int x = (int)breakout.BallPosition.X;
            int y = (int)breakout.BallPosition.Y;

            // create and add new balls beside original
            Ball a = new Ball(x - breakout.Ball.Width, y, 0, 0);
            Ball b = new Ball(x + breakout.Ball.Width, y, 0, 0);

            breakout.Balls.Add((Ball)breakout.AddGameObject(a));
            breakout.Balls.Add((Ball)breakout.AddGameObject(b));

            // add animations
            for (int i = 0; i < EXTRA_BALLS + 1; i++)
            {

                Animation animation = new Animation(
                    breakout,
                    breakout.Balls[i],
                    new List<Rectangle>
                    {
                        BreakoutGame.Ballset.GetTile(1),
                        BreakoutGame.Ballset.GetTile(2),
                        BreakoutGame.Ballset.GetTile(3)
                    },
                    BreakoutGame.Ballset,
                    ANIMATION_SPEED,
                    BreakoutGame.Ballset.GetTile(0)
                );

                animation.Animating = true;
                ballAnimations[i] = breakout.AddAnimation(animation);
            }

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

            // stop animations
            breakout.PaddleAugmentEffect.Animating = false;
            foreach (Animation animation in ballAnimations)
                animation.Animating = false;

            breakout.ClearAugment();
        }

        protected override bool condition()
            => breakout.BallCount == 1;
    }
}
