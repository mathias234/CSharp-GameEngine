using System.Collections.Generic;
using System.Drawing;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class RenderingEngine {
        private Camera _mainCamera;
        private Vector3 _ambientLight;

        private List<BaseLight> _lights;
        private BaseLight _activeLight;

        public RenderingEngine() {
            _lights = new List<BaseLight>();

            GL.ClearColor(0.37f, 0.59f, 1, 1);

            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.DepthClamp);

            GL.Enable(EnableCap.Texture2D);

            //_mainCamera = new Camera(MathHelper.DegreesToRadians(70.0f), (float)CoreEngine.GetWidth() / CoreEngine.GetHeight(), 0.1f, 1000);

            _ambientLight = new Vector3(0.3f);
        }

        public Vector3 GetAmbientLight
        {
            get { return _ambientLight; }
        }

        public void Render(GameObject gameObject) {
            ClearScreen();
            _lights.Clear();

            gameObject.AddToRenderingEngine(this);

            Shader forwardAmbient = ForwardAmbient.Instance;

            gameObject.Render(forwardAmbient, this);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
            GL.DepthMask(false);
            GL.DepthFunc(DepthFunction.Equal);

            foreach (var light in _lights) {
                _activeLight = light;
                gameObject.Render(light.Shader, this);
            }


            GL.DepthFunc(DepthFunction.Less);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
        }

        private static void ClearScreen() {
            //TODO: Stencil Buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        private static void SetTextures(bool enabled) {
            if (enabled)
                GL.Enable(EnableCap.Texture2D);
            else
                GL.Disable(EnableCap.Texture2D);

        }

        private static void UnbindTextures() {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private static void SetClearColor(Color color) {
            GL.ClearColor(color);
        }

        public static string GetOpenGlVersion() {
            return GL.GetString(StringName.Version);
        }

        public void AddLight(BaseLight light) {
            _lights.Add(light);
        }

        public BaseLight GetActiveLight {
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
    }
}
