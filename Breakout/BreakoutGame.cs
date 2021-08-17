
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
using System.Drawing;
using System.Media;

using Breakout.Render;
using Breakout.Utility;
using Breakout.GameObjects;

namespace Breakout
{
    class BreakoutGame : Game
    {
        public const int TILE_SIZE      = 16;

        private const int LEVELS        = 3;
        private const int ROWS          = 3;
        private const int SCALE         = 3;
        private const int START_LIFES   = 3;
        private const int BRICK_COUNT   = 40;
        private const int PADDLE_WIDTH  = TILE_SIZE * 3;
        private const int BRICK_TILE    = 7;

        private int score;
        private int lifes;

        private Level[] levels;
        private Level currentLevel;

        private Ball ball;
        private GameObject backdrop;
        private GameObject paddle;

        private Random random;

        public static readonly Tileset tileset = 
            new Tileset(
                Properties.Resources.tileset, 
                Properties.Resources.tileset.Width, 
                TILE_SIZE, 
                TILE_SIZE
            );

        public int Score 
        { 
            get => score; 
            set => score = value; 
        }

        public BreakoutGame(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker) 
            : base(screen, media, ticker)
        {

            // initalize fields

            screen.Scale = SCALE;

            score = 0;
            lifes = START_LIFES;
            random = new Random();

            // create levels
            levels = new Level[LEVELS]
            {
                new Level(random, ROWS, Screen.WidthPixels, tileset, 0, 8),
                new Level(random, ROWS, Screen.WidthPixels, tileset, 0, 8),
                new Level(random, ROWS, Screen.WidthPixels, tileset, 0, 8),
            };

            currentLevel = levels[0];
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
            for (int i = 0; i < currentLevel.Bricks.Count; i++)
            {
                Brick brick = currentLevel.Bricks[i];

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

            // process physics for other game objects
            List<GameObject> deleteQueue = new List<GameObject>();
            foreach (GameObject gameObject in gameObjects.Where(obj => !(obj is Ball)))
            {
                gameObject.X += gameObject.Velocity.X;
                gameObject.Y += gameObject.Velocity.Y;

                // delete debris that are not visable
                if (!ObjectVisable(gameObject))
                    deleteQueue.Add(gameObject);
            }

            // delete any objects in the queue
            deleteQueue.ForEach(gameObject => RemoveGameObject(gameObject));
            deleteQueue.Clear();
        }

        protected override void Render()
            => base.Render();

        public void BrickHit(int index)
        {
            Brick brick = currentLevel.Bricks[index];

            brick.Hits++;
            if (brick.HasBeenDestroyed)
            {
                PlaySound(Properties.Resources._break);
                Score += brick.Value;

                RemoveGameObject(currentLevel.Bricks[index]);
                currentLevel.Bricks.RemoveAt(index);

                // explode brick

                foreach (GameObject gameObject in brick.Debris)
                    AddGameObject(gameObject);
            }
            else PlaySound(Properties.Resources.bounce);
        }

        private void BuildLevel()
        {
            foreach (Brick brick in currentLevel.Bricks)
                AddGameObject(brick);
        }

        protected override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }

        protected override void StartGame()
        {
            // add backdrop
            backdrop = AddGameObject(new GameObject(0, 0, Properties.Resources.levelBackdrop, true));

            // create paddle
            float x = Screen.WidthPixels / 2 - PADDLE_WIDTH;
            float y = Screen.HeightPixels - TILE_SIZE * 2;
            paddle = AddGameObject(new GameObject(x, y, tileset.Texture, tileset.GetTile(36), 3));

            // create ball
            ball = (Ball)AddGameObject(new Ball(Screen.WidthPixels / 2, 50, 0, 0));

            // initalize and build the first game level
            currentLevel.InitalizeLevel();
            BuildLevel();

            ball.Velocity = new Utility.Vector2D(0, 5);
        }

        protected override void EndGame()
        {
            foreach (GameObject gameObject in gameObjects)
                RemoveGameObject(gameObject);
        }
    }
}
