
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
        private const int TICKRATE = 17;

        protected Screen screen;
        protected System.Windows.Forms.Timer ticker;
        protected SoundPlayer Media;
        protected long tick;
        protected List<GameObject> gameObjects;
        protected List<GameObject> deleteQueue;
        protected List<Animation> animations;

        protected bool processPhysics;
        private int sleepTicks;

        protected int SleepTicks
        {
            get => sleepTicks;
            set 
            {
                sleepTicks = value;
                if (sleepTicks <= 0) processPhysics = true;
            }
        }

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

        public static int TickRate
        {
            get => TICKRATE;
        }

        public long Tick
        {
            get => tick;
        }

        protected Game(Screen screen, SoundPlayer media, System.Windows.Forms.Timer ticker)
        {
            this.ticker             = ticker;
            this.ticker.Interval    = TICKRATE;
            this.screen             = screen;

            gameObjects             = new List<GameObject>();
            deleteQueue             = new List<GameObject>();
            animations              = new List<Animation>();

            Media                   = media;
            processPhysics          = true;
        }

        protected virtual void Physics()
        {
            // update objects
            foreach (GameObject gameObject in gameObjects)
                gameObject.Update();

            // update animations
            foreach (Animation animation in animations)
                animation.Update();

            // free objects queued for removal
            freeQueue();
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

        protected void queueFree(GameObject gameObject)
            => deleteQueue.Add(gameObject);

        private void freeGameObject(GameObject gameObject)
            => gameObjects.Remove(gameObject);

        protected void freeQueue()
        {
            deleteQueue.ForEach(gameObject => freeGameObject(gameObject));
            deleteQueue.Clear();
        }

        protected Animation addAnimation(Animation animation)
        {
            animations.Add(animation);
            return animations.Last();
        }

        public bool ObjectVisable(GameObject gameObject)
            => gameObject.X + gameObject.Width > 0 
            && gameObject.Y + gameObject.Height > 0 
            && gameObject.X < Screen.Width 
            && gameObject.Y < Screen.Height;

        public static bool DoesCollide(GameObject a, GameObject b)
        {
            bool collides = false;

            if (a.X + a.Width > b.X
                && a.X < b.X + b.Width
                && a.Y + a.Height > b.Y
                && a.Y < b.Y + b.Height)
            {
                a.OnCollsion(b);
                b.OnCollsion(a);
                collides = true;
            }

            return collides;
        }

        public void PlaySound(Stream sound)
            => new SoundPlayer(sound).Play();

        public static void doAfter(int milliseconds, Action callback)
        {
            float sleepFor = milliseconds / TickRate;

            Thread thread = new Thread(() =>
            {
                do
                {
                    Thread.Sleep(TICKRATE);
                    sleepFor--;
                }
                while (sleepFor > 0);

                callback();

            });
            thread.Start();

        }

        public abstract void GameLoop();
        protected abstract void StartGame();
        protected abstract void EndGame();
        protected abstract void SaveGame();
    }
}
