
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
    class Text : GameObject
    {
        // typeface tileset
        private const int CHARACTER_WIDTH   = 6;
        private const int CHARACTER_HEIGHT  = 5;
        private const int LINE_SPACING      = 1;
        private const char SPACE            = ' ';
        private const string map            = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.,";

        private static Tileset typeset = new Tileset(
            Properties.Resources.typeset, CHARACTER_WIDTH * 10, CHARACTER_WIDTH, CHARACTER_HEIGHT
        );

        // fields 
        private List<GameObject> characters;

        private string text;
        private int widthCharacters;

        public Text(float x, float y, string text = "", int widthCharacters = 100)
            : base(x, y)
        {
            // initalize fields
            this.x = x;
            this.y = y;
            this.text = text;
            this.widthCharacters = widthCharacters;

            width = CHARACTER_WIDTH * text.Length;
            height = CHARACTER_HEIGHT;

            characters = new List<GameObject>(text.Length);
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

        // Methods

        // formats text so that no words are orphaned
        private string format(string text)
        {
            string formatedString = string.Empty;

            Console.WriteLine(text);

            if (text.Contains(SPACE))
            {
                foreach (string word in text.Split(SPACE))
                {
                    int positon = formatedString.Length % widthCharacters;
                    int remainingSpace = widthCharacters - positon;

                    formatedString += (positon + word.Length + 1 > widthCharacters)
                        ? new string(SPACE , remainingSpace) + SPACE + word
                        : SPACE + word;
                }
            }
            else formatedString = text;

            return formatedString.Trim();
        }

        public override void Draw()
        {
            
        }

        public new void Update()
        {
            characters.Clear();

            string formatedText = format(text);

            for (int i = 0; i < formatedText.Length; i++)
            {
                int line = (int)Math.Floor((float)i / widthCharacters);

                characters.Add(new GameObject(
                    x + CHARACTER_WIDTH * (i % widthCharacters),
                    y + line * CHARACTER_HEIGHT + (LINE_SPACING * line),
                    typeset.Texture,
                    typeset.GetTile(map.IndexOf(formatedText[i].ToString())),
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
