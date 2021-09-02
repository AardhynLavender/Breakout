
//
//  Paddle Class
//
//  A derived Game Object that follows
//  the mouses x position. drawiung as
//  a simple green line on the screen
//
//  The paddle gets the mouse position
//  based off a public memeber asssigned
//  by the form class when the MouseMove
//  event is called
//

using System.Drawing;

namespace BreakoutMVP
{
    class Paddle : GameObject
    {
        // constants
        private const int WIDTH = 50;
        private const int FLOOR = 50;

        // constructor
        public Paddle(int width, int height)
            : base(0, 0)
        {
            this.width = WIDTH;
            y = height - FLOOR;
        }

        // position of the mouse (set by Form1)
        public int MouseX { get; set; }

        public override void Update()
        {
            // set paddle to mouses position
            x = MouseX - WIDTH / 2;
        }

        public override void Draw()
        {
            canvas.DrawLine(Pens.Green, x, y, x + WIDTH, y);
        }
    }
}
