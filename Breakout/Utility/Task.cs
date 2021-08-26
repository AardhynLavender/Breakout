
//
//  Task Class
//  
//  Defines a simple object to delay a function being
//  called for a specifed amount of game loops 
//
//  This avoids the need to use threading for asynchronous 
//  function callbacks and ensures the method is called at
//  the end of the game loop rather than loosing control
//  of execution
//

using System;

namespace Breakout.Utility
{
    class Task
    {
        int sleepFor;
        bool called;
        Action callback;

        public Task(Action callback, int milliseconds)
        {
            sleepFor = milliseconds / Game.TickRate;
            called = false;
            this.callback = callback;
        }

        public bool Called => called;

        public bool TryRun()
        {
            if (--sleepFor < 1)
            {
                callback();
                called = true;
            }

            return called;
        }
    }
}
