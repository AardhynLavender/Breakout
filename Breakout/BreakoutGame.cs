
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
        private const int START_SCORE   = 0;
        private const int PADDLE_WIDTH  = TILE_SIZE * 3;
        private const int SCORE_LENGTH  = 6;

        // usefull tile coordiantes
        private const int PADDLE = 36;
        private const int CLOSE = 24;

        private int score;
        private int lifes;

        private Level[] levels;
        private Level currentLevel;

        private Ball ball;
        private GameObject backdrop;
        private GameObject paddle;

        private GameObject closeButton;
        private Text scoreLabel;
        private Text scoreDisplay;
        private Text livesLabel;
        private List<GameObject> lifeDisplay;

        private Random random;

        public static readonly Tileset tileset = 
            new Tileset(
                Properties.Resources.tileset, 
                Properties.Resources.tileset.Width, 
                TILE_SIZE, 
                TILE_SIZE
            );

        public static readonly Tileset typeset =
            new Tileset(
                Properties.Resources.typeset,
                Properties.Resources.typeset.Width,
                6,
                5
            );

        public int Score 
        { 
            get => score;
            set
            {
                score = value;
                updateScore();
            }
        }

        public int Lives
        {
            get => lifes;
            set
            {
                lifes = value;
                updateLives();

                if (lifes <= 0)
                {
                    // tell the user they have lost
                    Sleep(1000, () => Quit());
                }
            }
        }

        private Animator paddleAnimation;

        public BreakoutGame(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker) 
            : base(screen, media, ticker)
        {

            // initalize fields
            screen.Scale    = SCALE;
            score           = START_SCORE;
            lifes           = START_LIFES;
            random          = new Random();

            scoreLabel      = new Text(10, 10, "SCORE");
            scoreDisplay    = new Text(10, 20);
            livesLabel      = new Text(0, 10, "LIVES");
            lifeDisplay     = new List<GameObject>(START_LIFES);

            // create levels
            levels = new Level[LEVELS]
            {
                new Level(random, ROWS, Screen.WidthPixels, tileset, 0, 8),
                new Level(random, ROWS, Screen.WidthPixels, tileset, 0, 8),
                new Level(random, ROWS, Screen.WidthPixels, tileset, 0, 8),
            };

            currentLevel = levels[0];

            // initalize coordiantes
            float x, y;

            // add backdrop
            x = 0;
            y = 0 - Properties.Resources.levelBackdrop.Height + Screen.HeightPixels;
            backdrop = AddGameObject(new GameObject(x, y, Properties.Resources.levelBackdrop, true));

            // create paddle
            x = Screen.WidthPixels / 2 - PADDLE_WIDTH / 2;
            y = Screen.HeightPixels - TILE_SIZE * 2;
            paddle = AddGameObject(new GameObject(x, y, tileset.Texture, tileset.GetTile(PADDLE), 3));

            // create ball
            ball = (Ball)AddGameObject(new Ball(0, 0, 0, 0));

            // initalize and build the first game level
            currentLevel.InitalizeLevel();

            livesLabel.X = screen.WidthPixels / 2 - livesLabel.Width / 2;

            // create close button
            closeButton = AddGameObject(new GameObject(0, 2, tileset.Texture, tileset.GetTile(CLOSE), ghost: true));
            closeButton.X = Screen.WidthPixels - closeButton.Width;

            // create animators
            paddleAnimation = new Animator(
                this,
                paddle,
                new List<Rectangle>
                {
                    tileset.GetTile(PADDLE + 3, 3, 1),
                    tileset.GetTile(PADDLE + 6, 3, 1)
                },
                tileset,
                100,
                loopCap: 10
            );

            lifeDisplay.ForEach(life =>
            {
                // create new animators
            });

            StartGame();
        }

        public override void GameLoop()
        {
            if (processPhysics) Physics();
            else SleepTicks--;

            Render();
            tick++;
        }
        
        protected override void Physics()
        {
            base.Physics();

            // update paddle position
            if (Screen.MouseX / SCALE - PADDLE_WIDTH / 2 < 0)
            {
                paddle.X = 0;
            }
            else if (Screen.MouseX / SCALE + PADDLE_WIDTH / 2 > Screen.WidthPixels)
            {
                paddle.X = Screen.WidthPixels - PADDLE_WIDTH;
            }
            else paddle.X = Screen.MouseX / SCALE - 24;

            // move ball by its X velocity, bouncing off vertical walls
            if (ball.X + ball.Velocity.X < 0 || ball.X + ball.Velocity.X + ball.Width > Screen.WidthPixels)
            {
                ball.Velocity.X *= -1;
                PlaySound(Properties.Resources.bounce);
            }
            else ball.X += ball.Velocity.X;

            // move ball by its Y velocity, bouncing off top wall
            if (ball.Y + ball.Velocity.Y < 0)
            {
                ball.Velocity.Y *= -1;
                PlaySound(Properties.Resources.bounce);
            }
            else if (ball.Y + ball.Velocity.Y > Screen.HeightPixels + 10)
            {
                // ball has fallen off the screen
                PlaySound(Properties.Resources._break);
                Lives--;
                StartBall();
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
                    brickHit(i);
                }
                // invert X velocity of colliding on the horizontal sides
                else if (x < brick.X + brick.Width
                    && x > brick.X
                    && y + ball.Velocity.Y < brick.Y + brick.Height
                    && y + ball.Velocity.Y > brick.Y)
                {
                    ball.Velocity.Y *= -1;
                    brickHit(i);
                }
            }

            // bounce of paddle, applying angular velocity depending
            // on the collison point on the paddle
            if (ball.X <= paddle.X + paddle.Width
                && ball.X + ball.Width > paddle.X 
                && ball.Y + ball.Velocity.Y <= paddle.Y + paddle.Height 
                && ball.Y + ball.Height + ball.Velocity.Y > paddle.Y)
            {
                paddle.OnCollsion(ball);
                PlaySound(Properties.Resources.bounce);

                // bounce ball
                ball.Velocity.X = ((ball.X - paddle.X - paddle.Width / 2) / paddle.Width) * 5;
                ball.Velocity.Y *= -1;
            }

            // check if player pressed the close button
            if (Screen.MouseX / SCALE > closeButton.X
                && Screen.MouseX / SCALE < closeButton.X + closeButton.Width
                && Screen.MouseY / SCALE> closeButton.Y
                && Screen.MouseY / SCALE < closeButton.Y + closeButton.Height
                && Screen.MouseDown
                )
            {
                EndGame();
                Quit();
            }

            // process physics for other game objects
            foreach (GameObject gameObject in gameObjects.Where(obj => !(obj is Ball)))
            {
                gameObject.X += gameObject.Velocity.X;
                gameObject.Y += gameObject.Velocity.Y;
            }

            paddleAnimation.Update();
        }

        protected override void Render()
            => base.Render();

        private void brickHit(int index)
        {
            Brick brick = currentLevel.Bricks[index];

            brick.Hits++;
            if (brick.HasBeenDestroyed)
            {
                PlaySound(Properties.Resources._break);
                Score += brick.Value;

                // remove the brick
                queueFree(currentLevel.Bricks[index]);
                currentLevel.Bricks.RemoveAt(index);

                // explode brick
                foreach (GameObject gameObject in brick.Debris)
                    AddGameObject(gameObject);
            }
            else PlaySound(Properties.Resources.bounce);
        }

        private void buildLevel()
        {
            // add bricks
            foreach (Brick brick in currentLevel.Bricks)
                AddGameObject(brick);
        }

        private void updateScore()
        {
            // remove previous score
            freeText(scoreDisplay);
            scoreDisplay.Clear();

            // replace score display with updated score
            scoreDisplay.Value = Score.ToString($"D{SCORE_LENGTH}");
            addText(scoreDisplay);
        }

        private void updateLives()
        {
            // remove previous lives
            foreach (GameObject life in lifeDisplay)
                queueFree(life);

            lifeDisplay.Clear();

            float x = screen.WidthPixels / 2 - START_LIFES * TILE_SIZE / 2;

            // add new lives
            for (int i = 0; i < lifes; i++)
                lifeDisplay.Add(
                    AddGameObject(new GameObject(
                        x + TILE_SIZE * i, 
                        TILE_SIZE + 1, 
                        tileset.Texture, 
                        tileset.GetTile(27), 
                        ghost: true
                    ))
                );
        }

        private void StartBall()
        {
            // rest paddle
            paddle.X        = Screen.WidthPixels / 2 - PADDLE_WIDTH / 2;

            // rest ball
            ball.X          = Screen.WidthPixels / 2 - ball.Width / 2;
            ball.Y          = 100;
            ball.Velocity   = new Utility.Vector2D(0, 5);

            // give the user a break
            Sleep(1000);
        }

        private void addText(Text text)
        {
            text.Update();
            foreach (GameObject character in text.Characters)
                AddGameObject(character);
        }

        private void freeText(Text text)
        {
            foreach (GameObject character in text.Characters)
                queueFree(character);
        }

        protected override void StartGame()
        {
            buildLevel();
            addText(scoreLabel);
            updateScore();
            addText(livesLabel);
            updateLives();
            StartBall();
        }

        protected override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }
         
        protected override void EndGame()
        {
            foreach (GameObject gameObject in gameObjects)
                queueFree(gameObject);
        }
    }
}
