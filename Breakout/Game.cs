
//
//
//
//
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.Render;
using Breakout.GameObjects;

namespace Breakout
{
    abstract class Game
    {
        protected Screen screen;

        protected long tick;

        protected List<GameObject> gameObjects;

        public Game(Screen screen)
        {
            gameObjects = new List<GameObject>();
            this.screen = screen;

            StartGame();
        }

        public virtual void Update()
        {
            foreach (GameObject gameObject in gameObjects)
            {

            }
        }

        public virtual void Render()
        {
            screen.RenderClear();

            foreach (GameObject gameObject in gameObjects)
                screen.RenderCopy(gameObject.Texture, gameObject.X, gameObject.Y, gameObject.Texture.Width, gameObject.Texture.Height);

            screen.RenderPresent();
        }

        public void AddGameObject(GameObject gameObject)
            => gameObjects.Add(gameObject);

        public abstract void GameLoop();

        public abstract void StartGame();

        public abstract void EndGame();

        public abstract void SaveGame();
    }
}
