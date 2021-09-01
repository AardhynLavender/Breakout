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
        private Text startButton;
        private Text guideButton;
        private Text optionsButton;
        private Text creditsButton;
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

            MenuObjects = new List<GameObject>
            {
                backdrop, title, startButton, guideButton, optionsButton, creditsButton,
            };
        }

        public void Open()
        {
            BreakoutGame.AddGameObject(backdrop);
            BreakoutGame.AddGameObject(title);

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
            
        }
    }
}
