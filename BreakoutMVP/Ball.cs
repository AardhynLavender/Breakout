
//
//  Ball Class
//
//  A derived GameObject that describes an object that
//  bounces off walls, paddles, bricks (also
//  destroying them)
//
//  I use a Action<Brick> delegate to give Ball a
//  delegate function from Game to delete the brick
//  it has collided with.
//
//  The 'deleteQueue' allows the delayed removal of
//  items from a list as a collection cannot be modified
//  while being looped.
//

using System;
using System.Collections.Generic;
using System.Drawing;

namespace BreakoutMVP
{
    class Ball : GameObject
    {
        // constants
        private const int ANGLE_MULTIPLIER = 5;
        private const int SIZE = 10;

        // fields
        private float velocityX;
        private float velocityY;

        private int screenWidth;
        private int screenHeight;

        private Paddle paddle;

        private List<Brick> bricks;
        private List<Brick> deleteQueue;
        private Action<Brick> destroyBrick;

        // constructors
        public Ball(int x, int y, int width, int height, Paddle paddle, List<Brick> bricks, Action<Brick> destoryBrick)
            : base(x, y)
        {
            // initalize fields
            velocityX = 0.0f;
            velocityY = 5.0f;
            screenWidth = width; 
            screenHeight = height;

            this.width  = this.height = SIZE;
            this.paddle = paddle;
            this.bricks = bricks;
            this.destroyBrick = destoryBrick;

            // create delete queue
            deleteQueue = new List<Brick>();
        }

        public override void Update()
        {
            // bounce of walls
            if (x + velocityX + SIZE > screenWidth || x + velocityX < 0)
                velocityX *= -1;
            if (y + velocityY + SIZE > screenHeight || y + velocityY < 0)
                velocityY *= -1;

            // bounce off paddle
            if (x + SIZE > paddle.X 
                && x < paddle.X + paddle.Width
                && y + velocityY + SIZE > paddle.Y
                && y + velocityY < paddle.Y)
            {
                velocityX = (x - paddle.X - paddle.Width / 2) / paddle.Width * ANGLE_MULTIPLIER;
                velocityY *= -1;
            }

            // bounce off and destory bricks
            foreach (Brick brick in bricks)
            {
                // check collison with brick on x axis
                if (x + velocityX + SIZE > brick.X
                    && x + velocityX < brick.X + brick.Width
                    && y + SIZE > brick.Y
                    && y < brick.Y + brick.Height)
                {
                    velocityX *= -1;

                    // destroy brick
                    destroyBrick(brick);
                    deleteQueue.Add(brick);
                }
                // check collision with brick on y axis
                else if (x + SIZE > brick.X
                    && x < brick.X + brick.Width
                    && y + velocityY + SIZE > brick.Y 
                    && y + velocityY < brick.Y + brick.Height)
                {
                    velocityY *= -1;

                    // destory brick
                    destroyBrick(brick);
                    deleteQueue.Add(brick);
                }
            }

            // update positon
            x += velocityX;
            y += velocityY;

            // delete bricks in the queue
            deleteQueue.ForEach(brick => bricks.Remove(brick));
            deleteQueue.Clear();
        }

        public override void Draw()
        {
            int radius = SIZE / 2;
            canvas.DrawEllipse(Pens.Red, x - radius, y - radius, SIZE, SIZE);
        }
    }
}