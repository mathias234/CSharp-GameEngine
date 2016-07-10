using System.Collections;
using Data.Voxel.Map;

namespace Data.Voxel {
    public class Voxel {
        private BlockTypes _type;
        private float _mass;

        public BlockTypes Type {
            get { return _type; }
            set { _type = value; }
        }

        public float Mass {
            get { return _mass; }
            set { _mass = value; }
        }

        public Voxel(BlockTypes type) {
            _type = type;
        }

    }
}