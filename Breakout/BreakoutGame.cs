
//
//  BreakOutGame Class
//
//  Defines the functionality and members to create and play Atari Breakout
//  with score counters, powerups, levels, and saving.
//  
//  BreakoutGame is responsible for managing the physics of its game objects
//  and the initalization and freeing of them.
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

        private Paddle paddle;
        private GameObject backdrop;
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

        public Level CurrentLevel               => currentLevel;
        public int BallCount                    => balls.Count;
        public List<Ball> Balls                 => balls;
        public Ball Ball                        => balls.First();
        public Vector2D BallPosition            => new Vector2D(ball.X, ball.Y);

        public Paddle Paddle                    => paddle;
        public Animation PaddleAugmentEffect    => paddleAugmentEffect;

        public int Scale => SCALE;

        // Constructor

        public BreakoutGame(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker) 
            : base(screen, media, ticker)
        {
            // proide game objects with reference to this class

            GameObject.BreakoutGame = this;
            GameObject.Screen = Screen;

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
            paddle = (Paddle)AddGameObject(new Paddle());

            // create ball
            balls = new List<Ball>(3);
            balls.Add((Ball)AddGameObject(new Ball()));

            // add lives display
            livesLabel.X = screen.WidthPixels / 2 - livesLabel.Width / 2;
            x = screen.WidthPixels / 2 - START_LIFES * TILE_SIZE / 2;

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

            // add heart break animation to hearts
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
            for (int i = 0; i < 5; i++)
                augments.Add(new GameObjects.Augments.TripleBallAugment());

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
            QueueFree(currentAugment);
            currentAugment = null;
        }

        protected override void Render()
            => base.Render();

        public void BrickHit(int index)
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
                QueueFree(currentLevel.Bricks[index]);
                currentLevel.Bricks.RemoveAt(index);

                // explode brick
                foreach (GameObject fragment in brick.Debris)
                {
                    AddGameObject(fragment);
                    QueueTask(HALF_SECOND, () => QueueFree(fragment));
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

            // animate point floater
            Animation animation = AddAnimation(new Animation(
                this,
                pointFloater,
                new List<Rectangle>
                {
                    Tileset.GetTile(tile),
                    Tileset.GetTile(tile + 1)
                },
                Tileset,
                50,
                Tileset.GetTile(tile),
                loop: true
            ));
            animation.Animating = true;

            // show point floater for half a second
            AddGameObject(pointFloater);
            QueueTask(HALF_SECOND, () => QueueFree(pointFloater));
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
                QueueFree(character);
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
                QueueFree(gameObject);

            processPhysics = false;
            QueueTask(TENTH_SECOND, () => freeQueue());

            // quit the application
            Quit();
        }
    }
}
