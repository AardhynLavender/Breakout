
//
//  Augment Class
//
//  Defines a gameobject that plugs in additional game functionality on collision
//  and optionally rejecting functionality after a time period
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
    class Augment : GameObject
    {
        // fields
        private Action apply;
        private Action reject;
        private int length;
        private bool rejectOnDeath;

        // constructor
        public Augment(Image texture, Rectangle srcRect, Action apply, Action reject = null, int length = -1, bool rejectOnDeath = true)
            : base (0,0, texture, srcRect, ghost:false)
        {
            // initalize fields
            this.apply          = apply;
            this.reject         = (reject is null) ? () => { } : reject;
            this.length         = length;
            this.rejectOnDeath  = rejectOnDeath;

            // set velocity
            Velocity = new Vector2D(0, 5);
        }

        // called to apply the augment
        public Action Apply 
        { 
            get => apply; 
            set => apply = value; 
        }

        // called to reject the given augment
        public Action Reject 
        { 
            get => reject; 
            set => reject = value; 
        }

        // called when the object is collided with
        public override void OnCollsion(GameObject collider)
        {
            // apply the augmentation
            apply();

            // after <length> reject the applied augment
            if (length > 0)
                Game.doAfter(length, reject);
        }
    }
}
