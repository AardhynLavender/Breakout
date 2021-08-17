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
        private const int DEBRIS = 9;
        private const int EXPLOSION_SPEED = 4;

        // Debris Trajectory Index
        // [0][1][2]
        // [3][4][5]
        // [6][7][8]
        private Vector2D[] trajectories =
        new Vector2D[DEBRIS]
        {
            new Vector2D(-EXPLOSION_SPEED, -EXPLOSION_SPEED),
            new Vector2D(0, -EXPLOSION_SPEED),
            new Vector2D(EXPLOSION_SPEED, -EXPLOSION_SPEED),
            new Vector2D(-EXPLOSION_SPEED, 0),
            new Vector2D(0,0),
            new Vector2D(EXPLOSION_SPEED, 0),
            new Vector2D(-EXPLOSION_SPEED, EXPLOSION_SPEED),
            new Vector2D(0, EXPLOSION_SPEED),
            new Vector2D(EXPLOSION_SPEED, EXPLOSION_SPEED)
        };

        private int value;
        private int density;
        private int hits;

        private Random random;

        private GameObject[] debris;

        public static int[] Map = new int[8] 
        {
            1, 1, 1, 3, 3, 3, 2, 2
        };

        public Brick(float x, float y, Image texture, Rectangle sourceRect, int tileSpanX, int value, int density, Random random)
            : base(x, y, texture, sourceRect, tileSpanX)
        {
            // initalize fields
            this.value      = value;
            this.density    = density;
            this.random     = random;

            debris          = new GameObject[DEBRIS];
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
            // randomise the center fragment
            if (trajectories.Length % 2 != 0)
            {
                int x;
                int y;

                // prevent (0,0) velocity
                do
                {
                    x = random.Next(-EXPLOSION_SPEED, EXPLOSION_SPEED);
                    y = random.Next(-EXPLOSION_SPEED, EXPLOSION_SPEED);
                }
                while (x == 0 && y == 0);

                // assign randomised trajectory to the center fragment
                trajectories[(int)Math.Floor((float)trajectories.Length / 2.0f)] = new Vector2D(x, y);
            }

            for (int i = 0; i < DEBRIS; i++)
            {
                int debrisWidth = width / (int)Math.Sqrt(DEBRIS);
                int debrisHeight = height / (int)Math.Sqrt(DEBRIS);

                int x = (i * debrisWidth) % width;
                int y = (int)Math.Floor((float)i / Math.Sqrt(DEBRIS)) * debrisHeight;

                int srcX = x + sourceRect.X;
                int srcY = y + sourceRect.Y;
                
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
