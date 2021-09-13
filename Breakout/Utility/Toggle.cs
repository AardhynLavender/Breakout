
//
//  Toggle Class
//
//  A Button with state. Calling the provided action
//  delegates when toggled and animating the toggle
//  texture.
//

using System;
using System.Drawing;

using Breakout.GameObjects;

namespace Breakout.Utility
{
    class Toggle : Button
    {
        // constants
        private const int MARGIN            = 5;
        private const int DEAD_TILE_SPACE   = 6;
        private const int TOGGLE_OFF        = 46;
        private const int TOGGLE_MID        = 47;
        private const int TOGGLE_ON         = 48;

        // fields
        private Rectangle inactive;
        private Rectangle active;
        private GameObject toggleObject;

        private Action onStateActive;
        private Action onStateInactive;

        private bool state;

        public Toggle(float x, float y, string label, Action onStateActive, Action onStateInactive, bool state = false)
            : base(x, y, label)
        {
            // initalize fields
            inactive = BreakoutGame.Tileset.GetTile(TOGGLE_OFF);
            active = BreakoutGame.Tileset.GetTile(TOGGLE_ON);

            // create toggle object
            toggleObject = new GameObject(x, y, BreakoutGame.Tileset.Texture, state ? active : inactive);

            // update width and height
            width += toggleObject.Width + MARGIN;
            height = toggleObject.Height - DEAD_TILE_SPACE;

            // assign on-click callback
            onClick = () =>
            {
                // call state event handler
                if (state)
                {
                    state = false;
                    onStateInactive();

                    toggleObject.SourceRect = BreakoutGame.Tileset.GetTile(TOGGLE_MID);
                    BreakoutGame.QueueTask(Time.TWENTYTH_SECOND, () => toggleObject.SourceRect = inactive);
                }
                else
                {
                    state = true;
                    onStateActive();

                    toggleObject.SourceRect = BreakoutGame.Tileset.GetTile(TOGGLE_MID);
                    BreakoutGame.QueueTask(Time.TWENTYTH_SECOND, () => toggleObject.SourceRect = active);
                }
            };

            // offset the text to the right of the toggle
            offsetX = toggleObject.Width + MARGIN;
            offsetY = 2;

            // set state;
            this.state = state;
        }

        // called when the object is freed from the game
        public override void OnFreeGameObject()
        {
            base.OnFreeGameObject();
            BreakoutGame.QueueFree(toggleObject);
        }

        // called when the object is added to the game
        public override void OnAddGameObject()
        {
            base.OnAddGameObject();
            BreakoutGame.AddGameObject(toggleObject);
        }
    }
}
