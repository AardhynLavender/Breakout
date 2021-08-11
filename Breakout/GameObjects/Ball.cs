﻿using System;
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

        public Ball(int x, int y, float angle, float magnitude)
            : base(x, y, BreakoutGame.assets.Texture, BreakoutGame.assets.GetTile(0))
        {
            this.angle = angle;
            this.magnitude = magnitude;
            Velocity.X = Velocity.Y = 1;
        }
    }
}