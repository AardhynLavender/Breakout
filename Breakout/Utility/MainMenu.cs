using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.GameObjects;

namespace Breakout.Utility
{
    class MainMenu : GameObject
    {
        private const string creditsText =
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

        private Button startButton;
        private Button guideButton;
        private Button optionsButton;
        private Button creditsButton;

        private Text credits;

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

        private GameObject backdrop;
        private GameObject title;

        private List<GameObject> MenuObjects;

        public MainMenu()
            : base(0,0)
        {
            int currentY        = Screen.HeightPixels / 3;

            title               = new GameObject(0, currentY, Properties.Resources.title, true);
            title.X             = Screen.WidthPixels / 2 - title.Width / 2;

            // starts the game
            startButton         = new Button(0, currentY += 30, "START GAME", () => start());
            startButton.X       = Screen.WidthPixels / 2 - startButton.Width / 2;

            // shows a guide of how to play
            guideButton         = new Button(0, currentY += 10, "HOW TO PLAY", () => ShowGuide());
            guideButton.X       = Screen.WidthPixels / 2 - guideButton.Width / 2;

            // shows options to the user
            optionsButton       = new Button(0, currentY += 10, "OPTIONS", () => ShowOptions());
            optionsButton.X     = Screen.WidthPixels / 2 - optionsButton.Width / 2;

            soundLabel          = new Text(10, 10, "Sound");
            sfxToggle           = new Toggle(10, 25, "sound effects", () => { }, () => { }, true);
            musicToggle         = new Toggle(10, 36, "music", () => { }, () => { });
            worldGenLabel       = new Text(10, 55, "Level Generation");
            hasLevelsToggle     = new Toggle(10, 70, "Single Level mode", () => { }, () => { });
            hasCeilingToggle    = new Toggle(10, 81, "ceiling mode", () => { }, () => { }, true);
            spawnAugmentsToggle = new Toggle(10, 92, "Spawn powerups", () => { }, () => { }, true);
            gameplayLabel       = new Text(10, 110, "Gameplay");
            infiniteLivesToggle = new Toggle(10, 125, "Infinite lives", () => { }, () => { });
            hasFloorToggle      = new Toggle(10, 136, "floor", () => { }, () => { });
            saveGameToggle      = new Toggle(10, 147, "save game", () => { }, () => { }, true);

            exitOptionsMenu = new Button(10, Screen.HeightPixels - 15, "return", () => 
            {
                optionsObjects.ForEach(o => BreakoutGame.QueueFree(o));
                BreakoutGame.QueueFree(backdrop);
                Open();
            });

            // shows the game credits
            creditsButton       = new Button(0, currentY += 10, "CREDITS", () => ShowCredits());
            creditsButton.X     = Screen.WidthPixels / 2 - creditsButton.Width / 2;

            credits             = new Text(10, Screen.HeightPixels, creditsText, Screen.WidthPixels / 5);
            credits.Velocity    = new Vector2D(0, -0.25f);

            // backdrop behind the Menu
            backdrop            = new GameObject(0, 0, Properties.Resources.levelBackdrop, true);

            MenuObjects = new List<GameObject>
            {
                backdrop, 
                title, 
                startButton, 
                guideButton, 
                optionsButton, 
                creditsButton
            };

            optionsObjects = new List<GameObject>
            {
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
            System.Console.WriteLine("called!");
            // initalise the menu
            MenuObjects.ForEach(o => 
            {
                BreakoutGame.AddGameObject(o);
                if (o is Button button) button.Enable(); 
            });

            backdrop.Y = 0;
            backdrop.Velocity = new Vector2D(0, -0.5f);
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
        }

        private void ShowOptions()
        {
            close();

            backdrop = BreakoutGame.AddGameObject(backdrop);
            backdrop.Velocity = new Vector2D(0, 0.5f);
            backdrop.Y = -backdrop.Height + Screen.HeightPixels;

            optionsObjects.ForEach(o => BreakoutGame.AddGameObject(o));
        }

        private void ShowCredits()
        {
            close();

            backdrop = BreakoutGame.AddGameObject(backdrop);
            backdrop.Velocity = new Vector2D(0, 0.5f);
            backdrop.Y = -backdrop.Height + Screen.HeightPixels;

            BreakoutGame.AddGameObject(credits);

            BreakoutGame.QueueTask(39500, () =>
            {
                credits.Velocity.Zero();
                credits.Characters.ForEach(character => character.Velocity.Zero());

                BreakoutGame.QueueTask(Time.SECOND * 2, () =>
                {
                    BreakoutGame.QueueFree(credits);
                    BreakoutGame.QueueFree(backdrop);
                    Open();

                    // reset credits
                    credits.Y = Screen.HeightPixels;
                    credits.Velocity = new Vector2D(0, -0.25f);
                });
            });
        }
    }
}
