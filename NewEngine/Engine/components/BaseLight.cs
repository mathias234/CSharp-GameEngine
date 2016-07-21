﻿using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class BaseLight : GameComponent {
        private Vector3 _color;
        private float _intensity;
        private Shader _shader;
        private ShadowInfo _shadowInfo;

        public BaseLight(Vector3 color, float intensity) {
            this._intensity = intensity;
            this._color = color;
            _shadowInfo = null;
        }

        public override void AddToEngine(CoreEngine engine) {
            engine.RenderingEngine.AddLight(this);
        }

        public float Intensity {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public Vector3 Color {
            get { return _color; }
            set { _color = value; }
        }

        public Shader Shader {
            get { return _shader; }
            set { _shader = value; }
        }

        public ShadowInfo ShadowInfo {
            get { return _shadowInfo; }
            set { _shadowInfo = value; }
        }
    }
}
