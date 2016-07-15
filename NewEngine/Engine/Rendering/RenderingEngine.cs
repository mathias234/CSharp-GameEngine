using System.Collections.Generic;
using System.Drawing;
using NewEngine.Engine.components;
using NewEngine.Engine.components.UIComponents;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;
using NewEngine.Engine.Rendering.Shading.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class RenderingEngine : MappedValues {
        private Camera _mainCamera;

        private List<BaseLight> _lights;
        private BaseLight _activeLight;
        private Dictionary<string, int> _samplerMap;

        private Shader forwardAmbient;

        public RenderingEngine() : base() {

            _lights = new List<BaseLight>();
            _samplerMap = new Dictionary<string, int>();

            _samplerMap.Add("diffuse", 0);
            _samplerMap.Add("normalMap", 1);

            AddVector3("ambient", new Vector3(0.3f));

            forwardAmbient = new Shader("forward-ambient");

            GL.ClearColor(0,0,0,0);

            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.DepthClamp);

            GL.Enable(EnableCap.Texture2D);

        }

        public virtual void UpdateUniformStruct(Transform transform, Material material, Shader shader, string uniformName, string uniformType) {
            LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid type in Rendering Engine");
        }

        public void Render(GameObject gameObject) {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            gameObject.RenderAll(forwardAmbient, this);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.DepthMask(false);
            GL.DepthFunc(DepthFunction.Equal);

            foreach (var light in _lights) {
                _activeLight = light;
                gameObject.RenderAll(light.Shader, this);
            }

            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
        }

        public static string GetOpenGlVersion() {
            return GL.GetString(StringName.Version);
        }

        public void AddLight(BaseLight light) {
            _lights.Add(light);
        }

        public BaseLight GetActiveLight
        {
            get { return _activeLight; }
        }

        public Camera MainCamera
        {
            get { return _mainCamera; }
            set { _mainCamera = value; }
        }

        public void AddCamera(Camera camera) {
            MainCamera = camera;
        }

        public int GetSamplerSlot(string samplerName) {
            return _samplerMap[samplerName];
        }
    }
}
