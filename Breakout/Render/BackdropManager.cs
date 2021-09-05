
//
//  Backdrop Manager Class
//
//  repeats the texture of the provided backdrop 
//  so it never apears offscreen optionally providing
//  translation for effect
//

using Breakout.Utility;
using Breakout.GameObjects;

namespace Breakout.Render
{
    class BackdropManager : GameObject
    {
        private GameObject backdropA;
        private GameObject backdropB;

        private Vector2D backdropVelocity;
        private Direction direction;
        private float speed;

        public Direction Direction
        {
            get => direction;
            set
            {
                direction = value;

                if (direction == Direction.UP)
                {
                    backdropVelocity = new Vector2D(0, -speed);
                }
                else if (direction == Direction.DOWN)
                {
                    backdropVelocity = new Vector2D(0, speed);
                }
                else throw new System.Exception("Unsupported direction");

                Start();
            }
        }

        public BackdropManager(GameObject backdrop, float speed, Direction direction, int z = 0)
            : base(backdrop.X, backdrop.Y)
        {
            // initalize fields
            this.speed = speed;

            // create backdrop partials
            backdropA = new GameObject(backdrop.X, backdrop.Y, backdrop.Texture, true);
            backdropB = new GameObject(backdrop.X, backdrop.Y, backdrop.Texture, true);
            backdropA.Z = backdropB.Z = z;

            // set direction
            Direction = direction;

            backdropB.Y = (direction == Direction.UP)
                ? backdropA.Y + backdropA.Height
                : backdropA.Y - backdropA.Height;

            Start();
        }

        public override void Draw()
        {  }

        public override void Update()
        {
            if (direction == Direction.UP)
            {
                if (backdropA.Y < backdropB.Y && backdropA.Y + backdropA.Height < 0)
                {
                    backdropA.Y = backdropB.Y + backdropB.Height;
                }
                else if (backdropB.Y < backdropB.Y && backdropB.Y + backdropA.Height < 0)
                {
                    backdropB.Y = backdropA.Y + backdropB.Height;
                }
            }
            else
            {
                if (backdropA.Y > 0 && backdropA.Y < backdropB.Y)
                {
                    backdropB.Y = backdropA.Y - backdropA.Height;
                }
                else if (backdropB.Y > 0 && backdropB.Y < backdropA.Y)
                {
                    backdropA.Y = backdropB.Y - backdropB.Height;
                }
            }
        }

        public void Stop()
        {
            backdropA.Velocity.Zero();
            backdropB.Velocity.Zero();
        }

        public void Start()
        {
            backdropA.Velocity = backdropB.Velocity = backdropVelocity;
        }

        public override void OnAddGameObject()
        {
            BreakoutGame.AddGameObject(backdropA);
            BreakoutGame.AddGameObject(backdropB);
        }

        public override void OnFreeGameObject()
        {
            BreakoutGame.QueueFree(backdropA);
            BreakoutGame.QueueFree(backdropB);
        }
    }
}
