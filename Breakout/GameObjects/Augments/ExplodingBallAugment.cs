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
        private const int DESTRUCTION_COUNT = 10;

        private Animation animation;

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
            BreakoutGame.Paddle.Animation.Start();
            BreakoutGame.AddAnimation(BreakoutGame.Ball.FluxBallAnimation).Start();

            // reset ball
            BreakoutGame.StartBall();

            // when the next brick is hit...
            BreakoutGame.CurrentLevel.OnBrickHit = index =>
            {
                Random random = new Random();

                // choose amount of bricks to destroy
                int amount = (BreakoutGame.CurrentLevel.BrickCount < DESTRUCTION_COUNT) 
                    ? BreakoutGame.CurrentLevel.BrickCount 
                    : DESTRUCTION_COUNT;

                // destory bricks
                for (int _ = 0; _ < amount; _++)
                {
                    // get a random brick
                    Brick brick = BreakoutGame.CurrentLevel.Bricks[random.Next(0, BreakoutGame.CurrentLevel.BrickCount)];

                    // create a 'zap' object and an animation for it
                    GameObject zap = new GameObject(brick.X, brick.Y, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(49), z: 80, ghost: true);
                    BreakoutGame.AddAnimation(new Animation(
                        BreakoutGame,
                        zap,
                        new List<Rectangle>
                        {
                            BreakoutGame.Tileset.GetTile(49),
                            BreakoutGame.Tileset.GetTile(50),
                            BreakoutGame.Tileset.GetTile(51)
                        },
                        BreakoutGame.Tileset,
                        random.Next(Time.TWENTYTH_SECOND, Time.TENTH_SECOND),
                        () => BreakoutGame.QueueFree(zap),
                        loop:false
                    )).Start();

                    // add zap and explode brick
                    BreakoutGame.AddGameObject(zap);
                    brick.Explode();
                }
                    
                reject();
            };
        }

        protected override void reject()
        {
            // remove callback
            BreakoutGame.CurrentLevel.OnBrickHit = brick => { };

            // stop animations
            BreakoutGame.Paddle.Animation.Stop();
            BreakoutGame.Ball.FluxBallAnimation.Stop();

            // restore original gameplay
            BreakoutGame.ClearAugment();
            BreakoutGame.StartBall();
        }
    }
}
