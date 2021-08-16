using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.Utility;

namespace Breakout.GameObjects
{
    class Brick : GameObject
    {
        private const int DEBRIS = 4;
        private const int EXPLOSION_SPEED = 5;

        private static Vector2D[] trajectories =
        new Vector2D[DEBRIS]
        {
            new Vector2D(-EXPLOSION_SPEED, -EXPLOSION_SPEED),
            new Vector2D(EXPLOSION_SPEED, -EXPLOSION_SPEED),
            new Vector2D(-EXPLOSION_SPEED, EXPLOSION_SPEED),
            new Vector2D(EXPLOSION_SPEED, EXPLOSION_SPEED)
        };

        private int value;
        private int density;
        private int hits;

        private GameObject[] debris;

        public static int[] Map = new int[7] 
        {
            1, 1, 1, 3, 3, 3, 2
        };

        public Brick(float x, float y, Image texture, Rectangle sourceRect, int tileSpanX, int value, int density)
            : base(x, y, texture, sourceRect, tileSpanX)
        {
            debris          = new GameObject[DEBRIS];
            this.value      = value;
            this.density    = density;
            hits            = 0;

            InitalizeExplosion();
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

        public GameObject[] Debris
        {
            get => debris;
            set => debris = value;
        }

        public bool HasBeenDestroyed { get => hits >= density; }

        private void InitalizeExplosion()
        {
            for (int i = 0; i < DEBRIS; i++)
            {
                int debrisWidth = width / (DEBRIS / 2);
                int debrisHeight = height / (DEBRIS / 2);

                int x = (i * debrisWidth) % width;
                int y = (int)Math.Floor((float)i / (DEBRIS / 2)) * debrisHeight;

                int srcX = x + sourceRect.X;
                int srcY = y + sourceRect.Y;

                //System.Windows.Forms.MessageBox.Show($"{x} {y} {debrisWidth} {debrisHeight}");
                
                // create a new debris object at the aproprate starting position
                debris[i] = new GameObject(this.x + x, this.y + y, texture, new Rectangle(srcX, srcY, debrisWidth, debrisHeight), ghost: true);
                
                // assign trajectory
                debris[i].Velocity = trajectories[i];
            }
        }

        public override void OnCollsion(GameObject collider)
            => hits++;
    }
}
