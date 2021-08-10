
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
        private float srcX;
        private float srcY;
        private int srcWidth;
        private int srcHeight;
        
        public Image Texture 
        { 
            get => texture; 
            set => texture = value; 
        }
        public float X { get; set; }
        public float Y { get; set; }
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

            srcWidth = width = texture.Width;
            srcHeight = height = texture.Height;

            velocity.Zero();
        }

        public GameObject(float x, float y, Image texture, int srcX, int srcY, int srcWidth, int srcHeight, bool ghost = false)
        {
            this.x = x;
            this.y = y;

            this.texture = texture;

            this.srcX = srcX;
            this.srcY = srcY;
            this.srcWidth = srcWidth;
            this.srcHeight = srcHeight;

            this.ghost = ghost;

            velocity.Zero();
        }

        public virtual void Draw(Screen screen)
        {
            Rectangle src = new Rectangle((int)srcX, (int)srcY, srcWidth, srcHeight);
            Rectangle des = new Rectangle((int)x, (int)y, width, height);
            screen.RenderCopy(texture, src, des);
        }

        public virtual void OnDestory()
        {  }

        public virtual void OnCollsion(GameObject collider)
        {  }
    }
}
