
//
//  BreakOutGame:Game class
//
//  Defines the functionality and members to create and play Atari Breakout
//  with score counters, powerups, levels, and saving.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.Render;
using Breakout.GameObjects;

namespace Breakout
{
    class BreakoutGame : Game
    {
        const int SCALE = 4;

        public BreakoutGame(Screen screen) 
            : base(screen)
        {
            screen.Scale = SCALE;

            AddGameObject(new GameObject(0, 0, Properties.Resources.dirt));
        }

        public override void EndGame()
        {
            
        }

        public override void GameLoop()
        {
            Physics();
            Render();
            tick++;
        }

        public override void SaveGame()
        {
            
        }

        public override void StartGame()
        {
            
        }
    }
}
