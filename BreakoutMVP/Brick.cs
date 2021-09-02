
// 
//  Brick Class
//
//  A derived Game Object that draws
//  as a simple cube of the provided
//  dimentions on the screen.
//

using System.Drawing;

namespace BreakoutMVP
{
    class Brick : GameObject
    {
        // constructor
        public Brick(int x, int y, int width, int height)
            : base(x, y)
        {
            // initalize fields
            this.width = width;
            this.height = height;
        }

        public override void Update()
        {
            // brick's a brick! nothing more needs updated...
        }

        public override void Draw()
        {
            // draw brick
            canvas.DrawRectangle(Pens.LightBlue, x, y, width, Height);
        }
    }
}
