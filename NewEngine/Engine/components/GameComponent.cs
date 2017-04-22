﻿using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;

namespace NewEngine.Engine.components {
    public abstract class GameComponent {
        public bool IsEnabled;

        public Transform Transform => gameObject.Transform;

        public GameObject gameObject;

        public virtual void Update(float deltaTime) {}

        public virtual void Render(string shader, string shaderType, float deltaTime, BaseRenderingEngine renderingEngine, string renderStage) {}

        public virtual void OnEnable() {}

        public virtual void AddToEngine(ICoreEngine engine) {}

        public virtual void OnDestroyed(ICoreEngine engine) {}
    }
}