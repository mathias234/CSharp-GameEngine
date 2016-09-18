using System;
using System.Collections.Generic;
using NewEngine.Engine.Core;

namespace NewEngine.Engine.Rendering.Fonts {
    public class TextMeshCreator {
        public const double LineHeight = 0.03f;
        public const int SpaceAscii = 32;

        private FontMetaFile _metaData;

        public TextMeshCreator(string metaFile) {
            _metaData = new FontMetaFile(metaFile);
        }

        public TextMeshData CreateTextMesh(GUIText text) {
            var lines = CreateStructure(text);
            var data = CreateQuadVertices(text, lines);
            return data;
        }

        private List<FontLine> CreateStructure(GUIText text) {
            var chars = text.TextString.ToCharArray();
            var lines = new List<FontLine>();

            var currentLine = new FontLine(_metaData.SpaceWidth, text.FontSize, text.MaxLineSize);
            var currentWord = new FontWord(text.FontSize);
            foreach (var c in chars) {
                int ascii = c;
                if (ascii == SpaceAscii) {
                    var added = currentLine.AttemptToAddWord(currentWord);
                    if (!added) {
                        lines.Add(currentLine);
                        currentLine = new FontLine(_metaData.SpaceWidth, text.FontSize, text.MaxLineSize);
                        currentLine.AttemptToAddWord(currentWord);
                    }
                    currentWord = new FontWord(text.FontSize);
                    continue;
                }
                var character = _metaData.GetCharacter(ascii);
                currentWord.AddCharacter(character);
            }
            CompleteStructure(lines, currentLine, currentWord, text);
            return lines;
        }

        private void CompleteStructure(List<FontLine> lines, FontLine currentLine, FontWord currentWord, GUIText text) {
            if (lines == null) LogManager.Error(new ArgumentNullException(nameof(lines)).Message);

            var added = currentLine.AttemptToAddWord(currentWord);
            if (!added) {
                lines?.Add(currentLine);
                currentLine = new FontLine(_metaData.SpaceWidth, text.FontSize, text.MaxLineSize);
                currentLine.AttemptToAddWord(currentWord);
            }
            lines?.Add(currentLine);
        }

        private TextMeshData CreateQuadVertices(GUIText text, List<FontLine> lines) {
            if (lines == null) LogManager.Error(new ArgumentNullException(nameof(lines)).Message);

            text.NumberOfLines = lines.Count;
            double cursorX = 0f;
            double cursorY = 0f;
            var vertices = new List<float>();
            var textureCoords = new List<float>();
            foreach (var line in lines) {
                if (text.IsCentered) {
                    cursorX = (line.MaxLength - line.LineLength) / 2;
                }
                foreach (var Word in line.Words) {
                    foreach (var letter in Word.Characters) {
                        AddVerticesForCharacter(cursorX, cursorY, letter, text.FontSize, vertices);
                        AddTexCoords(textureCoords, letter.XTextureCoord, letter.YTextureCoord, letter.XMaxTextureCoord,
                            letter.YMaxTextureCoord);
                        cursorX += letter.XAdvance * text.FontSize;
                    }
                    cursorX += _metaData.SpaceWidth * text.FontSize;
                }
                cursorX = 0;
                cursorY += LineHeight * text.FontSize;
            }

            return new TextMeshData(vertices.ToArray(), textureCoords.ToArray());
        }


        private static void AddVerticesForCharacter(double curserX, double curserY, FontCharacter character, double fontSize,
                ICollection<float> vertices) {
            var x = curserX + character.XOffset * fontSize;
            var y = curserY + character.YOffset * fontSize;
            var maxX = x + character.SizeX * fontSize;
            var maxY = y + character.SizeY * fontSize;
            var properX = 2 * x - 1;
            var properY = -2 * y + 1;
            var properMaxX = 2 * maxX - 1;
            var properMaxY = -2 * maxY + 1;
            AddVertices(vertices, properX, properY, properMaxX, properMaxY);
        }

        private static void AddVertices(ICollection<float> vertices, double x, double y, double maxX, double maxY) {
            vertices.Add((float)x);
            vertices.Add((float)y);
            vertices.Add((float)x);
            vertices.Add((float)maxY);
            vertices.Add((float)maxX);
            vertices.Add((float)maxY);
            vertices.Add((float)maxX);
            vertices.Add((float)maxY);
            vertices.Add((float)maxX);
            vertices.Add((float)y);
            vertices.Add((float)x);
            vertices.Add((float)y);
        }

        private static void AddTexCoords(ICollection<float> texCoords, double x, double y, double maxX, double maxY) {
            texCoords.Add((float)x);
            texCoords.Add((float)y);
            texCoords.Add((float)x);
            texCoords.Add((float)maxY);
            texCoords.Add((float)maxX);
            texCoords.Add((float)maxY);
            texCoords.Add((float)maxX);
            texCoords.Add((float)maxY);
            texCoords.Add((float)maxX);
            texCoords.Add((float)y);
            texCoords.Add((float)x);
            texCoords.Add((float)y);
        }
    }
}
