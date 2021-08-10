
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

namespace Breakout.GameObjects
{
    class GameObject
    {
        // postion and size
        private float x;
        private float y;
        private float width;
        private float height;

        public float X { get; set; }
        public float Y { get; set; }

        // texture
        private Image texture;
        private int sourceX;
        private int sourceY;
        private int sourceWidth;
        private int sourceHeight;
        
        public Image Texture 
        { 
            get => texture; 
            set => texture = value; 
        }

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

            width = texture.Width;
            height = texture.Height;

            velocity.Zero();
        }

        public GameObject(float x, float y, Image texture, int sourceWidth, int sourceHeight, bool ghost = false)
        {
            this.x = x;
            this.y = y;
            this.texture = texture;

            width = sourceWidth;
            height = sourceHeight;

            velocity.Zero();
        }

        public virtual void OnDestory()
        {  }

        public virtual void OnCollsion(GameObject collider)
        {  }
    }
}
