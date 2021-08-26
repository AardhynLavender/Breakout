
//
//  Augment Class
//
//  Defines a gameobject that plugs in additional game functionality on collision
//  and rejecting such functionality after either a timeout or condition is met
//

using System.Drawing;

using Breakout.Utility;

namespace Breakout.GameObjects
{
    abstract class Augment : GameObject
    {
        // fields
        protected BreakoutGame breakout;
        protected int length;
        protected int amount;
        protected bool rejectOnDeath;

        // constructor
        public Augment(BreakoutGame breakout, Image texture, Rectangle srcRect, int length = -1, bool rejectOnDeath = true)
            : base (0,0, texture, srcRect, ghost:false)
        {
            // initalize fields
            this.breakout       = breakout;
            this.length         = length;
            this.rejectOnDeath  = rejectOnDeath;
            Velocity            = new Vector2D(0, 2);
        }

        // Properties
        public int Amount => amount;

        // abstract and virtual members
        protected abstract void apply();
        protected abstract void reject();
        protected virtual bool condition() => false;

        public override void OnCollsion(GameObject collider)
        {
            // apply the augmentation
            apply();

            // after <length> reject the applied augment
            if (length > 0)
                breakout.QueueTask(length, reject);
        }

        public override void Update()
        {
            // reject the augment if condition is met
            if (condition()) reject();
        }
    }
}
