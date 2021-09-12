
//
//  Worm Class
//
//  Defines a game object with a worm texture that moves back
//  and forth on the provided y coordinate. varying its
//  offscreen wait time and taking part in the game physics
//  simulation.
//

using Breakout.Render;
using Breakout.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.GameObjects
{
    class Worm : GameObject
    {
        // constants
        private const int RIGHT = 54;
        private const int LEFT = 63;
        private const int SPAN = 2;
        private const int SPEED = 6;
        private const int Z_POS = 60;

        // fields
        private int speed = SPEED;
        private int screenWidth = BreakoutGame.Screen.WidthPixels;
        private int waitTime => Random.Next(Time.TENTH_SECOND, Time.SECOND);

        private Animation right;
        private Animation left;

        // constructor
        public Worm(int y)
            : base(0, y, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(RIGHT, SPAN), Z_POS)
        {
            // put worm off screen
            x -= width;

            right = BreakoutGame.AddAnimation(new Animation(
                BreakoutGame,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Tileset.GetTile(RIGHT, SPAN),
                    BreakoutGame.Tileset.GetTile(RIGHT + SPAN, SPAN),
                    BreakoutGame.Tileset.GetTile(RIGHT + SPAN * 2, SPAN),
                    BreakoutGame.Tileset.GetTile(RIGHT + SPAN, SPAN)
                },
                BreakoutGame.Tileset,
                Time.TENTH_SECOND,
                loop: true
            ));

            left = BreakoutGame.AddAnimation(new Animation(
                BreakoutGame,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Tileset.GetTile(LEFT, SPAN),
                    BreakoutGame.Tileset.GetTile(LEFT + SPAN, SPAN),
                    BreakoutGame.Tileset.GetTile(LEFT + SPAN * 2, SPAN),
                    BreakoutGame.Tileset.GetTile(LEFT + SPAN, SPAN)
                },
                BreakoutGame.Tileset,
                Time.TWENTYTH_SECOND,
                loop: true
            ));

            left.Start();
        }

        public override void Update()
        {
            // is the worm off screen
            if (x + width < 0 && Velocity.X < 0.0f)
            {
                // flip animation
                right.Stop();
                left.Start();

                // reverse direction
                BreakoutGame.QueueTask(waitTime, () => Velocity.X = speed);
            }
            else if (x > screenWidth && Velocity.X > 0.0f)
            {
                // flip animation
                left.Stop();
                right.Start();

                // reverse direction
                BreakoutGame.QueueTask(waitTime, () => Velocity.X = -speed);
            }
        }

        public override void OnAddGameObject()
            => BreakoutGame.QueueTask(waitTime, () => Velocity.X = speed);
    }
}
