using System.Collections;
namespace Data.Voxel {
    public class Voxel {
        private byte _type;

        public byte Type {
            get { return _type; }
            set { _type = value; }
        }

        public Voxel(byte type) {
            _type = type;
        }

    }
}