
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
using System.Media;

namespace Breakout
{
    class BreakoutGame : Game
    {
        private const int SCALE             = 3;
        private const int TILE_SIZE         = 16;
        private const int START_LIFES       = 3;
        private const int BRICK_COUNT       = 40;
        private const int PADDLE_WIDTH      = TILE_SIZE * 3;
        private const int BRICK_TILE        = 6;

        private Dictionary<char, int>
        characterMap = new Dictionary<char, int>
        {
            {'0', 8 },
            {'1', 9 },
            {'2', 10 },
            {'3', 11 },
            {'4', 12 },
            {'5', 13 },
            {'6', 14 },
            {'7', 15 },
            {'8', 16 },
            {'9', 17 },
            {'l', 18 },
            {'e', 19 },
            {'v', 20 },
            {'s', 21 },
            {'c', 22 },
            {'o', 23 },
            {'r', 24 },
        }; 

        private int score;
        private int lifes;

        private Ball ball;
        private GameObject paddle;
        private List<Brick> bricks;

        private Random random;

        public static readonly Tileset tileset = 
            new Tileset(
                Properties.Resources.tileset, 
                Properties.Resources.tileset.Width, 
                TILE_SIZE, 
                TILE_SIZE
            );

        public BreakoutGame(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker) 
            : base(screen, media, ticker)
        {
            screen.Scale = SCALE;

            // initalize fields
            score = 0;
            lifes = START_LIFES;
            random = new Random();

            // create ball
            ball = (Ball)AddGameObject(new Ball(screen.WidthPixels / 2, 50, 0, 0));

            // create paddle
            float x = screen.WidthPixels / 2 - PADDLE_WIDTH;
            float y = screen.HeightPixels - TILE_SIZE * 2;
            paddle  = AddGameObject(new GameObject(x, y, tileset.Texture, tileset.GetTile(32), 3));

            // create bricks
            bricks = new List<Brick>(BRICK_COUNT);
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
                    (Brick)AddGameObject(new Brick(x, y, tileset.Texture, tileset.GetTile(rand), span, 12, 1))
                );
            }

            this.ticker.Start();
            StartGame();
        }

        public override void GameLoop()
        {
            Physics();
            Render();
            tick++;
        }

        protected override void Physics()
        {
            // update paddle position
            paddle.X = Screen.MouseX / SCALE - 24;

            // move ball by its X velocity, bouncing off vertical walls
            if (ball.X + ball.Velocity.X < 0 || ball.X + ball.Velocity.X + ball.Width > Screen.WidthPixels)
            {
                ball.Velocity.X *= -1;
                PlaySound(Properties.Resources.bounce);
            }
            else ball.X += ball.Velocity.X;

            // move ball by its Y velocity, bouncing off horizontal walls
            if (ball.Y + ball.Velocity.Y < 0 || ball.Y + ball.Velocity.Y + ball.Height > Screen.HeightPixels)
            {
                ball.Velocity.Y *= -1;
                PlaySound(Properties.Resources.bounce);
            }
            else ball.Y += ball.Velocity.Y;

            // bounce off bricks
            for (int i = 0; i < bricks.Count; i++)
            {
                Brick brick = bricks[i];

                float x = ball.X;
                float y = ball.Y;

                // invert Y velocity if colluding on the vertical sides
                if (x + ball.Velocity.X < brick.X + brick.Width
                    && x + ball.Velocity.X > brick.X
                    && y < brick.Y + brick.Height
                    && y > brick.Y)
                {
                    ball.Velocity.X *= -1;
                    BrickHit(i);
                }
                // invert X velocity of colliding on the horizontal sides
                else if (x < brick.X + brick.Width
                    && x > brick.X
                    && y + ball.Velocity.Y < brick.Y + brick.Height
                    && y + ball.Velocity.Y > brick.Y)
                {
                    ball.Velocity.Y *= -1;
                    BrickHit(i);
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
                PlaySound(Properties.Resources.bounce);
                float relX = (ball.X - paddle.X - paddle.Width / 2) / paddle.Width;
                ball.Velocity.X = relX * 5;
                ball.Velocity.Y *= -1;
            }
        }


        protected override void Render()
            => base.Render();


        private void BrickHit(int index)
        {
            Brick brick = bricks[index];

            brick.Hits++;
            if (brick.HasBeenDestroyed)
            {
                PlaySound(Properties.Resources._break);
                score += brick.Value;

                RemoveGameObject(bricks[index]);
                bricks.RemoveAt(index);
            }
            else PlaySound(Properties.Resources.bounce);
        }

        private GameObject[] DisplayText(string text, int x, int y)
        {
            int length = text.Length;
            GameObject[] objects = new GameObject[length];

            for (int i = 0; i < length; i++)
            {
                int translation = i * TILE_SIZE;
                int tileIndex = characterMap[text[i]];
                objects[i] = AddGameObject(new GameObject(translation, y, tileset.Texture, tileset.GetTile(tileIndex), ghost: true));
            }

            return objects;
        }

        protected override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }

        protected override void StartGame()
        {
            ball.Velocity = new Utility.Vector2D(0, 5);
            DisplayText("score", 0,0);
        }

        protected override void EndGame()
        {
            foreach (GameObject gameObject in gameObjects)
                RemoveGameObject(gameObject);
        }
    }
}
