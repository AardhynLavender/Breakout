using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.GameObjects;

namespace Breakout.Render
{
    class Animator
    {
        private int frameCount;
        private int speed;
        private bool animating;
        private bool loop;
        private Action onAnimationEnd;
        private Mode animationMode;

        private Game game;
        private GameObject gameObject;

        private List<Image> imageFrames;
        private List<Rectangle> tileFrames;
        private int idleFrame;
        private int currentFrame;

        public enum Mode
        {
            IMAGE,
            TILESET
        }

        // Construct with generic list of images
        public Animator(Game game, GameObject gameObject, List<Image> textures, int speed, Action onAnimationEnd = null)
        {
            this.game           = game;
            this.gameObject     = gameObject;
            this.speed          = speed / game.TickRate;
            this.onAnimationEnd = (onAnimationEnd is null) ? () => { } : onAnimationEnd;

            imageFrames         = textures;
            animationMode       = Mode.IMAGE;
        }

        // Construct with generic list of tile coordinates
        public Animator(Game game, GameObject gameObject, List<Rectangle> textures, Tileset tileset, int speed, Action onAnimationEnd = null)
        {
            this.game = game;
            this.gameObject = gameObject;
            this.speed = speed / game.TickRate;
            this.onAnimationEnd = (onAnimationEnd is null) ? () => { } : onAnimationEnd;

            tileFrames = textures;
            animationMode = Mode.TILESET;
        }

        public void Update()
        {
            if (game.Tick % speed == 0 && animating)
            {
                if (loop)
                {
                    // next frame, looping back to start
                    currentFrame += 1 % frameCount;
                }
                else if (currentFrame + 1 == frameCount)
                {
                    // stop animating and call the Action Delegate
                    animating = false;
                    onAnimationEnd();
                }
                else currentFrame++;

                if (animationMode == Mode.IMAGE)
                {
                    // update the the objects texture
                    gameObject.Texture = imageFrames[currentFrame];
                }
                else
                {
                    // update the source rect for the tileset
                    gameObject.SourceRect = tileFrames[currentFrame];
                }
            }
        }
    }
}
