
//
//  TripleBallAugment Class
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

        private Animation animation;
        private Animation[] ballAnimations;

        public TripleBallAugment()
            : base(BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(TEXTURE))
        {
            animation = BreakoutGame.AddAnimation(new Animation(
                BreakoutGame,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Tileset.GetTile(TEXTURE),
                    BreakoutGame.Tileset.GetTile(TEXTURE + 1),
                    BreakoutGame.Tileset.GetTile(TEXTURE + 2),
                    BreakoutGame.Tileset.GetTile(TEXTURE + 3)
                },
                BreakoutGame.Tileset,
                Time.TWENTYTH_SECOND,
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
            BreakoutGame.PlaySound(Properties.Resources.powerup);
            BreakoutGame.Paddle.Animation.Animating = true;

            // reset ball
            BreakoutGame.StartBall();
            int x = (int)BreakoutGame.BallPosition.X;
            int y = (int)BreakoutGame.BallPosition.Y;

            // create and add new balls beside original
            Ball a = new Ball(x - BreakoutGame.Ball.Width, y, 0, 0);
            Ball b = new Ball(x + BreakoutGame.Ball.Width, y, 0, 0);

            BreakoutGame.Balls.Add((Ball)BreakoutGame.AddGameObject(a));
            BreakoutGame.Balls.Add((Ball)BreakoutGame.AddGameObject(b));

            // add and start animations
            BreakoutGame.Balls.ForEach(ball => BreakoutGame.AddAnimation(ball.ShinyBallAnimation).Start());

            // add to game and balls list
            BreakoutGame.QueueTask(Time.SECOND, () =>
            {
                a.Velocity = new Vector2D(1, 5);
                b.Velocity = new Vector2D(-1, 5);
            });
        }

        public override void Reject() 
        {
            Console.WriteLine("rejected!");

            // stop animations
            BreakoutGame.Paddle.Animation.Stop();
            BreakoutGame.Balls.ForEach(ball => 
            { 
                ball.ShinyBallAnimation.Stop(); 
            });

            while (BreakoutGame.BallCount > 1)
            {
                BreakoutGame.QueueFree(BreakoutGame.Balls[0]);
                BreakoutGame.Balls.RemoveAt(0);
            }
            
            BreakoutGame.ClearAugment();
        }

        protected override bool condition()
            => BreakoutGame.BallCount <= 1;
    }
}
