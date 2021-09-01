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

        private Text startButton;
        private Text guideButton;
        private Text optionsButton;
        private Text creditsButton;
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

            startButton = new Text(0, currentY += 30, "START GAME");
            startButton.X = Screen.WidthPixels / 2 - startButton.Width / 2;

            guideButton = new Text(0, currentY += 10, "HOW TO PLAY");
            guideButton.X = Screen.WidthPixels / 2 - guideButton.Width / 2;

            optionsButton = new Text(0, currentY += 10, "OPTIONS");
            optionsButton.X = Screen.WidthPixels / 2 - optionsButton.Width / 2;

            creditsButton = new Text(0, currentY += 10, "CREDITS");
            creditsButton.X = Screen.WidthPixels / 2 - creditsButton.Width / 2;

            backdrop = new GameObject(0, 0, Properties.Resources.levelBackdrop, true);
            backdrop.Velocity = new Vector2D(0, -0.5f);

            credits = new Text(10, Screen.HeightPixels, creditsText, Screen.WidthPixels / 5);
            credits.Velocity = new Vector2D(0, -0.25f);

            MenuObjects = new List<GameObject>
            {
                title, startButton, guideButton, optionsButton, creditsButton
            };
        }

        public void Open()
        {
            backdrop = BreakoutGame.AddGameObject(backdrop);
            backdrop.Y = 0;
            title = BreakoutGame.AddGameObject(title);

            foreach (Text button in MenuObjects.Where(menuObject => menuObject is Text))
                BreakoutGame.AddTextObject(button);
        }

        private void close()
        {
            foreach (GameObject menuObject in MenuObjects)
            {
                // remove text if Text
                if (menuObject is Text text)
                    text.Characters.ForEach(
                        character => BreakoutGame.QueueFree(character)
                    );

                // free the object
                BreakoutGame.QueueFree(menuObject);
            }
                
            // free the menu itself
            BreakoutGame.QueueFree(this);
        }

        public override void Draw() {  }

        public override void Update()
        {
            if (isClicked(startButton))
                start();

            else if (isClicked(guideButton))
                ShowGuide();

            else if (isClicked(optionsButton))
                ShowOptions();

            else if (isClicked(creditsButton))
                ShowCredits();
        }

        private bool isHovered(GameObject button)
            => Screen.MouseX / BreakoutGame.Scale > button.X
            && Screen.MouseX / BreakoutGame.Scale < button.X + button.Width
            && Screen.MouseY / BreakoutGame.Scale > button.Y
            && Screen.MouseY / BreakoutGame.Scale < button.Y + button.Height;

        private bool isClicked(GameObject button)
            => isHovered(button) && Screen.MouseDown;

        private void start()
        {
            close();
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

            BreakoutGame.AddGameObject(credits);
            BreakoutGame.AddTextObject(credits);

            Console.WriteLine("queued!");
            BreakoutGame.QueueTask(39500, () =>
            {
                credits.Velocity = new Vector2D(0,0);
                credits.Characters.ForEach(character => character.Velocity = new Vector2D(0, 0));
                BreakoutGame.QueueTask(Time.SECOND, () =>
                {
                    BreakoutGame.QueueFree(credits);
                    credits.Characters.ForEach(character => BreakoutGame.QueueFree(character));
                    Open();
                });
            });
        }
    }
}
