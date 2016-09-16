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

        private Dictionary<Mesh, List<GameObject>> _meshGameObjects = new Dictionary<Mesh, List<GameObject>>();
        private Dictionary<string, Shader> _loadedShaders = new Dictionary<string, Shader>();

        private Matrix4[] _matrices;


        public BatchMeshRenderer(Material material, Mesh meshes, GameObject gameObjects) {
            _material = material;

            _meshGameObjects.Add(meshes, new[] { gameObjects }.ToList());

            Initialize();
        }

        public void Initialize() {

        }

        public void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            if(!_material.Shader.GetShaderTypes.Contains(shaderType))
                return;

            // with alot of objects this will put a strain on the garbage collector, maybe find a fix
            for (int j = 0; j < _meshGameObjects.Count; j++) {
                _matrices = new Matrix4[_meshGameObjects.ElementAt(j).Value.Count];

                for (int i = 0; i < _matrices.Length; i++) {
                    _matrices[i] = _meshGameObjects.ElementAt(j).Value[i].Transform.GetTransformation();

                }
                _meshGameObjects.ElementAt(j).Key.BindBatch(_matrices, _meshGameObjects.ElementAt(j).Value.Count);
            }

            _material.Shader.Bind(shaderType);
            _material.Shader.UpdateUniforms(_meshGameObjects.ElementAt(0).Value[0].Transform, _material, renderingEngine, shaderType);

            foreach (var mesh in _meshGameObjects) {
                mesh.Key.DrawInstanced(mesh.Value.Count);
            }
        }

        public void AddGameObject(Mesh mesh, GameObject gObj) {
            if (_meshGameObjects.ContainsKey(mesh)) {
                _meshGameObjects[mesh].Add(gObj);
            }
            else {
                _meshGameObjects.Add(mesh, new[] { gObj }.ToList());
            }

            Initialize();
        }

        public int RemoveGameObject(Mesh mesh, GameObject gameObject) {
            if (_meshGameObjects.ContainsKey(mesh)) {
                if (_meshGameObjects[mesh].Contains(gameObject)) {
                    _meshGameObjects[mesh].Remove(gameObject);
                }
                if (_meshGameObjects[mesh].Count == 0)
                    _meshGameObjects.Remove(mesh);
            }

            return _meshGameObjects.Count;
        }

        public void UpdateObject(Mesh mesh, GameObject gameObject) {
            if (_meshGameObjects.ContainsKey(mesh)) {
                if (_meshGameObjects[mesh].Contains(gameObject)) {
                    _meshGameObjects[mesh].Remove(gameObject);
                    AddGameObject(mesh, gameObject);
                }
            }
        }
    }
}
