using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private Rectangle inactive = BreakoutGame.Tileset.GetTile(TOGGLE_OFF);
        private Rectangle active = BreakoutGame.Tileset.GetTile(TOGGLE_ON);

        // fields
        private bool state;
        private GameObject toggleObject;
        private Action onStateActive;
        private Action onStateInactive;

        public Toggle(float x, float y, string label, Action onStateActive, Action onStateInactive, bool state = false)
            : base(x, y, label)
        {
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

        public override void OnFreeGameObject()
        {
            base.OnFreeGameObject();
            BreakoutGame.QueueFree(toggleObject);
        }

        public override void OnAddGameObject()
        {
            base.OnAddGameObject();
            BreakoutGame.AddGameObject(toggleObject);
        }
    }
}
