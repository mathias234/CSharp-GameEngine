using System;

namespace NewEngine.Engine.Rendering.MeshLoading.Obj {
    public class ObjIndex : IEquatable<ObjIndex> {
        public int VertexIndex { get; set; }

        public int TexCoordIndex { get; set; }

        public int NormalIndex { get; set; }

        public bool Equals(ObjIndex index) {
            return VertexIndex == index.VertexIndex &&
                   TexCoordIndex == index.TexCoordIndex &&
                   NormalIndex == index.NormalIndex;
        }

        public override int GetHashCode() {
            var BASE = 17;
            var MULTIPLIER = 31;

            var result = BASE;

            result = MULTIPLIER*result + VertexIndex;
            result = MULTIPLIER*result + TexCoordIndex;
            result = MULTIPLIER*result + NormalIndex;

            return result;
        }
    }
}