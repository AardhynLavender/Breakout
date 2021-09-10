
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
        private const int TEXTURE = 54;
        private const int SPAN = 2;

        // fields
        private int speed = 2;
        private int screenWidth = BreakoutGame.Screen.WidthPixels;
        private int waitTime => Random.Next(Time.SECOND, Time.SECOND * 5);

        private Animation animation;

        // constructor
        public Worm(int y)
            : base(0, y, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(TEXTURE, SPAN), 60)
        {
            // put worm off screen
            x -= width;

            animation = BreakoutGame.AddAnimation(new Animation(
                BreakoutGame,
                this,
                new List<Rectangle>
                {
                    BreakoutGame.Tileset.GetTile(TEXTURE, SPAN),
                    BreakoutGame.Tileset.GetTile(TEXTURE + SPAN, SPAN),
                    BreakoutGame.Tileset.GetTile(TEXTURE + SPAN * 2, SPAN)
                },
                BreakoutGame.Tileset,
                Time.TENTH_SECOND,
                loop: true
            ));

            animation.Start();
        }

        public override void Update()
        {
            // is the worm off screen
            if (x + width < 0 && Velocity.X < 0.0f)
                BreakoutGame.QueueTask(waitTime, () => Velocity.X = speed);

            else if (x > screenWidth && Velocity.X > 0.0f)
                BreakoutGame.QueueTask(waitTime, () => Velocity.X = -speed);
        }

        public override void OnAddGameObject()
            => BreakoutGame.QueueTask(waitTime, () => Velocity.X = speed);
    }
}
