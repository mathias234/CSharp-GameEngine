using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Rendering.MeshLoading {
    public class ObjIndex : IEquatable<ObjIndex> {
        public int vertexIndex;
        public int texCoordIndex;
        public int normalIndex;

        public bool Equals(ObjIndex index) {
            return vertexIndex == index.vertexIndex &&
                   texCoordIndex == index.texCoordIndex &&
                   normalIndex == index.normalIndex;
        }

        public override int GetHashCode() {
            int BASE = 17;
            int MULTIPLIER = 31;

            int result = BASE;

            result = MULTIPLIER * result + vertexIndex;
            result = MULTIPLIER * result + texCoordIndex;
            result = MULTIPLIER * result + normalIndex;

            return result;
        }
    }
}
