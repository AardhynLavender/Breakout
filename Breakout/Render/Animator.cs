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
        private int loopCap;
        private long loops;
        private Action onAnimationEnd;
        private Mode animationMode;

        private Game game;
        private GameObject gameObject;

        private List<Image> imageFrames;
        private List<Rectangle> tileFrames;
        private int currentFrame;

        public enum Mode
        {
            IMAGE,
            TILESET
        }

        // Construct with generic list of images
        public Animator(Game game, GameObject gameObject, List<Image> textures, int speed, Action onAnimationEnd = null, bool loop = true, int loopCap = -1)
        {
            this.game           = game;
            this.gameObject     = gameObject;
            this.speed          = speed / game.TickRate;
            this.onAnimationEnd = (onAnimationEnd is null) ? () => { } : onAnimationEnd;
            this.loop           = loop;
            this.loopCap        = loopCap; 

            imageFrames         = textures;
            animationMode       = Mode.IMAGE;
            frameCount          = textures.Count;
            animating           = true;
        }

        // Construct with generic list of tile coordinates
        public Animator(Game game, GameObject gameObject, List<Rectangle> textures, Tileset tileset, int speed, Action onAnimationEnd = null, bool loop = true, int loopCap = -1)
        {
            this.game           = game;
            this.gameObject     = gameObject;
            this.speed          = speed / game.TickRate;
            this.onAnimationEnd = (onAnimationEnd is null) ? () => { } : onAnimationEnd;
            this.loop           = loop;
            this.loopCap        = loopCap;

            tileFrames          = textures;
            animationMode       = Mode.TILESET;
            frameCount          = textures.Count;
            animating           = true;
        }

        public void Update()
        {
            Console.WriteLine();
            if (game.Tick % speed == 0 && animating)
            {
                currentFrame++;
                if (currentFrame == frameCount)
                {
                    if (loop)
                    {
                        currentFrame = 0;
                    }
                    if (!loop || loops > loopCap)
                    {
                        // stop animating and call the Action Delegate
                        animating = false;
                        currentFrame = 0;
                        onAnimationEnd();
                    }


                    loops++;
                }

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
