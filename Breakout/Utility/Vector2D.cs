
//
//  Vector2D Structure
//
//  a data structure to define a vector in 2D space. providing
//  methods to zero, invert, and calculate distances.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Utility
{
    struct Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Zero() 
            => X = Y = 0;
        
        public void Invert()
        {
            float temp = X;
            X = Y;
            Y = temp;
        }

        public static void GetDistance(Vector2D a, Vector2D b)
            => new Vector2D(a.X - b.X, a.Y - b.Y);
    }
}
