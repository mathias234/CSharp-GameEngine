using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Rendering.Fonts {
    public class FontType {
        private Texture _textureAtlas;
        private TextMeshCreator _loader;

        public FontType(Texture textureAtlas, string filename) {
            _textureAtlas = textureAtlas;
            _loader = new TextMeshCreator(filename);
        }

        public Texture TextureAtlas => _textureAtlas;


        public TextMeshData LoadText(GUIText text) {
            return _loader.CreateTextMesh(text);
        }
    }
}
