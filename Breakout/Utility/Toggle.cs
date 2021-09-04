using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.GameObjects;

namespace Breakout.Utility
{
    class Toggle : Button
    {
        // constants
        private const int MARGIN        = 5;
        private const int TOGGLE_OFF    = 46;
        private const int TOGGLE_MID    = 47;
        private const int TOGGLE_ON     = 48;

        // fields
        private bool state;
        private GameObject toggleObject;
        private Action onStateActive;
        private Action onStateInactive;

        public Toggle(float x, float y, string label, Action onStateActive, Action onStateInactive)
            : base(x, y, label)
        {
            // create toggle object
            toggleObject = new GameObject(x, y, BreakoutGame.Tileset.Texture, BreakoutGame.Tileset.GetTile(TOGGLE_OFF));

            // update width and height
            width += toggleObject.Width + MARGIN;
            height = toggleObject.Height;

            // assign on-click callback
            onClick = () =>
            {
                // call state event handler
                if (state)
                {
                    state = false;
                    onStateActive();
                    toggleObject.SourceRect = BreakoutGame.Tileset.GetTile(TOGGLE_MID);
                    BreakoutGame.QueueTask(Time.TWENTYTH_SECOND, () => toggleObject.SourceRect = BreakoutGame.Tileset.GetTile(TOGGLE_ON));
                }
                else
                {
                    state = true;
                    onStateInactive();
                    toggleObject.SourceRect = BreakoutGame.Tileset.GetTile(TOGGLE_MID);
                    BreakoutGame.QueueTask(Time.TWENTYTH_SECOND, () => toggleObject.SourceRect = BreakoutGame.Tileset.GetTile(TOGGLE_OFF));
                }
            };

            // offset the text to the right of the toggle
            offsetX = toggleObject.Width + MARGIN;
            offsetY = 2;
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
