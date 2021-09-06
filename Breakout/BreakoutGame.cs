
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
using System.IO;
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

        private const int FONT_WIDTH        = 6;
        private const int FONT_HEIGHT       = 5;
        private const int HUD_MARGIN        = 10;

        // usefull tile coordiantes
        private const int CLOSE             = 26;
        private const int HEART             = 27;
        private const int POINT_TILE        = 30;

        private int score;
        private int lifes;

        private MainMenu menu;

        private Level[] levels;
        private Level currentLevel;

        private List<Ball> balls;
        private Ball ball => balls.First();

        private Paddle paddle;
        private GameObject backdrop;
        private GameObject closeButton;
        private Cursor cursor;

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

        // configuration fields

        private bool hasSfx;
        private bool hasCeiling;
        private bool hasLevels;
        private bool hasAugments;
        private bool hasInfiniteLives;
        private bool hasFloor;
        private bool hasPersistance;

        // properties

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

                if (lifes == 0)
                {
                    EndGame();
                    lifes = START_LIFES;
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

        // configuration properties

        public bool HasSfx { get => hasSfx; set => hasSfx = value; }
        public bool HasCeiling { get => hasCeiling; set => hasCeiling = value; }
        public bool HasLevels { get => hasLevels; set => hasLevels = value; }
        public bool HasAugments { get => hasAugments; set => hasAugments = value; }
        public bool HasInfiniteLives { get => hasInfiniteLives; set => hasInfiniteLives = value; }
        public bool HasFloor { get => hasFloor; set => hasFloor = value; }
        public bool HasPersistance { get => hasPersistance; set => hasPersistance = value; }
        
        // Constructor

        public BreakoutGame(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker) 
            : base(screen, media, ticker)
        {
            // proide game componants with reference to *this* and Screen
            GameComponant.BreakoutGame = this;
            GameComponant.Screen = Screen;

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
            backdrop = new GameObject(x, y, Properties.Resources.levelBackdrop, true);

            // create paddle
            paddle = new Paddle();

            // create ball
            balls = new List<Ball>();
            balls.Add(new Ball());

            // add lives display
            livesLabel.X = screen.WidthPixels / 2 - livesLabel.Width / 2;
            x = screen.WidthPixels / 2 - START_LIFES * TILE_SIZE / 2;

            for (int i = 0; i < lifes; i++)
                lifeDisplay.Add(
                    new GameObject(
                        x + TILE_SIZE * i,
                        TILE_SIZE + 1,
                        Tileset.Texture,
                        Tileset.GetTile(HEART),
                        ghost: true
                    )
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
                        Time.TWENTYTH_SECOND,
                        loop: false
                    )
                );

            // create augments
            augments = new List<Augment>();

            for (int _ = 0; _ < 5; _++)
                augments.Add(new GameObjects.Augments.TripleBallAugment());

            for (int _ = 0; _ < 5; _++)
                augments.Add(new GameObjects.Augments.ExplodingBallAugment());

            // create levels
            levels = new Level[LEVELS]
            {
                new Level(random, ROWS, Screen.WidthPixels, Tileset, 0, 8, augments),
                new Level(random, ROWS, Screen.WidthPixels, Tileset, 0, 8, augments),
                new Level(random, ROWS, Screen.WidthPixels, Tileset, 0, 8, augments),
            };

            currentLevel = levels[0];

            // open main menu
            menu = (MainMenu)AddGameObject(new MainMenu());
            menu.Open();

            // create close button
            closeButton = AddGameObject(new GameObject(0, 2, Tileset.Texture, Tileset.GetTile(CLOSE), ghost: true));
            closeButton.X = Screen.WidthPixels - closeButton.Width;

            cursor = (Cursor)AddGameObject(new Cursor());
        }
        
        protected override void Process()
        {
            base.Process();

            // paralax effect on backdrop
            backdrop.X = -TILE_SIZE / 2 - TILE_SIZE * (paddle.X + Paddle.Width / 2) / Screen.WidthPixels - 0.5f;

            // check if player pressed the close button
            if (Screen.MouseX / SCALE > closeButton.X
                && Screen.MouseX / SCALE < closeButton.X + closeButton.Width
                && Screen.MouseY / SCALE> closeButton.Y
                && Screen.MouseY / SCALE < closeButton.Y + closeButton.Height
                && Screen.MouseDown
                )
            {
                if (HasPersistance) SaveGame();
                Quit();
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
                if (currentAugment is null && HasAugments && currentLevel.DropAugment(out Augment augment, brick))
                    currentAugment = (Augment)AddGameObject(augment);

                ExplodeBrick(brick);
            }
            else PlaySound(Properties.Resources.bounce);
        }

        public void ExplodeBrick(Brick brick)
        {
            // remove the brick
            QueueFree(currentLevel.Bricks[currentLevel.Bricks.IndexOf(brick)]);
            currentLevel.Bricks.Remove(brick);

            // explode brick
            foreach (GameObject fragment in brick.Debris)
            {
                AddGameObject(fragment);
                QueueTask(Time.HALF_SECOND, () => QueueFree(fragment));
            }
        }

        private void buildLevel()
        {
            // initalize bricks.
            currentLevel.InitalizeLevel();

            // add bricks.
            foreach (Brick brick in currentLevel.Bricks)
                AddGameObject(brick);

            Console.WriteLine(CurrentLevel.RowSize);
        }

        private void updateScore()
            => scoreDisplay.Value = Score.ToString($"D{SCORE_LENGTH}");

        private void updateLives()
        {
            if (lifes > -1) heartbreak[lifes].Animating = true;
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

            QueueTask(Time.SECOND, () => ball.Velocity = new Utility.Vector2D(0, BALL_SPEED));
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
                Time.TENTH_SECOND,
                loop: true
            ));
            animation.Animating = true;

            // show point floater for half a second
            AddGameObject(pointFloater);
            QueueTask(Time.HALF_SECOND, () => QueueFree(pointFloater));
        }

        public override void StartGame()
        {

            if (!HasInfiniteLives)
            {
                AddGameObject(livesLabel);
                foreach (GameObject life in lifeDisplay)
                    AddGameObject(life);
            }

            AddGameObject(backdrop);
            AddGameObject(paddle);
            AddGameObject(ball);

            buildLevel();

            AddGameObject(scoreLabel);
            updateScore();

            StartBall();
        }

        protected override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }

        public override void PlaySound(Stream sound)
        {
            if (HasSfx) base.PlaySound(sound);
        }

        public override void EndGame()
        {
            // free groups of objects
            balls.ForEach(b => QueueFree(b));
            lifeDisplay.ForEach(l => QueueFree(l));
            currentLevel.Bricks.ForEach(b => QueueFree(b));

            // reset lives
            lifes = START_LIFES;
            foreach (Animation heartbreak in heartbreak)
                heartbreak.Reset();

            // reset score
            score = 0;

            // free game objects
            QueueFree(livesLabel);
            QueueFree(scoreDisplay);
            QueueFree(scoreLabel);
            QueueFree(Paddle);
            QueueFree(backdrop);

            // return to menu
            AddGameObject(menu);
            menu.Open();
        }
    }
}
