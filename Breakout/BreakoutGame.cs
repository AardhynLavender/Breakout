
//
//  BreakOutGame:Game class
//
//  Defines the functionality and members to create and play Atari Breakout
//  with score counters, powerups, levels, and saving.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.Render;
using Breakout.GameObjects;
using System.Drawing;

namespace Breakout
{
    class BreakoutGame : Game
    {
        private const int SCALE     = 3;
        private const int TILESIZE  = 16;

        private Ball ball;
        private GameObject paddle;

        public static readonly Tileset tileset = 
            new Tileset(
                Properties.Resources.tileset, 
                Properties.Resources.tileset.Width, 
                TILESIZE, 
                TILESIZE
            );

        public BreakoutGame(Screen screen) 
            : base(screen)
        {
            screen.Scale = SCALE;

            ball = (Ball)AddGameObject(new Ball(screen.WidthPixels / 2, 50, 0, 0));
            paddle = AddGameObject(new GameObject(screen.WidthPixels / 2 - 24, screen.HeightPixels - 32, tileset.Texture, tileset.GetTile(32), 3));

            StartGame();
        }

        public override void GameLoop()
        {
            Physics();
            Render();
            tick++;
        }

        public override void Physics()
        {
            // update paddle position
            paddle.X = screen.MouseX / SCALE - 24;

            // move ball by its X velocity, bouncing off vertical walls
            if (ball.X + ball.Velocity.X < 0 || ball.X + ball.Velocity.X + ball.Width > screen.WidthPixels) ball.Velocity.X *= -1;
            else ball.X += ball.Velocity.X;

            // move ball by its Y velocity, bouncing off horizontal walls
            if (ball.Y + ball.Velocity.Y < 0 || ball.Y + ball.Velocity.Y + ball.Height > screen.HeightPixels) ball.Velocity.Y *= -1;
            else ball.Y += ball.Velocity.Y;

            // bounce of paddle, applying angular velocity depending
            // on the collison point on the paddle
            if (ball.X < paddle.X + paddle.Width
                && ball.X + ball.Width > paddle.X 
                && ball.X + ball.Velocity.Y < paddle.Y + paddle.Height 
                && ball.Y + ball.Velocity.Y > paddle.Y)
            {
                float relX = (ball.X - paddle.X - paddle.Width / 2) / paddle.Width;
                ball.Velocity.X = relX * 5;
                ball.Velocity.Y *= -1;
            }
        }

        public override void Render()
            => base.Render();


        public override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }

        public override void StartGame()
        {
            ball.Velocity = new Utility.Vector2D(0, 5);
        }

        public override void EndGame()
        {
            // clean up..
        }
    }
}
