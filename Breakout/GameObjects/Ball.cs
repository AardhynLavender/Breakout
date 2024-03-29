﻿
//
//  Ball Class
//
//  A derived GameObject that defines an object with a ball texture
//  Balls bounce of bricks, worms, walls, and paddles based on 
//

using Breakout.Render;
using Breakout.Utility;
using Breakout.Utility.levels;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Breakout.GameObjects
{
    class Ball : GameObject
    {
        private const int ANGLE_MULTIPLIER = 5;

        private Animation shinyBallAnimation;
        private Animation fluxBallAnimation;

        public Animation ShinyBallAnimation => shinyBallAnimation;
        public Animation FluxBallAnimation => fluxBallAnimation;

        public Ball(int x = 0, int y = 0)
            : base(x, y, BreakoutGame.Ballset.Texture, BreakoutGame.Ballset.GetTile(0))
        {
            shinyBallAnimation = new Animation(
                BreakoutGame,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Ballset.GetTile(1),
                    BreakoutGame.Ballset.GetTile(2),
                    BreakoutGame.Ballset.GetTile(3)
                },
                BreakoutGame.Ballset,
                Time.TWENTYTH_SECOND
            );

            fluxBallAnimation = new Animation(
                BreakoutGame,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Ballset.GetTile(4),
                    BreakoutGame.Ballset.GetTile(5),
                    BreakoutGame.Ballset.GetTile(6)
                },
                BreakoutGame.Ballset,
                Time.TWENTYTH_SECOND
            );
        }

        public override void Physics()
        {
            // move ball by its X velocity, bouncing off vertical walls
            if (X + Velocity.X < 0 || X + Velocity.X + Width > Screen.WidthPixels)
            {
                Velocity.X *= -1;
                BreakoutGame.PlaySound(Properties.Resources.bounce);
            }
            else X += Velocity.X;

            // move by its Y velocity, bouncing off top wall
            if (Y + Velocity.Y < 0)
            {
                Velocity.Y *= -1;
                BreakoutGame.PlaySound(Properties.Resources.bounce);
            }
            else if (Y + Velocity.Y > Screen.HeightPixels + 10)
            {
                if (BreakoutGame.HasFloor) 
                    Velocity.Y *= -1;

                else
                {
                    // has fallen off the screen
                    BreakoutGame.PlaySound(Properties.Resources._break);

                    if (BreakoutGame.BallCount <= 1)
                    {
                        if (!BreakoutGame.HasInfiniteLives) 
                            BreakoutGame.Lives--;

                        if (BreakoutGame.Lives > 0) 
                            BreakoutGame.StartBall();
                    }
                    else
                    {
                        Velocity.Zero();
                        BreakoutGame.Balls.Remove(this);
                    }
                }
            }
            else Y += Velocity.Y;

            // bounce off bricks
            for (int i = 0; i < BreakoutGame.CurrentLevel.Bricks.Count; i++)
            {
                Brick brick = BreakoutGame.CurrentLevel.Bricks[i];

                // invert Y velocity if colliding on the vertical sides
                if (X + Velocity.X < brick.X + brick.Width
                    && X + Velocity.X + Width > brick.X
                    && Y < brick.Y + brick.Height
                    && Y > brick.Y)
                {
                    BreakoutGame.BrickHit(i);
                    BreakoutGame.CurrentLevel.OnBrickHit(i);

                    if (brick is RegrowthBrick && y < brick.Y) 
                        continue;

                    Velocity.X *= -1;
                }
                // invert X velocity of colliding on the horizontal sides
                else if (x < brick.X + brick.Width
                    && x + Width > brick.X
                    && y + Velocity.Y < brick.Y + brick.Height
                    && y + Velocity.Y > brick.Y)
                {
                    BreakoutGame.BrickHit(i);
                    BreakoutGame.CurrentLevel.OnBrickHit(i);

                    if (brick is RegrowthBrick && y < brick.Y)
                        continue;

                    Velocity.Y *= -1;
                }
            }

            // bounce off worms should level cast to ThirdLevel
            if (BreakoutGame.CurrentLevel is ThirdLevel level)
            {
                // bounce off worms above the ball
                foreach (Worm worm in level.Worms)
                {
                    if (Game.DoesCollide(worm, this))
                        Velocity.Y = 5;
                }
            }

            // bounce of paddle, applying angular velocity depending
            // on the collison point on the paddle
            if (X <= BreakoutGame.Paddle.X + BreakoutGame.Paddle.Width
                && X + Width > BreakoutGame.Paddle.X
                && Y + Velocity.Y <= BreakoutGame.Paddle.Y + BreakoutGame.Paddle.Height
                && Y + Height + Velocity.Y > BreakoutGame.Paddle.Y)
            {
                BreakoutGame.PlaySound(Properties.Resources.bounce);

                // bounce ball applying angular velocity
                // based on the collison point using an
                // angle multiplier constant
                Velocity.X = (X - BreakoutGame.Paddle.X - BreakoutGame.Paddle.Width / 2) / BreakoutGame.Paddle.Width * ANGLE_MULTIPLIER;
                
                Velocity.Y *= -1;
            }
        }
    }
}
