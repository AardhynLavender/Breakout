
//
//  Text Class
//
//  Manages a group of game objects acting as characters allowing text to be rendered to the screen
//  from a provided typeset and character map
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Breakout.GameObjects;
using Breakout.Render;

namespace Breakout.Utility
{
    class Text
    {
        // typeface tileset
        private const int CHARACTER_WIDTH   = 6;
        private const int CHARACTER_HEIGHT  = 5;
        private const string map            = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.,";

        private static Tileset typeset = new Tileset(
            Properties.Resources.typeset, CHARACTER_WIDTH * 10, CHARACTER_WIDTH, CHARACTER_HEIGHT
        );

        // fields 
        private List<GameObject> characters;
        private float x;
        private float y;
        private int width;
        private int height;
        private string text;

        public Text(float x, float y, string text = "")
        {
            // initalize fields
            this.x      = x;
            this.y      = y;
            this.text   = text;

            width       = CHARACTER_WIDTH * text.Length;
            height      = CHARACTER_HEIGHT;

            characters  = new List<GameObject>(text.Length);
        }

        // Properties

        public static string Map
        {
            get => map;
        }

        public List<GameObject> Characters
        {
            get => characters;
            set => characters = value;
        }

        public string Value
        {
            get => text;
            set => text = value;
        }

        public bool Empty
        {
            get => characters.Count == 0;
        }

        public float X
        {
            get => x;
            set => x = value;
        }

        public float Y
        {
            get => y;
            set => y = value;
        }

        public int Width
        {
            get => width;
            set => width = value;
        }

        public int Height
        {
            get => height;
            set => height = value;
        }

        // Methods

        public void Update()
        {
            characters.Clear();
            for (int i = 0; i < text.Length; i++)
            {
                characters.Add(new GameObject(
                    x + CHARACTER_WIDTH * i,
                    y, typeset.Texture,
                    typeset.GetTile(map.IndexOf(text[i].ToString())),
                    ghost: true
                ));
            }
        }

        public void Clear()
        {
            characters.Clear();
            Value = string.Empty;
        }
    }
}
