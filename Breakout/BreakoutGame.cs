using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.Render;

namespace Breakout
{
    class BreakoutGame : Game
    {

        public BreakoutGame(Screen screen) 
            : base(screen)
        {
            
        }

        public override void EndGame()
        {
            
        }

        public override void GameLoop()
        {
            Update();
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
