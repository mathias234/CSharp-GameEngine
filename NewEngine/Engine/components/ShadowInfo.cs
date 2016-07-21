using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace NewEngine.Engine.components {
    public class ShadowInfo {
        private Matrix4 _projection;
        private float _bias;
        private bool _flipFaces;

        public ShadowInfo(Matrix4 projection, float bias, bool flipFaces) {
            _projection = projection;
            _bias = bias;
            _flipFaces = flipFaces;
        }

        public Matrix4 Projection {
            get { return _projection; }
            set { _projection = value; }
        }

        public bool FlipFaces {
            get { return _flipFaces; }
        }

        public float Bias {
            get { return _bias; }
        }
    }
}
