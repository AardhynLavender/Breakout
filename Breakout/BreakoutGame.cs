
//
//  BreakOutGame:Game class
//
//  Defines the functionality and members to create and play Atari Breakout
//  with score counters, powerups, levels, and saving.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.Render;
using Breakout.GameObjects;
using System.Drawing;

namespace Breakout
{
    class BreakoutGame : Game
    {
        private const int SCALE             = 3;
        private const int TILE_SIZE         = 16;
        private const int BRICK_COUNT       = 15;
        private const int PADDLE_WIDTH      = TILE_SIZE * 3;
        private const int BRICK_TILE        = 6;

        private Ball ball;
        private GameObject paddle;
        private List<GameObject> bricks;

        private Random random;

        public static readonly Tileset tileset = 
            new Tileset(
                Properties.Resources.tileset, 
                Properties.Resources.tileset.Width, 
                TILE_SIZE, 
                TILE_SIZE
            );

        public BreakoutGame(Screen screen) 
            : base(screen)
        {
            screen.Scale = SCALE;
            random = new Random();

            // create ball
            ball = (Ball)AddGameObject(new Ball(screen.WidthPixels / 2, 50, 0, 0));

            // create paddle
            float x = screen.WidthPixels / 2 - PADDLE_WIDTH;
            float y = screen.HeightPixels - TILE_SIZE * 2;
            paddle = AddGameObject(new GameObject(x, y, tileset.Texture, tileset.GetTile(32), 3));

            // create bricks
            bricks = new List<GameObject>(BRICK_COUNT);
            for (int i = 0; i < BRICK_COUNT; i++)
            {
                // calculate postion of the brick
                x = (i * TILE_SIZE) % screen.WidthPixels;
                y = (float)Math.Floor((float)i / TILE_SIZE) * TILE_SIZE;

                // randomise a tile
                int rand = random.Next(7);

                // span the "brick" tile
                int span = 1; 
                if (rand == BRICK_TILE)
                {
                    span = 2;
                    i++;
                }

                bricks.Add(
                    AddGameObject(new GameObject(x, y, tileset.Texture, tileset.GetTile(rand), span))
                );
            }

            StartGame();
        }

        public override void GameLoop()
        {
            Physics();
            Render();
            tick++;
        }

        public override void Physics()
        {
            // update paddle position
            paddle.X = screen.MouseX / SCALE - 24;

            // move ball by its X velocity, bouncing off vertical walls
            if (ball.X + ball.Velocity.X < 0 || ball.X + ball.Velocity.X + ball.Width > screen.WidthPixels) ball.Velocity.X *= -1;
            else ball.X += ball.Velocity.X;

            // move ball by its Y velocity, bouncing off horizontal walls
            if (ball.Y + ball.Velocity.Y < 0 || ball.Y + ball.Velocity.Y + ball.Height > screen.HeightPixels) ball.Velocity.Y *= -1;
            else ball.Y += ball.Velocity.Y;

            // bounce off bricks
            for (int i = 0; i < bricks.Count; i++)
            {
                GameObject brick = bricks[i];

                float x = ball.X;
                float y = ball.Y;

                // invert Y velocity if colluding on the vertical sides
                if (x + ball.Velocity.X < ball.X + brick.Width 
                    && x + ball.Velocity.X > brick.X
                    && y < brick.Y + brick.Height 
                    && y > brick.Y)
                {
                    ball.Velocity.X *= -1;

                    RemoveGameObject(bricks[i]);
                    bricks.RemoveAt(i);
                }
                // invert X velocity of colliding on the horizontal sides
                else if (x < brick.X + brick.Width 
                    && x > brick.X
                    && y + ball.Velocity.Y < brick.Y + brick.Height 
                    && y + ball.Velocity.Y > brick.Y)
                {
                    ball.Velocity.Y *= -1;

                    RemoveGameObject(bricks[i]);
                    bricks.RemoveAt(i);
                }
            }

            // bounce of paddle, applying angular velocity depending
            // on the collison point on the paddle
            if (ball.X < paddle.X + paddle.Width
                && ball.X + ball.Width > paddle.X 
                && ball.X + ball.Velocity.Y < paddle.Y + paddle.Height 
                && ball.Y + ball.Velocity.Y > paddle.Y)
            {
                paddle.OnCollsion(ball);
                float relX = (ball.X - paddle.X - paddle.Width / 2) / paddle.Width;
                ball.Velocity.X = relX * 5;
                ball.Velocity.Y *= -1;
            }
        }

        public override void Render()
            => base.Render();


        public override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }

        public override void StartGame()
        {
            ball.Velocity = new Utility.Vector2D(0, 5);
        }

        public override void EndGame()
        {
            // clean up..
        }
    }
}
