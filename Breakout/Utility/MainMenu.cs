
//
//  Main Menu Class
//
//  Manages a group of buttons labels and toggles
//  and events to allow the user to play the game,
//  change options, and view infomation such as
//  credits and guides.
//

using System.Collections.Generic;
using System.Drawing;
using Breakout.GameObjects;
using Breakout.Render;

namespace Breakout.Utility
{
    class MainMenu : GameObject
    {
        private readonly string creditsText =
            "2021 WinForms Breakout v1.0.0 $NL "
            + "bit programming 2 assignment $NL "
            + "$NL $NL $NL "
            + "Programming, Art, and Design by $NL "
            + "$NL "
            + "Aardhyn Lavender $NL "
            + "$NL $NL $NL "
            + "Sound Effects from $NL "
            + "$NL "
            + "Axiom Verge $NL "
            + "by Tom Happ $NL "
            + "$NL $NL $NL "
            + "Based upon an Atari Breakout 1976 by $NL "
            + "$NL "
            + "Nolan Bushnell $NL "
            + "Steve Bristow $NL "
            + "Steve Wozniak $NL "
            + "Brad Stewart $NL "
            + "and Steve Jobs $NL "
            + "$NL $NL $NL "
            + "Developed using $NL "
            + "$NL "
            + "Visual Studio 2019 $NL "
            + "Logic Pro X $NL "
            + "GNU Image Manipulation program 2.10 $NL "
            + "microsoft Windows 10 $NL "
            + "apple macos 11 big sur $NL "
            + "$NL $NL $NL "
            + "Special Thanks $NL "
            + "$NL "
            + "Joy Gasson $NL "
            + "$NL "
            + "And of course $NL "
            + "YOU $NL ";

        private readonly string[] guideText = new string[4]
        {
            "break bricks by bouncing the ball off the paddle towards the bicks directing the paddle with your mouse",
            "bricks sometimes drop powerups that augment the gameplay to your advantage, collect them with your paddle",
            "if the ball drops bellow the paddle, youll loose a life, you have three lives for all 3 levels",
            "compleating game levels awards bonus points for lives not lost and brick clearing speed"
        };

        private readonly Bitmap[] guideImages = new Bitmap[4]
        {
            Properties.Resources.guideGraphicOne,
            Properties.Resources.guideGraphicTwo,
            Properties.Resources.guideGraphicThree,
            Properties.Resources.guideGraphicFour,
        };

        // fields
        private Button startButton;
        private Button guideButton;
        private Button optionsButton;
        private Button creditsButton;

        private Text credits;
        private Text credit;

        private Text soundLabel;
        private Toggle sfxToggle;
        private Toggle musicToggle;

        private Text worldGenLabel;
        private Toggle hasLevelsToggle;
        private Toggle hasCeilingToggle;
        private Toggle spawnAugmentsToggle;

        private Text gameplayLabel;
        private Toggle infiniteLivesToggle;
        private Toggle hasFloorToggle;
        private Toggle saveGameToggle;

        private Button exitOptionsMenu;

        private List<GameObject> optionsObjects;

        private List<GameObject> guideObjects;

        private BackdropManager backdropManager;
        private BackdropManager forgroundManager;
        private GameObject backdrop;
        private GameObject forground;

        private GameObject title;

        private List<GameObject> MenuObjects;

        public MainMenu()
            : base(0,0)
        {
            int currentY        = Screen.HeightPixels / 3;

            // title
            title               = new GameObject(0, currentY, Properties.Resources.title, true);
            title.X             = Screen.WidthPixels / 2 - title.Width / 2;
            title.Z             = 100;

            // starts the game
            startButton = new Button(0, currentY += 30, "START GAME", () => start(), soundFile: Properties.Resources.select);
            startButton.X       = Screen.WidthPixels / 2 - startButton.Width / 2;

            // shows a guide of how to play
            guideButton         = new Button(0, currentY += 10, "HOW TO PLAY", () => ShowGuide(), soundFile: Properties.Resources.select);
            guideButton.X       = Screen.WidthPixels / 2 - guideButton.Width / 2;

            guideObjects = new List<GameObject>();

            for (int i = 0; i < 4; i++)
            {
                y = Screen.HeightPixels;

                Text text = new Text(20, y += 100 * i, guideText[i], 25);
                GameObject image = new GameObject(Screen.WidthPixels - guideImages[i].Width - 20, y, guideImages[i], true);

                text.Velocity = image.Velocity = new Vector2D(0, -0.5f);

                guideObjects.Add(text);
                guideObjects.Add(image);
            }

            // shows options to the user

            optionsButton       = new Button(0, currentY += 10, "OPTIONS", () => ShowOptions(), soundFile: Properties.Resources.select);
            optionsButton.X     = Screen.WidthPixels / 2 - optionsButton.Width / 2;

            // show single credit

            credit              = new Text(0, 0, "aardhyn lavender 2021");
            credit.X            = Screen.WidthPixels / 2 - credit.Width / 2;
            credit.Y            = Screen.HeightPixels - credit.Height - 5;

            // set options

            // configuration

            BreakoutGame.HasSfx             = true;
            BreakoutGame.HasCeiling         = true;
            BreakoutGame.HasLevels          = true;
            BreakoutGame.HasAugments        = true;
            BreakoutGame.HasInfiniteLives   = false;
            BreakoutGame.HasFloor           = false;
            BreakoutGame.HasPersistance     = true;

            soundLabel          = new Text(10, 10, "Sound");

            sfxToggle           = new Toggle(10, 25, "sound effects", () => BreakoutGame.HasSfx = true, () => BreakoutGame.HasSfx = false, true);
            musicToggle         = new Toggle(10, 36, "music", () => { }, () => { });

            worldGenLabel       = new Text(10, 55, "Level Generation");

            hasLevelsToggle     = new Toggle(10, 70, "Levels mode", () => BreakoutGame.HasLevels = true, () => BreakoutGame.HasLevels = false, true);
            hasCeilingToggle    = new Toggle(10, 81, "ceiling mode", () => BreakoutGame.HasCeiling = true, () => BreakoutGame.HasCeiling = false, true);
            spawnAugmentsToggle = new Toggle(10, 92, "Spawn powerups", () => BreakoutGame.HasAugments = true, () => BreakoutGame.HasAugments = false, true);
            
            gameplayLabel       = new Text(10, 110, "Gameplay");

            infiniteLivesToggle = new Toggle(10, 125, "Infinite lives", () => BreakoutGame.HasInfiniteLives = true, () => BreakoutGame.HasInfiniteLives = false);
            hasFloorToggle      = new Toggle(10, 136, "floor", () => BreakoutGame.HasFloor = true, () => BreakoutGame.HasFloor = false);
            saveGameToggle      = new Toggle(10, 147, "save game", () => BreakoutGame.HasPersistance = true, () => BreakoutGame.HasPersistance = false, true);

            // exit options button

            exitOptionsMenu = new Button(10, Screen.HeightPixels - 15, "return", () => 
            {
                optionsObjects.ForEach(o => BreakoutGame.QueueFree(o));
                Open();
            }, soundFile: Properties.Resources.exit);

            // shows the game credits
            creditsButton       = new Button(0, currentY += 10, "CREDITS", () => ShowCredits(), soundFile: Properties.Resources.select);
            creditsButton.X     = Screen.WidthPixels / 2 - creditsButton.Width / 2;

            credits             = new Text(10, Screen.HeightPixels, creditsText, Screen.WidthPixels / 5);
            credits.Velocity    = new Vector2D(0, -0.25f);

            // backdrop behind the Menu
            backdrop            = new GameObject(0, 0, Properties.Resources.levelBackdrop, true);
            forground           = new GameObject(-16, 0, Properties.Resources.forground, true);
            backdropManager     = new BackdropManager(backdrop, 0.5f, Direction.UP);
            forgroundManager    = new BackdropManager(forground, 1.0f, Direction.UP);

            MenuObjects = new List<GameObject>
            {
                backdropManager, 
                forgroundManager,
                title, 
                startButton, 
                guideButton, 
                optionsButton, 
                creditsButton,
                credit
            };

            optionsObjects = new List<GameObject>
            {
                backdropManager,
                soundLabel,
                sfxToggle,
                musicToggle, 
                worldGenLabel,
                hasLevelsToggle,
                hasCeilingToggle,
                spawnAugmentsToggle,  
                gameplayLabel,
                infiniteLivesToggle,
                hasFloorToggle,
                saveGameToggle,
                exitOptionsMenu
            };
        }

        public void Open()
        {
            // initalise the menu
            MenuObjects.ForEach(o => 
            {
                BreakoutGame.AddGameObject(o);
                if (o is Button button) button.Enable(); 
            });

            backdropManager.Direction = Direction.UP;
            forgroundManager.Direction = Direction.UP;
        }

        private void close()
        {
            MenuObjects.ForEach(o =>
            {
                if (o is Button button) button.Disable();
                BreakoutGame.QueueFree(o);
            });
        }

        public override void Draw()
        {  }

        private void start()
        {
            close();
            BreakoutGame.QueueFree(this);

            // start the main game of breakout
            BreakoutGame.StartGame();
        }

        private void ShowGuide()
        {
            close();

            BreakoutGame.AddGameObject(backdropManager);
            backdropManager.Direction = Direction.DOWN;

            
            for (int i = 0; i < guideObjects.Count; i += 2)
            {
                int y = Screen.HeightPixels + (i * 40);
                guideObjects[i].Y = y;
                guideObjects[i + 1].Y = y;

                BreakoutGame.AddGameObject(guideObjects[i]);
                BreakoutGame.AddGameObject(guideObjects[i + 1]);
            }

            BreakoutGame.QueueTask(Time.SECOND * 19, () =>
            {
                guideObjects.ForEach(o => BreakoutGame.QueueFree(o));
                BreakoutGame.QueueFree(backdropManager);
                BreakoutGame.PlaySound(Properties.Resources.exit);

                Open();
            });
        }

        private void ShowOptions()
        {
            close();

            optionsObjects.ForEach(o => BreakoutGame.AddGameObject(o));
            backdropManager.Direction = Direction.DOWN;
        }

        private void ShowCredits()
        {
            close();

            BreakoutGame.AddGameObject(backdropManager);
            backdropManager.Direction = Direction.DOWN;

            BreakoutGame.AddGameObject(credits);

            BreakoutGame.QueueTask(39500, () =>
            {
                credits.Velocity.Zero();
                credits.Characters.ForEach(character => character.Velocity.Zero());

                BreakoutGame.QueueTask(Time.SECOND * 2, () =>
                {
                    closeCredits();
                });
            });
        }

        private void closeCredits()
        {
            BreakoutGame.QueueFree(credits);
            BreakoutGame.QueueFree(backdropManager);

            Open();

            // reset credits
            credits.Y = Screen.HeightPixels;
            credits.Velocity = new Vector2D(0, -0.25f);

            BreakoutGame.PlaySound(Properties.Resources.exit);
        }
    }
}
