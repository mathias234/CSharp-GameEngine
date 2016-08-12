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
        private Dictionary<string, List<GameObject>> _batches = new Dictionary<string, List<GameObject>>();
        private List<GameObject> _waters = new List<GameObject>();

        private Mesh _skybox;
        private Material _skyboxMaterial;
        private Shader _skyboxShader;
        private Shader _nullFilter;
        private Shader _gausFilter;
        private Shader _fxaaFilter;
        private Shader _brightFilter;
        private Shader _combineFilter;

        private Mesh _plane;
        private Transform _planeTransform;
        private Material _planeMaterial;
        private Texture _tempTarget;


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
                {"filterTexture", 12},
                {"tempFilter", 13},
                {"tempFilter2", 14},
                {"displayTexture", 15},
                {"reflectionTexture", 16},
                {"refractionTexture", 17 },
                {"refractionTextureDepth", 18 },
                { "tempShadowMap", 19}
            };

            SetVector3("ambient", new Vector3(0.3f));
            SetFloat("fxaaSpanMax", 8);
            SetFloat("fxaaReduceMin", 1 / 128.0f);
            SetFloat("fxaaReduceMul", 1 / 8.0f);
            SetFloat("bloomAmount", 0.2f);

            SetVector4("clipPlane", new Vector4(0, 0, 0, 15));

            SetTexture("displayTexture", new Texture(IntPtr.Zero, (int)CoreEngine.GetWidth(), (int)CoreEngine.GetHeight(), TextureMinFilter.Linear));
            SetTexture("tempFilter", new Texture(IntPtr.Zero, (int)CoreEngine.GetWidth(), (int)CoreEngine.GetHeight(), TextureMinFilter.Linear));
            SetTexture("tempFilter2", new Texture(IntPtr.Zero, (int)CoreEngine.GetWidth(), (int)CoreEngine.GetHeight(), TextureMinFilter.Linear));
            SetTexture("tempShadowMap", new Texture(IntPtr.Zero, (int)CoreEngine.GetWidth(), (int)CoreEngine.GetHeight(), TextureMinFilter.Linear));


            SetTexture("reflectionTexture", new Texture(IntPtr.Zero, 960, 540, TextureMinFilter.Linear));
            SetTexture("refractionTexture", new Texture(IntPtr.Zero, 960, 540, TextureMinFilter.Linear));
            SetTexture("refractionTextureDepth", new Texture(IntPtr.Zero, 960, 540, TextureMinFilter.Linear, PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent, false, FramebufferAttachment.DepthAttachment));

            _skyboxShader = new Shader("skybox");
            _nullFilter = new Shader("filters/filter-null");
            _gausFilter = new Shader("filters/filter-gausBlur7x1");
            _fxaaFilter = new Shader("filters/filter-fxaa");
            _brightFilter = new Shader("filters/filter-bright");
            _combineFilter = new Shader("filters/filter-combine");

            GL.ClearColor(0, 0, 0, 0);

            GL.FrontFace(FrontFaceDirection.Cw);
            //GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthClamp);
            GL.Enable(EnableCap.ClipPlane0);

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

        public void RenderBatches(float deltaTime) {
            RenderShadowMap(deltaTime);
            RenderReflectRefractBuffers(deltaTime);

            RenderObject(GetTexture("displayTexture"), deltaTime, "base");
            //RenderObject(GetTexture("displayTexture"), deltaTime, "ui");

            foreach (var water in _waters) {
                water.RenderAll(null, null, deltaTime, this, "water");
            }

            DoPostProccess();

            CoreEngine.GetCoreEngine.SwapBuffers();
        }

        private void DoPostProccess() {
            SetVector3("inverseFilterTextureSize", new Vector3(1.0f / GetTexture("displayTexture").Width, 1.0f / GetTexture("displayTexture").Height, 0.0f));

            ApplyFilter(_brightFilter, GetTexture("tempFilter"), GetTexture("tempFilter2"));

            BlurTexture(GetTexture("tempFilter2"), 10, Vector2.UnitX);
            BlurTexture(GetTexture("tempFilter2"), 10, Vector2.UnitY);

            BlurTexture(GetTexture("tempFilter2"), 10, Vector2.UnitX);
            BlurTexture(GetTexture("tempFilter2"), 10, Vector2.UnitY);

            ApplyFilter(_combineFilter, GetTexture("tempFilter2"), GetTexture("tempFilter"));


            ApplyFilter(_fxaaFilter, GetTexture("tempFilter"), null);
            // ApplyFilter(_fxaaFilter, _lights[1].ShadowInfo.ShadowMap, null);
        }

        // fill the Reflect and Refract textures
        private void RenderReflectRefractBuffers(float deltaTime) {

            var distance = 2 * (MainCamera.Transform.Position.Y - 0);

            MainCamera.Transform.Position -= new Vector3(0, distance, 0);
            MainCamera.Transform.Rotation = MainCamera.Transform.Rotation.InvertPitch();

            SetVector4("clipPlane", new Vector4(0, 1, 0, -1));
            RenderObject(GetTexture("reflectionTexture"), deltaTime, "reflect");

            MainCamera.Transform.Position += new Vector3(0, distance, 0);
            MainCamera.Transform.Rotation = MainCamera.Transform.Rotation.InvertPitch(); ;

            SetVector4("clipPlane", new Vector4(0, -1, 0, 1));
            RenderObject(GetTexture("refractionTexture"), deltaTime, "refract");
            RenderObject(GetTexture("refractionTextureDepth"), deltaTime, "refract");

            SetVector4("clipPlane", new Vector4(0, 0, 0, 0));
        }

        private void RenderObject(Texture mainRenderTarget, float deltaTime, string renderStage) {
            mainRenderTarget.BindAsRenderTarget();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            RenderSkybox();

            foreach (var batch in _batches) {
                foreach (var gameObject in batch.Value) {
                    gameObject.RenderAll(null, "base", deltaTime, this, renderStage);

                    foreach (var light in _lights) {
                        ActiveLight = light;
                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
                        GL.DepthMask(false);
                        GL.DepthFunc(DepthFunction.Equal);

                        if (light.ShadowInfo != null)
                            SetTexture("shadowMap", light.ShadowInfo.ShadowMap);

                        gameObject.RenderAll(light.GetType().Name, "light", deltaTime, this, "lightStage");

                        GL.DepthMask(true);
                        GL.DepthFunc(DepthFunction.Less);
                        GL.Disable(EnableCap.Blend);
                    }
                }
            }

        }

        private void RenderShadowMap(float deltaTime) {
            foreach (var light in _lights) {
                if (light.ShadowInfo != null) {
                    light.ShadowInfo.ShadowMap.BindAsRenderTarget();

                    GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
                    GL.ClearColor(1.0f, 1.0f, 0.0f, 0.0f);


                    _altCamera.SetProjection = light.ShadowInfo.Projection;
                    ShadowCameraTransform shadowCameraTransform = light.CalcShadowCameraTransform(MainCamera.Transform);

                    _altCamera.Transform.Position = shadowCameraTransform.pos;
                    _altCamera.Transform.Rotation = shadowCameraTransform.rot;

                    LightMatrix = _altCamera.GetViewProjection() * _biasMatrix;

                    SetFloat("shadowVarianceMin", light.ShadowInfo.MinVariance);
                    SetFloat("shadowBleedingReduction", light.ShadowInfo.LightBleedReductionAmount);

                    var flipFaces = light.ShadowInfo.FlipFaces;

                    var temp = MainCamera;
                    MainCamera = _altCamera;

                    if (flipFaces) GL.CullFace(CullFaceMode.Front);

                    foreach (var batch in _batches) {
                        foreach (var batchObj in batch.Value) {
                            batchObj.RenderAll("shadowMapGenerator", "shadowMap", deltaTime, this, "shadows");
                        }
                    }

                    if (flipFaces) GL.CullFace(CullFaceMode.Back);

                    MainCamera = temp;

                    var shadowSoftness = light.ShadowInfo.ShadowSoftness;

                    if (Math.Abs(shadowSoftness) > 0.0001f)
                        BlurShadowMap(light, shadowSoftness);

                }
                else {
                    LightMatrix = Matrix4.CreateScale(0, 0, 0);
                    SetFloat("shadowVarianceMin", 0.00002f);
                    SetFloat("shadowBleedingReduction", 0.0f);
                }
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

        public void BlurShadowMap(BaseLight light, float blurAmount) {
            var shadowMap = light.ShadowInfo.ShadowMap;
            var tempTarget = GetTexture("tempShadowMap");

            SetVector3("blurScale", new Vector3(blurAmount / shadowMap.Width, 0.0f, 0.0f));
            ApplyFilter(_gausFilter, shadowMap, tempTarget);

            SetVector3("blurScale", new Vector3(0.0f, blurAmount / shadowMap.Height, 0.0f));
            ApplyFilter(_gausFilter, tempTarget, shadowMap);
        }

        public void BlurTexture(Texture texture, float blurAmount, Vector2 axis) {
            SetVector3("blurScale", new Vector3(0.0f, blurAmount / texture.Height, 0.0f));

            var temp = GetTexture("tempFilter");

            if (axis == Vector2.UnitX) {
                SetVector3("blurScale", new Vector3(0.0f, blurAmount / texture.Width, 0.0f));
            }
            else if (axis == Vector2.UnitY) {
                SetVector3("blurScale", new Vector3(0.0f, blurAmount / texture.Height, 0.0f));
            }

            ApplyFilter(_gausFilter, texture, GetTexture("tempFilter"));

            // put the blured texture back into the original texture
            ApplyFilter(_nullFilter, GetTexture("tempFilter"), texture);

            SetTexture("tempFilter", temp);
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

        public void AddObjectToBatch(Mesh mesh, GameObject gameObject) {
            if (_batches.ContainsKey(mesh.Filename)) {
                _batches[mesh.Filename].Add(gameObject);
            }
            else {
                var obj = new List<GameObject>();
                obj.Add(gameObject);
                _batches.Add(mesh.Filename, obj);
            }
        }

        public void RemoveFromBatch(Mesh mesh, GameObject gameObject) {
            if (_batches.ContainsKey(mesh.Filename)) {
                var batch = _batches[mesh.Filename];
                batch.Remove(gameObject);

                _batches[mesh.Filename] = batch;
            }
        }

        public void AddWater(GameObject gameObject) {
            _waters.Add(gameObject);
        }

        public void RemoveWater(GameObject gameObject) {
            if (_waters.Contains(gameObject)) {
                _waters.Remove(gameObject);
            }
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