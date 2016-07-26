using OpenTK;

namespace NewEngine.Engine.Rendering {
    public static class PrimitiveObjects {
        /// <summary>
        ///     Creates a 2x2 plane
        /// </summary>
        public static Mesh CreatePlane => Plane();

        private static Mesh Plane() {
            Vertex[] vertices = {
                new Vertex(new Vector3(-1, -1, 0), new Vector2(1, 0)),
                new Vertex(new Vector3(-1, 1, 0), new Vector2(1, 1)),
                new Vertex(new Vector3(1, 1, 0), new Vector2(0, 1)),
                new Vertex(new Vector3(1, -1, 0), new Vector2(0, 0))
            };

            int[] indices = {
                2, 0, 1,
                3, 0, 2
            };

            return new Mesh(vertices, indices, true);
        }
    }
}