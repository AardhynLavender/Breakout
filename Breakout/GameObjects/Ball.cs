using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout;

namespace Breakout.GameObjects
{
    class Ball : GameObject
    {
        private float angle;
        private float magnitude;

        private const int BALL_TEXTURE = 31;

        public Ball(int x, int y, float angle, float magnitude)
            : base(x, y, BreakoutGame.assets.Texture, BreakoutGame.assets.GetTile(BALL_TEXTURE))
        {
            this.angle = angle;
            this.magnitude = magnitude;
            Velocity.X = Velocity.Y = 1;
        }
    }
}
