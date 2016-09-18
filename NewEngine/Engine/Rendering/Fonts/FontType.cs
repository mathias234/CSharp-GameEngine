using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Rendering.Fonts {
    public class FontType {
        private int _textureAtlas;
        private TextMeshCreator _loader;

        public FontType(int textureAtlas, string filename) {
            _textureAtlas = textureAtlas;
            _loader = new TextMeshCreator(filename);
        }

        public int TextureAtlas => _textureAtlas;


        public TextMeshData LoadText(GUIText text) {
            return _loader.CreateTextMesh(text);
        }
    }
}
