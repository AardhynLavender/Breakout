
//
//  BreakOutGame:Game class
//
//  Defines the functionality and members to create and play Atari Breakout
//  with score counters, powerups, levels, and saving.
//

using Breakout.GameObjects;
using Breakout.Render;
using Breakout.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;

namespace Breakout
{
    class BreakoutGame : Game
    {
        public const int TILE_SIZE          = 16;

        private const int LEVELS            = 3;
        private const int ROWS              = 6;
        private const int SCALE             = 3;
        private const int ANGLE_MULTIPLIER  = 5;

        private const int BALL_SPEED        = 5;
        private const int BALL_SIZE         = 6;

        private const int START_LIFES       = 3;
        private const int START_SCORE       = 0;
        private const int SCORE_LENGTH      = 6;

        private const int PADDLE_TILES      = 3;
        private const int PADDLE_WIDTH      = TILE_SIZE * PADDLE_TILES;
        private const int FONT_WIDTH        = 6;
        private const int FONT_HEIGHT       = 5;
        private const int HUD_MARGIN        = 10;

        // usefull tile coordiantes
        private const int PADDLE            = 36;
        private const int CLOSE             = 26;
        private const int HEART             = 27;
        private const int POINT_TILE        = 30;

        // Time
        private const int SECOND            = 1000;
        private const int HALF_SECOND       = 500;
        private const int TENTH_SECOND      = 100;
        private const int TWENTEITH_SECOND  = 50;

        private int score;
        private int lifes;

        private Level[] levels;
        private Level currentLevel;

        private List<Ball> balls;
        private Ball ball => balls.First();
        private GameObject backdrop;
        private GameObject paddle;

        private GameObject closeButton;
        private Text scoreLabel;
        private Text scoreDisplay;
        private Text livesLabel;
        private List<GameObject> lifeDisplay;

        private Random random;

        private List<Augment> augments;
        private Augment currentAugment;

        private Animation paddleAugmentEffect;
        private Animation[] heartbreak;

        public static readonly Tileset Tileset = 
            new Tileset(
                Properties.Resources.tileset, 
                Properties.Resources.tileset.Width, 
                TILE_SIZE, 
                TILE_SIZE
            );

        public static readonly Tileset Typeset =
            new Tileset(
                Properties.Resources.typeset,
                Properties.Resources.typeset.Width,
                FONT_WIDTH,
                FONT_HEIGHT
            );

        public static readonly Tileset Ballset =
            new Tileset(
                Properties.Resources.ball,
                Properties.Resources.ball.Width,
                BALL_SIZE,
                BALL_SIZE
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
                    QueueTask(SECOND, () => EndGame());
                }
            }
        }

        // public readonly members for Augments

        public int BallCount => balls.Count;
        public List<Ball> Balls => balls;
        public Ball Ball => balls.First();
        public Vector2D BallPosition => new Vector2D(ball.X, ball.Y);

        public GameObject Paddle => paddle;
        public Animation PaddleAugmentEffect => paddleAugmentEffect;

        // Constructor

        public BreakoutGame(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker) 
            : base(screen, media, ticker)
        {

            // initalize fields
            screen.Scale    = SCALE;
            score           = START_SCORE;
            lifes           = START_LIFES;
            random          = new Random();

            scoreLabel      = new Text(HUD_MARGIN, HUD_MARGIN, "SCORE");
            scoreDisplay    = new Text(HUD_MARGIN, HUD_MARGIN * 2);
            livesLabel      = new Text(0, HUD_MARGIN, "LIVES");
            lifeDisplay     = new List<GameObject>(START_LIFES);

            // initalize coordiantes
            float x, y;

            // add backdrop
            x = -TILE_SIZE;
            y = 0 - Properties.Resources.levelBackdrop.Height + Screen.HeightPixels;
            backdrop = AddGameObject(new GameObject(x, y, Properties.Resources.levelBackdrop, true));

            // create paddle
            x = Screen.WidthPixels / 2 - PADDLE_WIDTH / 2;
            y = Screen.HeightPixels - TILE_SIZE * 2;
            paddle = AddGameObject(new GameObject(x, y, Tileset.Texture, Tileset.GetTile(PADDLE), 3));

            // add paddle animation
            paddleAugmentEffect = AddAnimation(new Animation(
                this,
                paddle,
                new List<Rectangle>
                {
                    Tileset.GetTile(PADDLE + PADDLE_TILES, PADDLE_TILES),
                    Tileset.GetTile(PADDLE + PADDLE_TILES * 2, PADDLE_TILES)
                },
                Tileset,
                TENTH_SECOND,
                Tileset.GetTile(PADDLE, PADDLE_TILES),
                loopCap: 10
            ));

            // create ball
            balls = new List<Ball>(3);
            balls.Add((Ball)AddGameObject(new Ball(0, 0, 0, 0)));

            livesLabel.X = screen.WidthPixels / 2 - livesLabel.Width / 2;
            x = screen.WidthPixels / 2 - START_LIFES * TILE_SIZE / 2;

            // add lives display
            for (int i = 0; i < lifes; i++)
                lifeDisplay.Add(
                    AddGameObject(new GameObject(
                        x + TILE_SIZE * i,
                        TILE_SIZE + 1,
                        Tileset.Texture,
                        Tileset.GetTile(HEART),
                        ghost: true
                    ))
                );

            // add heartbreaking animation to hearts
            heartbreak = new Animation[START_LIFES];
            for (int i = 0; i < START_LIFES; i++)
                heartbreak[i] = AddAnimation(
                    new Animation(
                        this,
                        lifeDisplay[i],
                        new List<Rectangle>()
                        {
                            Tileset.GetTile(HEART + 1),
                            Tileset.GetTile(HEART + 2)
                        },
                        Tileset,
                        TWENTEITH_SECOND,
                        loop: false
                    )
                );

            // create close button
            closeButton = AddGameObject(new GameObject(0, 2, Tileset.Texture, Tileset.GetTile(CLOSE), ghost: true));
            closeButton.X = Screen.WidthPixels - closeButton.Width;

            // create augments
            augments = new List<Augment>();
            augments.Add(new GameObjects.Augments.TripleBallAugment(this));
            augments.Add(new GameObjects.Augments.TripleBallAugment(this));
            augments.Add(new GameObjects.Augments.TripleBallAugment(this));
            augments.Add(new GameObjects.Augments.TripleBallAugment(this));
            augments.Add(new GameObjects.Augments.TripleBallAugment(this));
            augments.Add(new GameObjects.Augments.TripleBallAugment(this));

            // create levels
            levels = new Level[LEVELS]
            {
                new Level(random, ROWS, Screen.WidthPixels, Tileset, 0, 8, augments),
                new Level(random, ROWS, Screen.WidthPixels, Tileset, 0, 8, augments),
                new Level(random, ROWS, Screen.WidthPixels, Tileset, 0, 8, augments),
            };

            currentLevel = levels[0];

            // initalize and build the first game level
            currentLevel.InitalizeLevel();

            // start game
            StartGame();
        }
        
        protected override void Process()
        {
            base.Process();

            // paralax effect on backdrop
            backdrop.X = -TILE_SIZE / 2 - TILE_SIZE * (paddle.X + PADDLE_WIDTH / 2) / Screen.WidthPixels - 0.5f;

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

            // process each ball
            foreach (Ball ball in balls)
            {
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

                    if (balls.Count <= 1)
                    {
                        Lives--;
                        if (Lives > 0) StartBall();
                    }
                    else
                    {
                        queueFree(ball);
                        QueueTask(0, () => balls.Remove(ball));
                    }
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
                    PlaySound(Properties.Resources.bounce);

                    // bounce ball
                    ball.Velocity.X = (ball.X - paddle.X - paddle.Width / 2) / paddle.Width * ANGLE_MULTIPLIER;
                    ball.Velocity.Y *= -1;
                }
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
            }

            // process physics for other game objects
            foreach (GameObject gameObject in gameObjects.Where(obj => !(obj is Ball)))
            {
                gameObject.X += gameObject.Velocity.X;
                gameObject.Y += gameObject.Velocity.Y;
            }

            // process current augment if not null
            if (!(currentAugment is null))
            {
                // free augments if they go off the screen
                if (currentAugment.Y > screen.HeightPixels)
                    ClearAugment();

                // hide augments that have been 'caught' off the screen
                else if (DoesCollide(paddle, currentAugment))
                    hideActiveAugment();
            }
        }

        private void hideActiveAugment()
        {
            currentAugment.Velocity.Zero();
            currentAugment.X = currentAugment.Y = -20;
        }

        public void ClearAugment()
        {
            queueFree(currentAugment);
            currentAugment = null;
        }

        protected override void Render()
            => base.Render();

        private void brickHit(int index)
        {
            Brick brick = currentLevel.Bricks[index];

            brick.Hits++;

            // increment and show gained points
            Score += brick.Value * brick.Hits;
            floatPoints(brick);

            if (brick.HasBeenDestroyed)
            {
                PlaySound(Properties.Resources._break);

                // does this brick drop an augment
                if (currentAugment is null && currentLevel.DropAugment(out Augment augment, brick))
                    currentAugment = (Augment)AddGameObject(augment);
                 
                // remove the brick
                queueFree(currentLevel.Bricks[index]);
                currentLevel.Bricks.RemoveAt(index);

                // explode brick
                foreach (GameObject fragment in brick.Debris)
                {
                    AddGameObject(fragment);
                    QueueTask(HALF_SECOND, () => queueFree(fragment));
                }
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
            if (Lives > -1) heartbreak[Lives].Animating = true;
        }

        public void StartBall()
        {
            // reset ball
            ball.X = Screen.WidthPixels / 2 - ball.Width / 2;

            // place ball as far up as possible (excluding level ceiling space)
            ball.Y = currentLevel.Ceiling + TILE_SIZE / 2;
            bool placedBall;
            do
            {
                placedBall = true;
                foreach (Brick brick in currentLevel.Bricks)
                    if (ball.Y < brick.Y + brick.Height)
                    {
                        ball.Y += TILE_SIZE;
                        placedBall = false;
                        break;
                    }
            }
            while (!placedBall);

            ball.Velocity.Zero();

            QueueTask(SECOND, () => ball.Velocity = new Utility.Vector2D(0, BALL_SPEED));
        }

        private void floatPoints(Brick brick)
        {
            // calculate point tile to show
            int tile = POINT_TILE + ((brick.Hits - 1) * 2);

            // create and setup floating point
            GameObject pointFloater = new GameObject(brick.X, brick.Y, Tileset.Texture, Tileset.GetTile(tile), ghost: false);
            pointFloater.Velocity = new Vector2D(0, -2);

            // show point floater for half a second
            AddGameObject(pointFloater);
            QueueTask(HALF_SECOND, () => queueFree(pointFloater));
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

            paddle.X = Screen.WidthPixels / 2 - PADDLE_WIDTH / 2;

            StartBall();
        }

        protected override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }
         
        protected override void EndGame()
        {
            // free all game objects
            foreach (GameObject gameObject in gameObjects) 
                queueFree(gameObject);

            processPhysics = false;
            QueueTask(TENTH_SECOND, () => freeQueue());

            // quit the application
            Quit();
        }
    }
}
