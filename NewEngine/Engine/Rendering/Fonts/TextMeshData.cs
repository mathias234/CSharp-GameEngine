namespace NewEngine.Engine.Rendering.Fonts {
    public class TextMeshData {
        private float[] _vertexPositions;
        private float[] _textureCoords;

        public TextMeshData(float[] vertexPositions, float[] textureCoords) {
            _vertexPositions = vertexPositions;
            _textureCoords = textureCoords;
        }

        public float[] VertexPositions => _vertexPositions;

        public float[] TextureCoords => _textureCoords;

        public int VertexCount => _vertexPositions.Length / 2;
    }
}
