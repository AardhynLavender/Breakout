
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
        private const int SCALE = 4;

        private Tileset assets;

        public BreakoutGame(Screen screen) 
            : base(screen)
        {
            screen.Scale = SCALE;
        }

        public override void GameLoop()
        {
            Physics();
            Render();
            tick++;
        }

        public override void Physics()
        {
            
        }

        public override void Render()
            => base.Render();


        public override void SaveGame()
        {
            // save persistant data (high score, level?)...
        }

        public override void StartGame()
        {
            // start game logic...
        }

        public override void EndGame()
        {
            // clean up..
        }
    }
}
