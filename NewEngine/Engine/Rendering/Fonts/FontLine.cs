using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Rendering.Fonts {
    public class FontLine {
        private double _maxLength;
        private double _spaceSize;

        private List<FontWord> _words = new List<FontWord>();
        private double _currentLineLength;

        public FontLine(double spaceWidth, double fontSize, double maxLength) {
            _spaceSize = spaceWidth*fontSize;
            _maxLength = maxLength;
        }

        public bool AttemptToAddWord(FontWord word) {
            var additionalLength = word.WordWidth;
            additionalLength += _words.Count > 0 ? _spaceSize : 0;

            if (!(_currentLineLength + additionalLength <= _maxLength)) return false;

            _words.Add(word);
            _currentLineLength += additionalLength;
            return true;
        }

        public double MaxLength => _maxLength;

        public double LineLength => _currentLineLength;

        public List<FontWord> Words => _words;
    }
}
