using System.Collections.Generic;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class RenderingEngine : MappedValues {
        private Camera _mainCamera;
        private Camera _altCamera;
        private GameObject _altCameraObject;

        private List<BaseLight> _lights;
        private BaseLight _activeLight;
        private Dictionary<string, int> _samplerMap;

        private Shader _forwardAmbient;
        private Shader _shadowMapShader;

        private Matrix4 _lightMatrix;

        private Matrix4 _biasMatrix = Matrix4.CreateTranslation(1.0f, 1.0f, 1.0f) * Matrix4.CreateScale(0.5f, 0.5f, 0.5f);

        private Mesh _skybox;
        private Shader _skyboxShader;
        private Material _skyboxMaterial;

        public RenderingEngine() : base() {
            _lights = new List<BaseLight>();
            _samplerMap = new Dictionary<string, int>();

            _samplerMap.Add("diffuse", 0);
            _samplerMap.Add("normalMap", 1);
            _samplerMap.Add("dispMap", 2);
            _samplerMap.Add("cutoutMask", 3);
            _samplerMap.Add("shadowMap", 4);
            _samplerMap.Add("skybox", 5);

            SetVector3("ambient", new Vector3(0.3f));
            SetTexture("shadowMap", new Texture(null, 2048, 2048, TextureFilter.Point, PixelInternalFormat.DepthComponent16, PixelFormat.DepthComponent, true, FramebufferAttachment.DepthAttachment));

            _forwardAmbient = new Shader("forward-ambient");
            _shadowMapShader = new Shader("shadowMapGenerator");
            _skyboxShader = new Shader("skybox");

            GL.ClearColor(0, 0, 0, 0);

            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);

            _altCamera = new Camera(Matrix4.Identity);
            _altCameraObject = new GameObject().AddComponent(_altCamera);

            _skyboxMaterial = new Material(null);
            _skybox = new Mesh("skybox.obj");
        }

        // TODO: change this as i dont want people to override the RenderingEngine i rather want them to add their function using either an Action or Func <- not decided
        public virtual void UpdateUniformStruct(Transform transform, Material material, Shader shader, string uniformName, string uniformType) {
            LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid type in Rendering Engine");
        }

        public void Render(GameObject gameObject) {
            CoreEngine.BindAsRenderTarget();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // render the skybox first!
            RenderSkybox();

            gameObject.RenderAll(_forwardAmbient, this);

            foreach (var light in _lights) {
                _activeLight = light;

                RenderShadowMap(light, gameObject);

                CoreEngine.BindAsRenderTarget();

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                GL.DepthMask(false);
                GL.DepthFunc(DepthFunction.Equal);

                gameObject.RenderAll(light.Shader, this);

                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
                GL.Disable(EnableCap.Blend);
            }
        }

        public void RenderShadowMap(BaseLight light, GameObject gameObject) {

            ShadowInfo shadowInfo = light.ShadowInfo;

            GetTexture("shadowMap").BindAsRenderTarget();
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (shadowInfo != null) {
                _altCamera.SetProjection = shadowInfo.Projection;
                _altCamera.Transform.Position = _activeLight.Transform.GetTransformedPosition();
                _altCamera.Transform.Rotation = _activeLight.Transform.GetTransformedRotation();

                _lightMatrix = _altCamera.GetViewProjection() * _biasMatrix;

                SetVector3("shadowTexelSize", new Vector3(1.0f / 1024.0f, 1.0f / 1024.0f, 0));
                SetFloat("shadowBias", shadowInfo.Bias / 1024.0f);
                bool flipFaces = shadowInfo.FlipFaces;


                Camera temp = _mainCamera;
                _mainCamera = _altCamera;

                if (flipFaces) GL.CullFace(CullFaceMode.Front);
                gameObject.RenderAll(_shadowMapShader, this);
                if (flipFaces) GL.CullFace(CullFaceMode.Back);
                _mainCamera = temp;
            }

        }

        //TODO: implement skybox rendering
        public void RenderSkybox() {
            GL.DepthMask(false);
            _skyboxShader.Bind();
            _skyboxShader.UpdateUniforms(MainCamera.Transform, _skyboxMaterial, this);
            _skybox.Draw();
            GL.DepthMask(true);
        }

        public void SetSkybox(string textureTopFilename, string textureBottomFilename, string textureFrontFilename, string textureBackFilename, string textureLeftFilename, string textureRightFilename) {
            var cubemap = new CubemapTexture(textureTopFilename, textureBottomFilename, textureFrontFilename,
                textureBackFilename, textureLeftFilename, textureRightFilename);
            _skyboxMaterial.SetCubemapTexture("skybox", cubemap);
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

        public Matrix4 LightMatrix
        {
            get { return _lightMatrix; }
        }

        public void AddCamera(Camera camera) {
            MainCamera = camera;
        }

        public int GetSamplerSlot(string samplerName) {
            return _samplerMap[samplerName];
        }
    }
}
