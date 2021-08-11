
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

namespace Breakout
{
    class BreakoutGame : Game
    {
        private const int SCALE     = 3;
        private const int TILESIZE  = 16;

        private Ball ball;

        public static readonly Tileset assets = 
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

            ball = new Ball(10, 10, 0, 0);
            AddGameObject(ball);

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
            // move ball, bouncing off walls
            if (ball.X + ball.Velocity.X < 0 || ball.X + ball.Velocity.X + ball.Width > screen.Width / SCALE) ball.Velocity.X *= -1;
            else ball.X += ball.Velocity.X;

            if (ball.Y + ball.Velocity.Y < 0 || ball.Y + ball.Velocity.Y + ball.Height> screen.Height / SCALE) ball.Velocity.Y *= -1;
            else ball.Y += ball.Velocity.Y;

            // bounce off walls

            // bounce off paddle
        }

        public override void Render()
            => base.Render();


        public override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }

        public override void StartGame()
        {
            ball.Velocity = new Utility.Vector2D(5, 5);
        }

        public override void EndGame()
        {
            // clean up..
        }
    }
}
