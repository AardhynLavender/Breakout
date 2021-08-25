
//
//  Augment Class
//
//  Defines a gameobject that plugs in additional game functionality on collision
//  and optionally rejecting functionality in multiple ways
//      - provided Func<bool> delegate returns true
//      - provided time period passes.
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
        private Func<bool> condition;
        private Action reject;
        private int length;
        private bool rejectOnDeath;

        // constructor
        public Augment(Image texture, Rectangle srcRect, Action apply, Action reject = null, Func<bool> condition = null, int length = -1, bool rejectOnDeath = true)
            : base (0,0, texture, srcRect, ghost:false)
        {
            // initalize fields
            this.apply          = apply;
            this.reject         = (reject is null) ? () => { } : reject;
            this.condition      = (condition is null) ? () => false : condition;
            this.length         = length;
            this.rejectOnDeath  = rejectOnDeath;

            // set velocity
            Velocity = new Vector2D(0, 2);
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

        public override void OnCollsion(GameObject collider)
        {
            // apply the augmentation
            apply();

            // after <length> reject the applied augment
            if (length > 0)
                Game.doAfter(length, reject);
        }

        public override void Update()
        {
            if (condition()) reject();
        }
    }
}
