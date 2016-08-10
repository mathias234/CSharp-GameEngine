using System;
using System.Collections.Generic;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.components {
    public class WaterMesh : GameComponent {
        private Shader _baseShader;
        private Mesh _waterMesh;
        private Material _material;
        private float _waveSpeed;

        public WaterMesh(int width, int height, float waveSpeed = 0.01f, float waveStrength = 0.02f, float refractivePower = 0.5f, float dudvTiling = 6) {
            _waveSpeed = waveSpeed;
            _baseShader = new Shader("water/baseWater");

            Vertex[] vertices = {
                new Vertex(new Vector3(0, 0, 0), new Vector2(1, 0)),
                new Vertex(new Vector3(0, 0, height), new Vector2(1, 1)),
                new Vertex(new Vector3(width, 0, height), new Vector2(0, 1)),
                new Vertex(new Vector3(width, 0, 0), new Vector2(0, 0))
            };

            int[] indices = {
                2, 0, 1,
                3, 0, 2
            };

            _waterMesh = new Mesh(vertices, indices, true);

            _material = new Material(new Texture("dudvMap.png"), 3, 32, new Texture("matchingNormalMap.png"));
            _material.SetFloat("moveFactor", 0);
            _material.SetFloat("waveStrength", waveStrength);
            _material.SetFloat("refractivePower", refractivePower);
            _material.SetFloat("tiling", dudvTiling);
        }

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            if (renderStage != "water")
                return;

            Shader shaderToUse;

                shaderToUse = _baseShader;

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                _material.SetFloat("moveFactor", _material.GetFloat("moveFactor") + _waveSpeed * deltaTime);

                shaderToUse.Bind();
                shaderToUse.UpdateUniforms(Parent.Transform, _material, renderingEngine);
                _waterMesh.Draw();

                GL.Disable(EnableCap.Blend);
        }
    }
}