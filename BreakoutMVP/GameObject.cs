
//
//  GameObject Class
//
//  Defines an abstract object in the game
//  that can be drawn to the screen.
//

using System.Drawing;

namespace BreakoutMVP
{
    public abstract class GameObject
    {
        // constants
        private const int STANDARD_Z = 50;

        // fields
        protected float x;
        protected float y;
        protected float z;

        protected int width;
        protected int height;

        // static memebers
        protected static Graphics canvas;

        // constructor
        public GameObject(float x, float y, int z = STANDARD_Z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // where to draw
        public static Graphics Canvas
        {
            set => canvas = value;
        }

        // public position and dimension accessors
        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Z { get => z; set => z = value; }

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }

        // update and draw
        public abstract void Draw();
        public abstract void Update();
    }
}
