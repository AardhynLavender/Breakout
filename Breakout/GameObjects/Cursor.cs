
//
//  Cursor Class
//
//  A simple derived game object that follows the invisable
//  Windows cursor around like a little puppy.
//

namespace Breakout.GameObjects
{
    class Cursor : GameObject
    {
        private const int Z_INDEX = 100;
        private const int TEXTURE = 45;
        private const int OFFSET = 3;

        public Cursor()
            : base(10, 00, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(TEXTURE), z:Z_INDEX, ghost: true)
        {  }

        public override void Update()
        {
            X = Screen.MouseX / Screen.Scale - OFFSET;
            Y = Screen.MouseY / Screen.Scale - OFFSET;
        }
    }
}
