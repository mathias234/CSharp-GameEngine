using System;
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

        private Matrix4 _biasMatrix = Matrix4.CreateTranslation(1.0f, 1.0f, 1.0f) * Matrix4.CreateScale(0.5f, 0.5f, 0.5f);

        private List<BaseLight> _lights;
        private Dictionary<string, int> _samplerMap;

        private Mesh _skybox;
        private Material _skyboxMaterial;
        private Shader _skyboxShader;
        private Shader _nullFilter;
        private Shader _gausFilter;

        private Mesh _plane;
        private Transform _planeTransform;
        private Material _planeMaterial;
        private Texture _tempTarget;


        public static int NumShadowMaps = 10;

        private Texture[] _shadowMaps = new Texture[NumShadowMaps];
        private Texture[] _shadowMapsTempTargets = new Texture[NumShadowMaps];


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
                {"layer2", 11},
                {"filterTexture", 12}
            };

            SetVector3("ambient", new Vector3(0.2f));

            _skyboxShader = new Shader("skybox");
            _nullFilter = new Shader("filters/filter-null");
            _gausFilter = new Shader("filters/filter-gausBlur7x1");

            GL.ClearColor(0, 0, 0, 0);

            GL.FrontFace(FrontFaceDirection.Cw);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);

            _altCamera = new Camera(Matrix4.Identity);
            _altCameraObject = new GameObject("alt camera").AddComponent(_altCamera);

            _skyboxMaterial = new Material(null);
            _skybox = new Mesh("skybox.obj");

            int width = (int)CoreEngine.GetWidth();
            int height = (int)CoreEngine.GetHeight();

            _tempTarget = new Texture(null, width, height, TextureMinFilter.Nearest);

            _plane = PrimitiveObjects.CreatePlane;
            _planeMaterial = new Material(_tempTarget, 1, 8);
            _planeTransform = new Transform();
            _planeTransform.Rotate(new Vector3(0, 1, 0), MathHelper.DegreesToRadians(180.0f));

            for (int i = 0; i < NumShadowMaps; i++) {
                int shadowMapSize = 1 << (i + 1);
                _shadowMaps[i] = new Texture((IntPtr)0, shadowMapSize, shadowMapSize, TextureMinFilter.Linear, PixelInternalFormat.Rg32f, PixelFormat.Rgba, true);
                _shadowMapsTempTargets[i] = new Texture((IntPtr)0, shadowMapSize, shadowMapSize, TextureMinFilter.Linear, PixelInternalFormat.Rg32f, PixelFormat.Rgba, true);
            }

            LightMatrix = Matrix4.CreateScale(0, 0, 0);

        }

        public BaseLight ActiveLight { get; private set; }

        public Camera MainCamera { get; set; }

        public Matrix4 LightMatrix { get; private set; }

        // TODO: change this as i dont want people to override the RenderingEngine i rather want them to add their function using either an Action or Func <- not decided
        public virtual void UpdateUniformStruct(Transform transform, Material material, Shader shader,
            string uniformName, string uniformType) {
            LogManager.Error("Failed to update uniform: " + uniformName + ", not a valid type in Rendering Engine");
        }

        public void Render(GameObject gameObject, float deltaTime) {
            CoreEngine.BindAsRenderTarget();

            GL.ClearColor(0.0f, 0.0f, 0.3f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderSkybox();

            // since we are rendering the "base" pass we do not need to specify the shader
            gameObject.RenderAll(null, "base", deltaTime, this);

            foreach (var light in _lights) {
                ActiveLight = light;

                RenderShadowMap(ActiveLight, deltaTime, gameObject);

                CoreEngine.BindAsRenderTarget();

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                GL.DepthMask(false);
                GL.DepthFunc(DepthFunction.Equal);

                gameObject.RenderAll(light.GetType().Name, "light", deltaTime, this);

                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
                GL.Disable(EnableCap.Blend);

            }

            // render the particle system last
            gameObject.RenderAll("", "ParticleSystem", deltaTime, this);

            CoreEngine.GetCoreEngine.SwapBuffers();
        }

        private void RenderShadowMap(BaseLight light, float deltaTime, GameObject gameObject) {
            var shadowInfo = light.ShadowInfo;

            int shadowMapIndex = 0;

            if (shadowInfo != null)
                shadowMapIndex = shadowInfo.ShadowMapSizeAsPowerOf2 - 1;

            SetTexture("shadowMap", _shadowMaps[shadowMapIndex]);
            _shadowMaps[shadowMapIndex].BindAsRenderTarget();
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.ClearColor(1.0f, 1.0f, 0.0f, 0.0f);


            if (shadowInfo != null) {
                _altCamera.SetProjection = shadowInfo.Projection;

                ShadowCameraTransform shadowCameraTransform =
                    ActiveLight.CalcShadowCameraTransform(MainCamera.Transform);

                _altCamera.Transform.Position = shadowCameraTransform.pos;
                _altCamera.Transform.Rotation = shadowCameraTransform.rot;

                LightMatrix = _altCamera.GetViewProjection() * _biasMatrix;

                SetFloat("shadowVarianceMin", shadowInfo.MinVariance);
                SetFloat("shadowBleedingReduction", shadowInfo.LightBleedReductionAmount);

                var flipFaces = shadowInfo.FlipFaces;

                var temp = MainCamera;
                MainCamera = _altCamera;

                if (flipFaces) GL.CullFace(CullFaceMode.Front);
                gameObject.RenderAll("shadowMapGenerator", "light", deltaTime, this);
                if (flipFaces) GL.CullFace(CullFaceMode.Back);

                MainCamera = temp;

                var shadowSoftness = shadowInfo.ShadowSoftness;

                if (Math.Abs(shadowSoftness) > 0.0001f)
                    BlurShadowMap(shadowMapIndex, shadowSoftness);
            }
            else {
                LightMatrix = Matrix4.CreateScale(0, 0, 0);
                SetFloat("shadowVarianceMin", 0.00002f);
                SetFloat("shadowBleedingReduction", 0.0f);
            }

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

        public void BlurShadowMap(int shadowMapIndex, float blurAmount) {
            var shadowMap = _shadowMaps[shadowMapIndex];
            var tempTarget = _shadowMapsTempTargets[shadowMapIndex];

            SetVector3("blurScale", new Vector3(blurAmount / shadowMap.Width, 0.0f, 0.0f));
            ApplyFilter(_gausFilter, shadowMap, tempTarget);

            SetVector3("blurScale", new Vector3(0.0f, blurAmount / shadowMap.Height, 0.0f));
            ApplyFilter(_gausFilter, tempTarget, shadowMap);
        }

        public void ApplyFilter(Shader filter, Texture source, Texture dest) {
            if (source == dest) LogManager.Error("ApplyFilter: source texture cannot be the same as dest texture!");
            if (dest == null)
                CoreEngine.BindAsRenderTarget();
            else
                dest.BindAsRenderTarget();

            SetTexture("filterTexture", source);

            _altCameraObject.Transform.Rotation = Quaternion.Identity;
            _altCamera.SetProjection = Matrix4.Identity;
            _altCameraObject.Transform.Position = new Vector3(0, 0, 0);

            Camera temp = MainCamera;
            MainCamera = _altCamera;

            GL.ClearColor(0, 0, 0.5f, 1.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            filter.Bind();
            filter.UpdateUniforms(_planeTransform, _planeMaterial, this);
            _plane.Draw();

            MainCamera = temp;
            SetTexture("filterTexture", null);
        }

        public static string GetOpenGlVersion() {
            return GL.GetString(StringName.Version);
        }

        public void AddLight(BaseLight light) {
            _lights.Add(light);
        }

        public void RemoveLight(BaseLight light) {
            _lights.Remove(light);
        }

        public void AddCamera(Camera camera) {
            MainCamera = camera;
        }

        public int GetSamplerSlot(string samplerName) {
            return _samplerMap[samplerName];
        }
    }
}