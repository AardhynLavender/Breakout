
//
//  Game Class
//
//  Defines Game Objects and handles their updating 
//  and renering to the screen via double buffering 
//
//  The 'deleteQueue' allows the delayed removal of
//  items from a list as a collection cannot be modified
//  while being looped.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BreakoutMVP
{
    public sealed class Game
    {
        // constants
        private const int BRICKS = 40;
        private const int ROWS = 5;
        private const int BRICK_HEIGHT = 30;
        private const int CEILING = BRICK_HEIGHT * 2;

        // fields
        private Graphics buffer;
        private Bitmap bufferImage;
        private Graphics screen;
        private List<GameObject> gameObjects;
        private List<GameObject> deleteQueue;
        private List<Brick> bricks;
        private int mousePosX;
        private int width;
        private int height;

        // game objects
        private Ball ball;
        private Paddle paddle;

        // properties

        public int MousePosX
        {
            set => mousePosX = value;
        }

        // constructor
        public Game(int width, int height, Graphics screen)
        {
            // initalize fields
            bufferImage = new Bitmap(width, height);
            buffer      = Graphics.FromImage(bufferImage);

            gameObjects = new List<GameObject>();
            deleteQueue = new List<GameObject>();

            this.screen = screen;
            this.width  = width;
            this.height = height;

            // initalize static members
            GameObject.Canvas = buffer;

            // create and add ball
            paddle = (Paddle)AddGameObject(new Paddle(width, height));

            // create and add bricks
            int brickWidth = width / (BRICKS / ROWS);
            bricks = new List<Brick>(BRICKS);
            for (int i = 0; i < BRICKS; i++)
            {
                // calculate position of brick
                int x = i * brickWidth % width;
                int y = (int)Math.Floor((double)i * brickWidth / width) * BRICK_HEIGHT + CEILING;

                // add brick to game
                bricks.Add((Brick)AddGameObject(new Brick(x, y, brickWidth, BRICK_HEIGHT)));
            }

            // create and add ball
            ball = (Ball)AddGameObject(new Ball(width / 2, height / 2, width, height, paddle, bricks, (Brick brick) => deleteQueue.Add(brick)));
        }

        // called by the winform timer
        public void GameLoop()
        {
            // clear the screen
            buffer.FillRectangle(Brushes.Black, new Rectangle(0, 0, width, height));

            // update game
            paddle.MouseX = mousePosX;

            // update and draw game objects
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update();
                gameObject.Draw();
            }

            // draw to the screen
            screen.DrawImage(bufferImage, 0, 0);

            // clear the queue
            deleteQueue.ForEach(gameObject => gameObjects.Remove(gameObject));
            deleteQueue.Clear();
        }

        // adds a new game object to the pool
        private GameObject AddGameObject(GameObject gameObject)
        { 
            gameObjects.Add(gameObject);
            return gameObjects.Last();
        }
    }
}
