using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Rendering {
    public class Attenuation {
        private float _constant;
        private float _linear;
        private float _exponent;
        public Attenuation(float constant, float linear, float exponent) {
            _constant = constant;
            _linear = linear;
            _exponent = exponent;
        }

        public float Constant {
            get { return _constant; }
            set { _constant = value; }
        }

        public float Linear {
            get { return _linear; }
            set { _linear = value; }
        }

        public float Exponent {
            get { return _exponent; }
            set { _exponent = value; }
        }
    }
}
