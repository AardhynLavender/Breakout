
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
using Breakout.Utility.levels;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;

namespace Breakout
{
    class BreakoutGame : Game
    {
        // constants
        public const int TILE_SIZE          = 16;

        private const int LEVELS            = 3;
        private const int ROWS              = 6;

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

        // fields

        private int score;
        private int hiScore;
        private int lifes;
        private bool levelRunning;

        private MainMenu menu;

        private Level[] levels;
        private int currentLevel;

        private List<Ball> balls;
        private Ball ball => balls.First();

        private Paddle paddle;
        private GameObject backdrop;
        private GameObject closeButton;
        private Cursor cursor;

        private Text timeLabel;
        private Text gameTime;
        private Stopwatch gameStopwatch;

        private Text scoreLabel;
        private Text scoreDisplay;

        private Text hiScoreLabel;
        private Text hiScoreDisplay;

        private Text levelBanner;
        private Text levelScoreDisplay;
        private Text timeBonusDisplay;
        private Text livesBonusDisplay;

        private Text looseBanner;

        private Text livesLabel;
        private List<GameObject> lifeDisplay;

        private Augment currentAugment;

        private Animation[] heartbreak;

        // static fields

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

                // update hiscore if needed
                if (score > hiScore)
                    HiScore = score;

                updateScore();
            }
        }

        public int HiScore
        {
            get => hiScore;
            set
            {
                hiScore = value;

                updateHiScore();
            }
        }

        public int Lives
        {
            get => lifes;
            set
            {
                lifes = value;
                updateLives();
                
                // end game if out of lives
                if (lifes == 0)
                {
                    EndGame();
                    lifes = START_LIFES;
                }
            }
        }

        public Level CurrentLevel
        {
            get => levels[currentLevel];
        }

        public bool LevelRunning        
        { 
            get => levelRunning;
            set => levelRunning = value; 
        }

        public int BallCount                    
            => balls.Count;

        public List<Ball> Balls                 
            => balls;

        public Ball Ball                        
            => balls.First();

        public Vector2D BallPosition            
            => new Vector2D(ball.X, ball.Y);

        public Paddle Paddle                    
            => paddle;

        public int Scale 
            => screen.Scale;

        // configuration properties

        public bool HasSfx              { get => hasSfx; set => hasSfx = value; }
        public bool HasCeiling          { get => hasCeiling; set => hasCeiling = value; }
        public bool HasLevels           { get => hasLevels; set => hasLevels = value; }
        public bool HasAugments         { get => hasAugments; set => hasAugments = value; }
        public bool HasInfiniteLives    { get => hasInfiniteLives; set => hasInfiniteLives = value; }
        public bool HasFloor            { get => hasFloor; set => hasFloor = value; }
        public bool HasPersistance      { get => hasPersistance; set => hasPersistance = value; }

        // Constructor

        public BreakoutGame(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker) 
            : base(screen, media, ticker)
        {
            // initalize fields

            score           = START_SCORE;
            lifes           = START_LIFES;

            // provide game components with reference to *this* class and the Screen

            GameComponant.BreakoutGame = this;
            GameComponant.Screen = Screen;
            GameComponant.Random = random;

            // initalize coordiante variables

            float x, y;

            // add backdrop

            x = -TILE_SIZE;
            backdrop = new GameObject(x, 0, Properties.Resources.levelBackdrop, true);

            // create paddle

            paddle = new Paddle();

            // create ball

            balls = new List<Ball>();
            balls.Add(new Ball());

            // score display

            scoreLabel      = new Text(HUD_MARGIN, HUD_MARGIN, "score");
            scoreDisplay    = new Text(HUD_MARGIN, HUD_MARGIN * 2);

            // hi score display

            hiScoreLabel = new Text(HUD_MARGIN * 5.2f, HUD_MARGIN, "hi");
            hiScoreDisplay = new Text(HUD_MARGIN * 5.2f, HUD_MARGIN * 2);

            // add loose banner

            y = Screen.HeightPixels / 3;
            looseBanner = new Text(HUD_MARGIN, y, "you lost");

            // add level banner

            levelBanner = new Text(HUD_MARGIN, y, "");
            timeBonusDisplay = new Text(HUD_MARGIN, y += HUD_MARGIN, "");
            livesBonusDisplay = new Text(HUD_MARGIN, y += HUD_MARGIN, "");
            levelScoreDisplay = new Text(HUD_MARGIN, y += HUD_MARGIN, "");

            // stopwatch display

            gameStopwatch = new Stopwatch();

            x = screen.WidthPixels - HUD_MARGIN * 2;

            timeLabel = new Text(x, HUD_MARGIN, "time"); ;
            gameTime = new Text(0, HUD_MARGIN * 2);

            timeLabel.X -= timeLabel.Width;
            gameTime.X = screen.WidthPixels - HUD_MARGIN * 5;

            // lives display

            lifeDisplay = new List<GameObject>(START_LIFES);

            livesLabel = new Text(0, HUD_MARGIN, "LIVES");
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

            // open main menu

            menu = (MainMenu)AddGameObject(new MainMenu());
            menu.Open();

            // create close button

            closeButton = AddGameObject(new GameObject(0, 2, Tileset.Texture, Tileset.GetTile(CLOSE), ghost: true));
            closeButton.X = Screen.WidthPixels - closeButton.Width;

            // create levels

            levels = new Level[LEVELS]
            {
                new Level(ROWS, Screen.WidthPixels, Tileset, 0, 9),
                new SecondLevel(ROWS, Screen.WidthPixels, Tileset, 0, 9),
                new ThirdLevel(ROWS, Screen.WidthPixels, Tileset, 0, 9),
            };

            // create cursor

            cursor = (Cursor)AddGameObject(new Cursor());
        }
        
        // process the game per loop
        protected override void Process()
        {
            base.Process();

            // paralax effect on backdrop
            float offset = -TILE_SIZE / 2 - TILE_SIZE;
            float paddle_Center = (paddle.X + Paddle.Width / 2);

            backdrop.X = offset *  paddle_Center / Screen.WidthPixels;

            // check if player pressed the close button
            if (Screen.MouseX / Scale > closeButton.X
                && Screen.MouseX / Scale < closeButton.X + closeButton.Width
                && Screen.MouseY / Scale > closeButton.Y
                && Screen.MouseY / Scale < closeButton.Y + closeButton.Height
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
                if (currentAugment.Y > screen.HeightPixels || !levelRunning)
                    ClearAugment();

                // hide augments that have been 'caught'
                else if (DoesCollide(paddle, currentAugment))
                    hideActiveAugment();
            }

            // level processing
            if (levelRunning)
            {
                // check if level has been cleared
                if (CurrentLevel.BrickCount <= 0)
                    NextLevel();

                // update the game time
                updateTime();
            }
        }

        // hides the active augment off the screen while its applied
        private void hideActiveAugment()
        {
            currentAugment.Velocity.Zero();
            currentAugment.X = currentAugment.Y = -20;
        }

        // removes the current augment
        public void ClearAugment()
        {
            QueueFree(currentAugment);
            currentAugment = null;
        }

        // manager method for when a brick is hit
        public void BrickHit(int index)
        {
            // get brick
            Brick brick = CurrentLevel.Bricks[index];

            // updates its hits
            brick.Hits++;

            // increment and show gained points
            Score += brick.Value * brick.Hits;
            floatPoints(brick);

            // destroy the brick if
            if (brick.HasBeenDestroyed)
            {
                // does this brick drop an augment
                if (currentAugment is null && HasAugments && CurrentLevel.DropAugment(out Augment augment, brick))
                    currentAugment = (Augment)AddGameObject(augment);

                brick.Explode();
            }
            else PlaySound(Properties.Resources.bounce);
        }

        // goes to the next level
        private void NextLevel()
        {
            levelRunning = false;

            if (currentLevel + 1 < levels.Length && hasLevels)
            {
                // reject any active augments
                if (!(currentAugment is null)) currentAugment.Reject();

                // free current level
                CurrentLevel.Free();

                // transition backdrop
                backdrop.Velocity.Y = 1;

                // hide ball
                ball.Velocity.Zero();
                ball.X = ball.Y = -10;

                // show level stats
                showLevelStats();

                QueueTask(Time.SECOND * 4, () =>
                {
                    backdrop.Velocity.Zero();

                    // reset timer
                    gameStopwatch.Reset();

                    // build the next level
                    currentLevel++;
                    levelRunning = true;
                    CurrentLevel.Build();

                    StartBall();
                });
            }
            else
            {
                // end the game
                EndGame();
            }
        }

        // show the statistics for the completed level
        private void showLevelStats()
        {
            // initalize variables
            int passedMinutes   = (int)Math.Floor(gameStopwatch.Elapsed.TotalMinutes);
            int timeBonus       = 0;
            int livesBonus      = 0;

            // compute and add time bonus

            if (passedMinutes < 3)
                timeBonus = Score * 2;

            else if (passedMinutes == 3)
                timeBonus = (int)(score * 1.66f);

            else if (passedMinutes == 4)
                 timeBonus = (int)(score * 1.33f);

            Score += timeBonus;

            // compute lives bonus

            if (!HasInfiniteLives)
            {
                if (Lives == 3)
                    livesBonus = 600;

                else if (Lives == 2)
                    livesBonus = 300;
            }

            // set text values
            levelBanner.Value   = (currentLevel == 2) ? "you won!" : $"Completed level    {currentLevel + 1}";
            timeBonusDisplay.Value  = $"bonus points       {timeBonus.ToString($"D{SCORE_LENGTH}")}";
            livesBonusDisplay.Value = $"lives points       {livesBonus.ToString($"D{SCORE_LENGTH}")}";
            levelScoreDisplay.Value = $"final level score  {scoreDisplay.Value}";

            // add objects
            AddGameObject(levelBanner);
            AddGameObject(timeBonusDisplay);
            AddGameObject(livesBonusDisplay);
            AddGameObject(levelScoreDisplay);

            updateScore();

            // remove the objects after 4 seconds
            QueueTask(Time.SECOND * 4, () =>
            {
                QueueFree(levelBanner);
                QueueFree(timeBonusDisplay);
                QueueFree(livesBonusDisplay);
                QueueFree(levelScoreDisplay);
            });
        }

        // shows the player they have lost
        private void ShowLooseScreen()
        {
            // set text and pos
            levelScoreDisplay.Value = $"final score  {scoreDisplay.Value}";
            levelScoreDisplay.Y = looseBanner.Y + HUD_MARGIN;

            // add objects
            AddGameObject(looseBanner);
            AddGameObject(levelScoreDisplay);

            // remove objects
            QueueTask(Time.SECOND * 4, () =>
            {
                QueueFree(looseBanner);
                QueueFree(levelScoreDisplay);
            });
        }

        // update the score display with the score
        private void updateScore()
            => scoreDisplay.Value = Score.ToString($"D{SCORE_LENGTH}");

        // update the hi score display with the score
        private void updateHiScore()
            => hiScoreDisplay.Value = hiScore.ToString($"D{SCORE_LENGTH}");

        // update the lives display with the score
        private void updateLives()
            => heartbreak[lifes].Animating = lifes > -1;

        // update the time display with the score
        private void updateTime()
            => gameTime.Value = $"{gameStopwatch.Elapsed.Minutes:D2} {gameStopwatch.Elapsed.Seconds:D2}";

        // reset the ball to the start position
        public void StartBall()
        {
            // reset ball
            ball.X = Screen.WidthPixels / 2 - ball.Width / 2;

            // place ball as far up as possible (excluding level ceiling space)
            ball.Y = CurrentLevel.Ceiling + TILE_SIZE / 2;
            bool placedBall;
            do
            {
                placedBall = true;
                foreach (Brick brick in CurrentLevel.Bricks)
                    if (ball.Y < brick.Y + brick.Height)
                    {
                        ball.Y += TILE_SIZE;
                        placedBall = false;
                        break;
                    }
            }
            while (!placedBall);

            // wait before applying velocity
            ball.Velocity.Zero();
            QueueTask(Time.SECOND, () => 
                ball.Velocity = new Vector2D(0, BALL_SPEED)
            );
        }

        // display a point indicator when a brick is destroyed
        private void floatPoints(Brick brick)
        {
            if (brick.Value > 0)
            {
                // calculate point tile to show
                int tile = POINT_TILE + ((brick.Hits - 1) * 2);

                // create and setup 'floating' point
                GameObject pointFloater = new GameObject(brick.X, brick.Y, Tileset.Texture, Tileset.GetTile(tile), ghost: false);
                pointFloater.Velocity = new Vector2D(0, -2);

                // create animation 'floating point'
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

                // show 'floating point' for half a second
                AddGameObject(pointFloater);
                QueueTask(Time.HALF_SECOND, () => QueueFree(pointFloater));
            }
        }

        // set up starting ame objects, HUD, and variables
        public override void StartGame()
        {
            // add lives display if infinite lives is false
            if (!HasInfiniteLives)
            {
                AddGameObject(livesLabel);
                lifeDisplay.ForEach(life => AddGameObject(life));
            }

            // reset backdrop position

            backdrop.Y = 0 - Properties.Resources.levelBackdrop.Height + Screen.HeightPixels;
            AddGameObject(backdrop);

            // paddle and ball

            AddGameObject(paddle);
            AddGameObject(ball);

            // HUD

            AddGameObject(timeLabel);
            AddGameObject(gameTime);

            AddGameObject(scoreLabel);
            updateScore();

            AddGameObject(hiScoreLabel);
            updateHiScore();

            // initalize level

            currentLevel = 0;
            CurrentLevel.Build();
            levelRunning = true;

            gameStopwatch.Start();
            StartBall();
        }

        // save the game data (unimplimented)
        protected override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }

        // play a sound if HasSfx is true
        public override void PlaySound(Stream sound)
        {
            if (HasSfx) base.PlaySound(sound);
        }

        // remove game related objects, reset variables, and return to the menu
        public override void EndGame()
        {
            // stop the stopwatch
            gameStopwatch.Stop();

            QueueTask(Time.SECOND, () =>
            {
                // free ball[s]
                balls.ForEach(b => QueueFree(b));

                // reset lives
                lifeDisplay.ForEach(l => QueueFree(l));
                foreach (Animation heartbreak in heartbreak)
                    heartbreak.Reset();

                // remove any augmentation
                if (!(currentAugment is null))
                {
                    // reject and remove the current augment
                    currentAugment.Reject();
                    ClearAugment();
                }

                // free game objects
                QueueFree(livesLabel);
                QueueFree(scoreDisplay);
                QueueFree(scoreLabel);
                QueueFree(hiScoreLabel);
                QueueFree(hiScoreDisplay);
                QueueFree(timeLabel);
                QueueFree(Paddle);

                // has the player won
                if (currentLevel == 2 && CurrentLevel.BrickCount == 0) 
                    showLevelStats();
                else
                    ShowLooseScreen();
                
                // free the level
                CurrentLevel.Free();

                // reset score and time

                score = 0;
                levelRunning = false;

                gameStopwatch.Reset();
                QueueFree(gameTime);

                // wait then reset level and return the menu
                QueueTask(Time.SECOND * 4, () =>
                {
                    // free backdrop
                    QueueFree(backdrop);

                    // reset lives
                    lifes = START_LIFES;

                    // return to menu
                    PlaySound(Properties.Resources.exit);
                    AddGameObject(menu);
                    menu.Open();
                });
            });
        }
    }
}
