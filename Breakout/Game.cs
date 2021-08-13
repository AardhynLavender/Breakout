
//
//  Game Class
//
//  Defines functionality and members for a abstract game object.
//  that manages GameObjects and renders infomation to a Screen
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.Render;
using Breakout.Utility;
using Breakout.GameObjects;
using System.Media;
using System.IO;

namespace Breakout
{
    abstract class Game
    {
        public Screen screen;
        public SoundPlayer Media;

        protected long tick;

        protected List<GameObject> gameObjects;

        public Game(Screen screen, SoundPlayer media)
        {
            gameObjects = new List<GameObject>();
            this.screen = screen;
            this.Media = media;
        }

        public virtual void Physics()
        {

        } 

        public virtual void Render()
        {
            screen.RenderClear();

            foreach (GameObject gameObject in gameObjects) 
                gameObject.Draw(screen);

            screen.RenderPresent();
        }

        public GameObject AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
            return gameObject;
        }

        public void RemoveGameObject(GameObject gameObject)
            => gameObjects.Remove(gameObject);

        public abstract void GameLoop();

        public abstract void StartGame();

        public void PlaySound(Stream sound)
            => new SoundPlayer(sound).Play();

        public abstract void EndGame();

        public abstract void SaveGame();
    }
}
