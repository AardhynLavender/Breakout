
//
//  Ball Class
//
//  A derived GameObject that defines an object with a ball texture
//

namespace Breakout.GameObjects
{
    class Ball : GameObject
    {
        private float angle;
        private float magnitude;

        public Ball(int x = 0, int y = 0, float angle = 180, float magnitude = 5)
            : base(x, y, BreakoutGame.Ballset.Texture, BreakoutGame.Ballset.GetTile(0))
        {
            this.angle = angle;
            this.magnitude = magnitude;
        }
    }
}
