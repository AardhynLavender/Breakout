
//
//  GameComponant Class
//
//  Defines an object that knows its Game, Screen, and Random.
//

using Breakout.Render;
using System;

namespace Breakout.Utility
{
    abstract class GameComponant
    {
        // all game components need to know what game they're a component of
        public static BreakoutGame BreakoutGame;

        // all game components need to know where to draw themselves
        public static Screen Screen;

        // all game components have access to a single random object
        public static Random Random;
    }
}
