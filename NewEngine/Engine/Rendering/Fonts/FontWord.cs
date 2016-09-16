using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Rendering.Fonts {
    public class FontWord {
        private List<FontCharacter> _characters = new List<FontCharacter>();
        private double _width;
        private double _fontSize;

        public FontWord(double fontSize) {
            _fontSize = fontSize;
        }

        public void AddCharacter(FontCharacter character) {
            _characters.Add(character);
            _width += character.XAdvance*_fontSize;
        }

        public List<FontCharacter> Characters => _characters;

        public double WordWidth => _width;
    }
}
