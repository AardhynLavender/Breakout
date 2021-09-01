
//
//  GameComponant Class
//
//  Defines
//

using Breakout.Render;

namespace Breakout.Utility
{
    abstract class GameComponant
    {
        // all game componants need to know what game they're a part of
        public static BreakoutGame BreakoutGame;

        // all game componatns need to know where to draw themselves
        public static Screen Screen;
    }
}
