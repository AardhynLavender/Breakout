
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
        protected List<Animator> animations;

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

        public int TickRate
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
            animations              = new List<Animator>();

            Media                   = media;
            processPhysics          = true;
        }

        protected virtual void Physics()
        {
            // update objects
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();

                // delete objects that are not visable
                if (!ObjectVisable(gameObject))
                    deleteQueue.Add(gameObject);
            }

            // update animations
            foreach (Animator animation in animations)
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

        protected Animator addAnimation(Animator animation)
        {
            animations.Add(animation);
            return animations.Last();
        }

        public bool ObjectVisable(GameObject gameObject)
            => gameObject.X + gameObject.Width > 0 
            && gameObject.Y + gameObject.Height > 0 
            && gameObject.X < Screen.Width 
            && gameObject.Y < Screen.Height;

        public void PlaySound(Stream sound)
            => new SoundPlayer(sound).Play();

        protected void Sleep(int milliseconds)
        {
            sleepTicks = milliseconds / TICKRATE;
            processPhysics = false;
        }

        protected void Sleep(int milliseconds, Action callback)
        {
            sleepTicks = milliseconds / TICKRATE;
            processPhysics = false;

            new Thread(() => 
            {
                do { Thread.Sleep(TICKRATE); } while (!processPhysics);
                callback();
            }).Start();
        }

        protected void doAfter(int milliseconds, Action callback)
        {
            float sleepFor = milliseconds / TICKRATE;

            new Thread(() =>
            {
                do
                {
                    Thread.Sleep(TICKRATE);
                    sleepFor--;
                }
                while (sleepFor > 0);

                callback();

            }).Start();
        }

        protected void doFor(int milliseconds, Action callback, int delay = TICKRATE)
        {
            float sleepFor = milliseconds / TICKRATE;

            new Thread(() =>
            {
                do 
                { 
                    Thread.Sleep(TICKRATE);
                    sleepFor--;

                    if (sleepTicks % delay == 0) 
                        callback();
                } 
                while (sleepFor > 0);
            }).Start();
        }

        public abstract void GameLoop();
        protected abstract void StartGame();
        protected abstract void EndGame();
        protected abstract void SaveGame();
    }
}
