using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        // TEMP VARIABLES
        private static Texture g_TempTarget;
        private static Mesh g_Mesh;
        private static Transform g_Transform;
        private static Material g_Material;
        private static Camera g_Camera;
        private static GameObject g_cameraObject;

        public RenderingEngine() : base() {
            CoreEngine.BindAsRenderTarget();

            _lights = new List<BaseLight>();
            _samplerMap = new Dictionary<string, int>();

            _samplerMap.Add("diffuse", 0);
            _samplerMap.Add("normalMap", 1);
            _samplerMap.Add("dispMap", 2);

            AddVector3("ambient", new Vector3(0.1f));

            forwardAmbient = new Shader("forward-ambient");

            GL.ClearColor(0, 0, 0, 0);

            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.DepthClamp);

            GL.Enable(EnableCap.Texture2D);


            //int width = CoreEngine.GetWidth() / 8;
            //int height = CoreEngine.GetHeight() / 8;
            //int dataSize = width * height * 4;

            //char[] data = new char[dataSize];

            //g_TempTarget = new Texture(data, width, height, TextureFilter.Point, FramebufferAttachment.ColorAttachment0);

            //Vertex[] vertices = { new Vertex(new Vector3(-1,-1,0),new Vector2(1,0)),
            //             new Vertex(new Vector3(-1,1,0),new Vector2(1,1)),
            //             new Vertex(new Vector3(1,1,0),new Vector2(0,1)),
            //             new Vertex(new Vector3(1,-1,0),new Vector2(0,0)) };

            //int[] indices = { 2, 1, 0,
            //                3, 2, 0 };

            //g_Material = new Material(g_TempTarget, 1, 8);

            //g_Transform = new Transform();


            //g_Mesh = new Mesh(vertices, indices, true);

            //g_Camera = new Camera(Matrix4.Identity);
            //g_cameraObject = new GameObject().AddComponent(g_Camera);

            //g_Camera.Transform.Rotate(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(-180.0f));
        }

        public virtual void UpdateUniformStruct(Transform transform, Material material, Shader shader, string uniformName, string uniformType) {
            LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid type in Rendering Engine");
        }

        public void Render(GameObject gameObject) {
            //g_TempTarget.BindAsRenderTarget();

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

            // Temp render
            //CoreEngine.BindAsRenderTarget();

            //Camera temp = MainCamera;
            //_mainCamera = g_Camera;


            //GL.ClearColor(0.0f, 0.0f, 0.5f, 1.0f);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //forwardAmbient.Bind();
            //forwardAmbient.UpdateUniforms(g_Transform, g_Material, this);
            //g_Mesh.Draw();

            //MainCamera = temp;
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
