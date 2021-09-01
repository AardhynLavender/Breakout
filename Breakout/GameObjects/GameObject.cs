
//
//  GameObject class
//
//  A generic representation of a object in 2D space
//  with a texture and basic physics infomation.
//


using System.Drawing;
using Breakout.Render;
using Breakout.Utility;

namespace Breakout.GameObjects
{
    class GameObject : GameComponant
    {
        // postion and size
        protected float x;
        protected float y;
        protected int width;
        protected int height;

        // texture
        protected Image texture;
        protected Rectangle sourceRect;

        public Image Texture 
        { 
            get => texture; 
            set => texture = value; 
        }
        public float X 
        { 
            get => x; 
            set => x = value; 
        }

        public float Y 
        { 
            get => y;
            set => y = value; 
        }
        public int Width 
        { 
            get => width;
            set => width = value; 
        }

        public int Height 
        { 
            get => height;
            set => height = value; 
        }

        public Rectangle SourceRect 
        { 
            get => sourceRect; 
            set => sourceRect = value; 
        }

        // physics
        public Vector2D Velocity;
        protected bool ghost;

        public GameObject(float x, float y, Image texture, bool ghost = false)
        {
            X = x;
            Y = y;

            this.texture = texture;
            this.ghost = ghost;

            sourceRect = new Rectangle();

            sourceRect.Width = width = texture.Width;
            sourceRect.Height = height = texture.Height;

            Velocity.Zero();
        }

        public GameObject(float x, float y, Image texture, Rectangle sourceRect, int tileSpanX = 1, int tileSpanY = 1, bool ghost = false)
        {
            this.x          = x;
            this.y          = y;
            this.texture    = texture;
            this.sourceRect = sourceRect;
            this.ghost      = ghost;

            // span multuple tiles if specified
            this.sourceRect.Width *= tileSpanX;
            this.sourceRect.Height *= tileSpanY;

            width           = this.sourceRect.Width;
            height          = this.sourceRect.Height;

            Velocity.Zero();
        }

        public virtual void Draw()
            => Screen.RenderCopy(texture, sourceRect, new Rectangle((int)x, (int)y, width, height));

        public virtual void Physics()
        {
            X += Velocity.X;
            Y += Velocity.Y;
        }

        public virtual void Update()
        {  }

        public virtual void OnCollsion(GameObject collider)
        {  }
    }
}
