using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class BatchMeshRenderer {
        private Material _material;

        private Dictionary<Mesh, List<GameObject>> meshGameObjects = new Dictionary<Mesh, List<GameObject>>();
        private Dictionary<string, Shader> _loadedShaders = new Dictionary<string, Shader>();


        private Shader _baseShader;

        public BatchMeshRenderer(Material material, Mesh meshes, GameObject gameObjects) {
            _material = material;

            meshGameObjects.Add(meshes, new[] { gameObjects }.ToList());

            _baseShader = new Shader("batching/forward-batched-ambient");

            Initialize();
        }

        public void Initialize() {

        }

        public void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            Shader shaderToUse;

            if (shaderType == "base") {
                shaderToUse = _baseShader;
            }
            else if (shaderType != "light" && shaderType != "shadowMap") {
                return;
            }
            else if (_loadedShaders.ContainsKey(shader)) {
                shaderToUse = _loadedShaders[shader];
            }
            else {
                shaderToUse = new Shader("batching/forward-batched-" + shader);
                _loadedShaders.Add(shader, shaderToUse);
            }

            foreach (var mesh in meshGameObjects) {
                List<Matrix4> matrices = new List<Matrix4>();

                foreach (var gameObject in mesh.Value) {
                    matrices.Add(gameObject.Transform.GetTransformation());
                }
                mesh.Key.BindBatch(matrices.ToArray(), mesh.Value.Count);
            }

            shaderToUse.Bind();
            shaderToUse.UpdateUniforms(meshGameObjects.ElementAt(0).Value[0].Transform, _material, renderingEngine);

            foreach (var mesh in meshGameObjects) {
                mesh.Key.DrawInstanced(mesh.Value.Count);
            }
        }

        public void AddGameObject(Mesh mesh, GameObject gObj) {
            if (meshGameObjects.ContainsKey(mesh)) {
                meshGameObjects[mesh].Add(gObj);
            }
            else {
                meshGameObjects.Add(mesh, new[] { gObj }.ToList());
            }

            Initialize();
        }
    }
}
