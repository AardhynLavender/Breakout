
using Breakout.Utility;
using System;

namespace Breakout.GameObjects.Augments
{
    class TripleBallAugment : Augment
    {
        private const int EXTRA_BALLS = 2;

        public TripleBallAugment(BreakoutGame game)
            : base(game, BreakoutGame.tileset.Texture, BreakoutGame.tileset.GetTile(18))
        {  }

        protected override void apply()
        {
            Console.WriteLine("Augment Applied!");

            int x = (int)breakout.BallPosition.X;
            int y = (int)breakout.BallPosition.Y;

            Ball a = new Ball(x, y, 0, 0) { Velocity = new Vector2D(-5, -5) };
            Ball b = new Ball(x, y, 0, 0) { Velocity = new Vector2D(5, -5) };

            // add to game and balls list
            breakout.Balls.Add((Ball)breakout.AddGameObject(a));
            breakout.Balls.Add((Ball)breakout.AddGameObject(b));
        }

        protected override void reject()
        {
            Console.WriteLine("rejected!");
            breakout.ClearAugment();
        }

        protected override bool condition()
            => breakout.BallCount == 1;
    }
}
