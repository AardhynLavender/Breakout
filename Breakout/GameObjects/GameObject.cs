
//
//  GameObject class
//
//  A generic representation of a object in 2D space
//  with texture and renderiung managment and physics
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.Utility;
using Breakout.Render;
using System.Windows.Forms;

namespace Breakout.GameObjects
{
    class GameObject
    {
        // postion and size
        private float x;
        private float y;
        private int width;
        private int height;

        // texture
        private Image texture;
        private Rectangle sourceRect;
        
        public Image Texture 
        { 
            get => texture; 
            set => texture = value; 
        }
        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public int Width { get; set; }
        public int Height { get; set; }

        // physics
        private Vector2D velocity;
        private bool ghost;

        public Vector2D Velocity
        {
            get => velocity; 
            set => velocity = value;
        }

        public GameObject(float x, float y, Image texture, bool ghost = false)
        {
            X = x;
            Y = y;
            this.texture = texture;

            sourceRect = new Rectangle();

            sourceRect.Width = width = texture.Width;
            sourceRect.Height = height = texture.Height;

            velocity.Zero();
        }

        public GameObject(float x, float y, Image texture, Rectangle sourceRect, bool ghost = false)
        {
            this.x          = x;
            this.y          = y;
            this.texture    = texture;
            this.sourceRect = sourceRect;
            this.ghost      = ghost;

            this.width = sourceRect.Width;
            this.height = sourceRect.Height;

            velocity.Zero();
            velocity.X = 1;
            velocity.Y = 1;
        }

        public virtual void Draw(Breakout.Render.Screen screen)
            => screen.RenderCopy(texture, sourceRect, new Rectangle((int)x, (int)y, width, height));

        public virtual void OnDestory()
        {  }

        public virtual void OnCollsion(GameObject collider)
        {  }
    }
}
