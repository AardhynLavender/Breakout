
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
using System.Threading;

using Breakout.Render;
using Breakout.Utility;
using Breakout.GameObjects;
using System.Media;
using System.IO;

namespace Breakout
{
    abstract class Game
    {
        private Screen screen;
        protected System.Windows.Forms.Timer ticker;
        protected SoundPlayer Media;
        protected long tick;
        protected List<GameObject> gameObjects;

        public Screen Screen 
        { 
            get => screen; 
            set => screen = value; 
        }

        public Action Quit 
        { 
            get; 
            set; 
        }

        protected Game(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
        {
            gameObjects = new List<GameObject>();
            this.ticker = ticker;
            this.screen = screen;
            this.Media = media;
        }

        protected virtual void Physics()
        {

        } 

        protected virtual void Render()
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

        public bool ObjectVisable(GameObject gameObject)
            => gameObject.X + gameObject.Width > 0 
            && gameObject.Y + gameObject.Height > 0 
            && gameObject.X < Screen.Width 
            && gameObject.Y < Screen.Height;

        public void PlaySound(Stream sound)
            => new SoundPlayer(sound).Play();

        public abstract void GameLoop();

        protected abstract void StartGame();

        protected abstract void EndGame();

        protected abstract void SaveGame();
    }
}
