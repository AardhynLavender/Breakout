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
        private GameObject backdrop;
        private GameObject title;

        private List<GameObject> MenuObjects;

        public MainMenu()
            : base(0,0)
        {
            int currentY = Screen.HeightPixels / 3;

            title = new GameObject(0, currentY, Properties.Resources.title, true);
            title.X = Screen.WidthPixels / 2 - title.Width / 2;

            startButton = new Button(0, currentY += 30, "START GAME", () => start());
            startButton.X = Screen.WidthPixels / 2 - startButton.Width / 2;

            guideButton = new Button(0, currentY += 10, "HOW TO PLAY", () => ShowGuide());
            guideButton.X = Screen.WidthPixels / 2 - guideButton.Width / 2;

            optionsButton = new Button(0, currentY += 10, "OPTIONS", () => ShowOptions());
            optionsButton.X = Screen.WidthPixels / 2 - optionsButton.Width / 2;

            creditsButton = new Button(0, currentY += 10, "CREDITS", () => ShowCredits());
            creditsButton.X = Screen.WidthPixels / 2 - creditsButton.Width / 2;

            backdrop = new GameObject(0, 0, Properties.Resources.levelBackdrop, true);

            credits = new Text(10, Screen.HeightPixels, creditsText, Screen.WidthPixels / 5);
            credits.Velocity = new Vector2D(0, -0.25f);

            MenuObjects = new List<GameObject>
            {
                backdrop, 
                title, 
                startButton, 
                guideButton, 
                optionsButton, 
                creditsButton
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

                BreakoutGame.QueueTask(Time.SECOND, () =>
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
