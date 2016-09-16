using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Core;

namespace NewEngine.Engine.Rendering.Fonts {
    public class FontMetaFile {
        private const int PadTop = 0;
        private const int PadLeft = 1;
        private const int PadBottom = 2;
        private const int PadRight = 3;

        private const int DesiredPadding = 3;

        private const char Splitter = ' ';
        private const char NumberSeperator = ',';

        private double _aspectRatio;

        private double _verticalPerPixelSize;
        private double _horizontalPerPixelSize;
        private double _spaceWidth;

        private int[] _padding;
        private int _paddingWidth;
        private int _paddingHeight;

        private Dictionary<int, FontCharacter> _metaData = new Dictionary<int, FontCharacter>();

        private TextReader _reader;
        private Dictionary<string, string> _values = new Dictionary<string, string>();

        public FontMetaFile(string filename) {
            _aspectRatio = (double) CoreEngine.GetWidth()/CoreEngine.GetHeight();
            OpenFile(filename);
            LoadPaddingData();
            LoadLineSizes();
            int imageWidth = GetValueOfVariable("scaleW");
            LoadCharacterData(imageWidth);
            Close();
        }

        public double SpaceWidth => _spaceWidth;

        public FontCharacter GetCharacter(int ascii) {
            return _metaData[ascii];
        }

        private bool ProcessNextLine() {
            _values.Clear();
            string line = null;
            try {
                line = _reader.ReadLine();
            }
            catch (Exception) {
                // ignored
            }
            if(line == null)
                return false;

            foreach (var part in line.Split(Splitter)) {
                var valueParis = part.Split('=');
                if (valueParis.Length == 2) {
                    ValuesPut(valueParis[0], valueParis[1]);
                }
            }
            return true;
        }

        private int GetValueOfVariable(string variable) {
            return int.Parse(_values[variable]);
        }

        private int[] GetValuesOfVariable(string variable) {
            var numbers = _values[variable].Split(NumberSeperator);
            var actualValues = new int[numbers.Length];
            for (var i = 0; i < actualValues.Length; i++) {
                actualValues[i] = int.Parse(numbers[i]);
            }

            return actualValues;
        }

        private void Close() {
            _reader.Close();
        }

        private void OpenFile(string filename) {
            try {
                _reader = new StreamReader(File.Open(filename, FileMode.Open));
            }
            catch (Exception ex) {
                LogManager.Error(ex.Message);
            }
        }

        private void LoadPaddingData() {
            ProcessNextLine();
            _padding = GetValuesOfVariable("padding");
            _paddingWidth = _padding[PadLeft] + _padding[PadRight];
            _paddingHeight = _padding[PadTop] + _padding[PadBottom];
        }

        private void LoadLineSizes() {
            ProcessNextLine();
            var lineHeightPixels = GetValueOfVariable("lineHeight") - _paddingHeight;
            _verticalPerPixelSize = TextMeshCreator.LineHeight/(double) lineHeightPixels;
            _horizontalPerPixelSize = _verticalPerPixelSize / _aspectRatio;

        }

        private void LoadCharacterData(int imageWidth) {
            ProcessNextLine();
            ProcessNextLine();
            while (ProcessNextLine()) {
                var c = LoadCharacter(imageWidth);
                if(c != null)
                    MetaDataPut(c.Id, c);
            }
        }

        private FontCharacter LoadCharacter(int imageSize) {
            var id = GetValueOfVariable("id");
            if (id == TextMeshCreator.SpaceAscii) {
                _spaceWidth = (GetValueOfVariable("xadvance") - _paddingWidth) * _horizontalPerPixelSize;
                return null;
            }

            var xTex = ((double)GetValueOfVariable("x") + (_padding[PadLeft] - DesiredPadding)) / imageSize;
            var yTex = ((double)GetValueOfVariable("y") + (_padding[PadTop] - DesiredPadding)) / imageSize;
            var width = GetValueOfVariable("width") - (_paddingWidth - (2 * DesiredPadding));
            var height = GetValueOfVariable("height") - ((_paddingHeight) - (2 * DesiredPadding));
            var quadWidth = width * _horizontalPerPixelSize;
            var quadHeight = height * _verticalPerPixelSize;
            var xTexSize = (double)width / imageSize;
            var yTexSize = (double)height / imageSize;
            var xOff = (GetValueOfVariable("xoffset") + _padding[PadLeft] - DesiredPadding) * _horizontalPerPixelSize;
            var yOff = (GetValueOfVariable("yoffset") + (_padding[PadTop] - DesiredPadding)) * _verticalPerPixelSize;
            var xAdvance = (GetValueOfVariable("xadvance") - _paddingWidth) * _horizontalPerPixelSize;
            return new FontCharacter(id, xTex, yTex, xTexSize, yTexSize, xOff, yOff, quadWidth, quadHeight, xAdvance);
        }

        private void ValuesPut(string key, string value) {
            if (_values.ContainsKey(key))
                _values[key] = value;
            else {
                _values.Add(key, value);
            }
        }


        private void MetaDataPut(int key, FontCharacter value) {
            if (_metaData.ContainsKey(key))
                _metaData[key] = value;
            else {
                _metaData.Add(key, value);
            }
        }
    }
}
