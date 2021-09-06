using Breakout.Render;
using Breakout.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.GameObjects.Augments
{
    class ExplodingBallAugment : Augment
    {
        private const int TEXTURE = 22;

        private Animation animation;
        private Animation ballAnimation;

        public ExplodingBallAugment()
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

            animation.Start();
        }

        protected override void apply()
        {
            Console.WriteLine("Augment applied!");

            // play sound
            BreakoutGame.PlaySound(Properties.Resources.powerup);
            BreakoutGame.Paddle.Animation.Animating = true;

            // reset ball
            BreakoutGame.StartBall();

            // when the next brick is hit...
            BreakoutGame.CurrentLevel.OnBrickHit = index =>
            {
                Random random = new Random();

                // destory brick
                BreakoutGame.ExplodeBrick(BreakoutGame.CurrentLevel.Bricks[index]);
                for (int _ = 0; _ < BreakoutGame.CurrentLevel.BrickCount / 10; _++)
                    BreakoutGame.ExplodeBrick(BreakoutGame.CurrentLevel.Bricks[random.Next(0, BreakoutGame.CurrentLevel.BrickCount)]);
                    
                reject();
            };
        }

        protected override void reject()
        {
            BreakoutGame.CurrentLevel.OnBrickHit = brick => { };
            BreakoutGame.Paddle.Animation.Animating = false;

            BreakoutGame.ClearAugment();
            BreakoutGame.StartBall();
        }
    }
}
