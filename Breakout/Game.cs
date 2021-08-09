
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
        private Screen screen;

        private int width;
        private int height;
        private int scale;

        private List<GameObject> gameObjects;

        public Game(Screen screen)
        {
            this.screen = screen;
            StartGame();
        }

        public virtual void Update()
        {
            foreach (GameObject gameObject in gameObjects) gameObject.Update();
        }

        public virtual void PhysicsProcess()
        {

        }

        public virtual void Render()
        {
            screen.RenderClear();

            foreach (GameObject gameObject in gameObjects)
                screen.Buffer.DrawImage(gameObject.Texture, gameObject.X, gameObject.Y);

            screen.RenderPresent();
        }

        public abstract void GameLoop();
        public abstract void StartGame();
        public abstract void EndGame();
        public abstract void SaveGame();
    }
}
