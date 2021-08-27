using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.GameObjects;

namespace Breakout.Render
{
    class Animation
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
        private Image idleImage;
        private List<Rectangle> tileFrames;
        private Rectangle idleRect;
        private int currentFrame;

        public bool Animating 
        { 
            get => animating;
            set
            {
                animating = value;
                if (!animating ) gameObject.SourceRect = idleRect;
            }
        }

        public enum Mode
        {
            IMAGE,
            TILESET
        }

        // Construct with generic list of images
        public Animation(Game game, GameObject gameObject, List<Image> textures, int speed, Image idleImage, Action onAnimationEnd = null, bool loop = true, int loopCap = -1)
        {
            this.game           = game;
            this.gameObject     = gameObject;
            this.speed          = speed / Game.TickRate;
            this.idleImage      = idleImage;
            this.onAnimationEnd = (onAnimationEnd is null) ? () => { } : onAnimationEnd;
            this.loop           = loop;
            this.loopCap        = loopCap; 

            imageFrames         = textures;
            animationMode       = Mode.IMAGE;
            frameCount          = textures.Count;
            animating           = false;
        }

        // Construct with generic list of tile coordinates
        public Animation(Game game, GameObject gameObject, List<Rectangle> textures, Tileset tileset, int speed, Rectangle? idleRect = null, Action onAnimationEnd = null, bool loop = true, int loopCap = -1)
        { 
            this.game           = game;
            this.gameObject     = gameObject;
            this.speed          = speed / Game.TickRate;
            this.idleRect       = idleRect is null ? textures[textures.Count - 1] : (Rectangle)idleRect;
            this.onAnimationEnd = (onAnimationEnd is null) ? () => { } : onAnimationEnd;
            this.loop           = loop;
            this.loopCap        = loopCap;

            tileFrames          = textures;
            animationMode       = Mode.TILESET;
            frameCount          = textures.Count;
            animating           = false;
        }

        public void Update()
        {
            if (game.Tick % speed == 0 && animating)
            {
                if (currentFrame == frameCount)
                {
                    loops++;
                    if (loop)
                    {
                        currentFrame = 0;
                    }
                    else if (loops > loopCap)
                    {
                        // stop animating and call the Action Delegate
                        animating = false;
                        onAnimationEnd();
                        return;
                    }
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

                currentFrame++;
            }
        }
    }
}
