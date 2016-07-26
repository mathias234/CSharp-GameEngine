using System.Collections.Generic;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.ResourceManagament;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class RenderingEngine : MappedValues {
        private Camera _altCamera;
        private GameObject _altCameraObject;

        private Matrix4 _biasMatrix = Matrix4.CreateTranslation(1.0f, 1.0f, 1.0f)*Matrix4.CreateScale(0.5f, 0.5f, 0.5f);

        private List<BaseLight> _lights;
        private Dictionary<string, int> _samplerMap;

        private Mesh _skybox;
        private Material _skyboxMaterial;
        private Shader _skyboxShader;

        public RenderingEngine() {
            _lights = new List<BaseLight>();
            _samplerMap = new Dictionary<string, int> {
                {"diffuse", 0},
                {"normalMap", 1},
                {"dispMap", 2},
                {"cutoutMask", 3},
                {"shadowMap", 4},
                {"skybox", 5},
                {"tex2", 6},
                {"tex2Nrm", 7},
                {"layer1", 8},
                {"tex3", 9},
                {"tex3Nrm", 10},
                {"layer2", 11}
            };


            // terrain releated

            SetVector3("ambient", new Vector3(0.3f));
            SetTexture("shadowMap",
                new Texture(null, 2048, 2048, TextureFilter.Point, PixelInternalFormat.DepthComponent16,
                    PixelFormat.DepthComponent, true, FramebufferAttachment.DepthAttachment));

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

        public BaseLight GetActiveLight { get; private set; }

        public Camera MainCamera { get; set; }

        public Matrix4 LightMatrix { get; private set; }

        // TODO: change this as i dont want people to override the RenderingEngine i rather want them to add their function using either an Action or Func <- not decided
        public virtual void UpdateUniformStruct(Transform transform, Material material, Shader shader,
            string uniformName, string uniformType) {
            LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid type in Rendering Engine");
        }

        public void Render(GameObject gameObject) {
            CoreEngine.BindAsRenderTarget();

            GL.ClearColor(0.0f, 0.0f, 0.2f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderSkybox();

            // since we are rendering the "base" pass we do not need to specify the shader
            gameObject.RenderAll(null, this, true);

            foreach (var light in _lights) {
                GetActiveLight = light;

                RenderShadowMap(light, gameObject);

                CoreEngine.BindAsRenderTarget();

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                GL.DepthMask(false);
                GL.DepthFunc(DepthFunction.Equal);

                //LogManager.Debug(light.GetType().Name);

                gameObject.RenderAll(light.GetType().Name, this, false);

                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
                GL.Disable(EnableCap.Blend);
            }
        }

        private void RenderShadowMap(BaseLight light, GameObject gameObject) {
            var shadowInfo = light.ShadowInfo;

            GetTexture("shadowMap").BindAsRenderTarget();
            GL.Clear(ClearBufferMask.DepthBufferBit);

            if (shadowInfo == null) return;
            _altCamera.SetProjection = shadowInfo.Projection;
            _altCamera.Transform.Position = GetActiveLight.Transform.GetTransformedPosition();
            _altCamera.Transform.Rotation = GetActiveLight.Transform.GetTransformedRotation();

            LightMatrix = _altCamera.GetViewProjection()*_biasMatrix;

            SetVector3("shadowTexelSize", new Vector3(1.0f/1024.0f, 1.0f/1024.0f, 0));
            SetFloat("shadowBias", shadowInfo.Bias/1024.0f);
            var flipFaces = shadowInfo.FlipFaces;


            var temp = MainCamera;
            MainCamera = _altCamera;

            if (flipFaces) GL.CullFace(CullFaceMode.Front);
            gameObject.RenderAll("shadowMapGenerator", this, false);
            if (flipFaces) GL.CullFace(CullFaceMode.Back);

            MainCamera = temp;
        }

        private void RenderSkybox() {
            GL.DepthMask(false);
            _skyboxShader.Bind();
            _skyboxShader.UpdateUniforms(MainCamera.Transform, _skyboxMaterial, this);
            _skybox.Draw();
            GL.DepthMask(true);
        }

        public void SetSkybox(string textureTopFilename, string textureBottomFilename, string textureFrontFilename,
            string textureBackFilename, string textureLeftFilename, string textureRightFilename) {
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

        public void AddCamera(Camera camera) {
            MainCamera = camera;
        }

        public int GetSamplerSlot(string samplerName) {
            return _samplerMap[samplerName];
        }
    }
}