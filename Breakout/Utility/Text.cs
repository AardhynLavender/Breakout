
//
//  Text Class
//
//  Manages a group of game objects acting as characters, allowing text to be rendered to the screen
//  from a provided typeset with a character map and a provided text input.
//

using System;
using System.Collections.Generic;
using System.Linq;

using Breakout.GameObjects;
using Breakout.Render;

namespace Breakout.Utility
{
    class Text : GameObject
    {
        // Constants
        private const int Z_INDEX           = 99;
        private const int CHARACTER_WIDTH   = 6;
        private const int CHARACTER_HEIGHT  = 5;
        private const int LINE_SPACING      = 3;

        private const char SPACE            = ' ';
        private const string MAP            = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.,";

        // create a tileset from the typeset asset
        private static Tileset typeset = new Tileset(
            Properties.Resources.typeset, CHARACTER_WIDTH * 10, CHARACTER_WIDTH, CHARACTER_HEIGHT
        );

        // fields 
        private List<GameObject> characters;
        private string text;
        private int widthCharacters;

        // offset the character objects from the Text objects origin
        protected int offsetX;
        protected int offsetY;

        public Text(float x, float y, string text = "", int widthCharacters = 100)
            : base(x, y)
        {
            // initalize fields
            this.x = x;
            this.y = y;
            this.text = text;
            this.widthCharacters = widthCharacters;

            // calculate width
            width = CHARACTER_WIDTH * text.Length;
            height = CHARACTER_HEIGHT;

            // initalize offset
            offsetX = offsetY = 0;

            // inialize charactesr list
            characters = new List<GameObject>(text.Length);
        }

        // Properties

        // returns the map constant
        public static string Map
        {
            get => MAP;
        }

        // the individual characters
        public List<GameObject> Characters
        {
            get => characters;
            set => characters = value;
        }

        // Text value of the text box
        // updates the text when edited
        public string Value
        {
            get => text;
            set 
            {
                text = value;
                updateText();
            }
        }

        // formats text so that no words are orphaned
        private string format(string text)
        {
            string formatedString = string.Empty;

            // are there more than one word
            if (text.Contains(SPACE))
            {
                // loop each word
                foreach (string word in text.Split(SPACE))
                {
                    // get infomation about the word
                    int positon = formatedString.Length % widthCharacters;
                    int remainingSpace = widthCharacters - positon;

                    // is there a newline command
                    if (word.Contains("$NL"))
                    {
                        // fill remaining line space with SPACE
                        formatedString += new string(SPACE, remainingSpace);
                    }
                    else
                    {
                        // add new word if its fits
                        // else fill remaining line with SPACE
                        formatedString += (positon + word.Length + 1 > widthCharacters)
                            ? new string(SPACE, remainingSpace) + SPACE + word
                            : SPACE + word;
                    }
                }
            }
            else formatedString = text;

            // return formated string uppercase without trailing and leading spaces
            return formatedString.Trim().ToUpper();
        }

        // override base drawing functionality
        public override void Draw()
        {  }

        // updates the text displayed
        private void updateText()
        {
            deleteCharacters();

            // format text
            string formatedText = format(text);

            // loop characters
            for (int i = 0; i < formatedText.Length; i++)
            {
                // calculate line
                int line = (int)Math.Floor((float)i / widthCharacters);

                // calcualte characters position
                float charX = x + offsetX + CHARACTER_WIDTH * (i % widthCharacters);
                float charY = y + offsetY + line * CHARACTER_HEIGHT + (LINE_SPACING * line);

                // add new game object with appropriate texture base on MAP
                characters.Add(new GameObject(
                    charX,
                    charY,
                    typeset.Texture,
                    typeset.GetTile(MAP.IndexOf(formatedText[i].ToString())),
                    z: Z_INDEX,
                    ghost: true
                ));

                // give character its parents velocity
                characters.Last().Velocity = Velocity;
            }

            // add game charactes to game
            addCharacters();
        }

        // update text when this is added
        public override void OnAddGameObject()
            => updateText();

        // delete characters when this is deleted
        public override void OnFreeGameObject()
            => deleteCharacters();

        // frees all associated character game objects
        private void deleteCharacters()
        {
            characters.ForEach(c => BreakoutGame.QueueFree(c));
            characters.Clear();
        }

        // adds the character game objects to the game
        private void addCharacters()
            => characters.ForEach(c => BreakoutGame.AddGameObject(c));
    }
}
