using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.GameObjects
{
    class Brick : GameObject
    {
        private int value;
        private int density;
        private int hits;

        public Brick(float x, float y, Image texture, Rectangle sourceRect, int tileSpanX, int value, int density)
            : base(x, y, texture, sourceRect, tileSpanX)
        {
            this.value = value;
            this.density = density;
            hits = density;
        }

        public int Value
        { 
            get => value; 
            set => this.value = value; 
        }

        public int Hits
        { 
            get => hits; 
            set => hits = value; 
        }

        public override void OnCollsion(GameObject collider)
            => hits--;
    }
}
