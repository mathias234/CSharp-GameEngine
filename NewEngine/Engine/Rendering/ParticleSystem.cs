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
    public class ParticleSystem : GameComponent {
        private static Random _random = new Random();

        private int _maxParticles;
        private Vector3 _startPostionMin;
        private Vector3 _startPostionMax;
        private float _spread;
        private Vector3 _directionMin;
        private Vector3 _directionMax;
        private Vector4 _colorMin;
        private Vector4 _colorMax;
        private Vector3 _gravity;

        private float _sizeMin;
        private float _sizeMax;
        private float _lifeMin;
        private float _lifeMax;
        private bool _allowTransparency;
        private bool _overwriteOldParticles;
        private bool _fadeOut;

        private int _billboardVertexBuffer;
        private int _particlesPositionBuffer;
        private int _particlesColorBuffer;

        private Particle[] _particles;

        private static float[] _particulePostitionSizeData;
        private static float[] _particuleColorData;

        private Shader _particleShader;

        private int _lastUsedParticle = 0;
        private int _newParticlesEachFrame;
        private Material _material;

        public ParticleSystem(int maxParticles, Vector3 startPostionMin, Vector3 startPostionMax, Vector4 colorMin, Vector4 colorMax, float spread, Vector3 gravity, Vector3 directionMin, Vector3 directionMax, float sizeMin, float sizeMax, float lifeMin, float lifeMax, int newParticlesEachFrame, bool allowTransparency, bool overwriteOldParticles, bool fadeOut) {
            _maxParticles = maxParticles;
            _startPostionMin = startPostionMin;
            _startPostionMax = startPostionMax;
            _colorMin = colorMin;
            _colorMax = colorMax;
            _spread = spread;
            _gravity = gravity;
            _directionMin = directionMin;
            _directionMax = directionMax;
            _sizeMin = sizeMin;
            _sizeMax = sizeMax;
            _lifeMin = lifeMin;
            _lifeMax = lifeMax;
            _newParticlesEachFrame = newParticlesEachFrame;
            _allowTransparency = allowTransparency;
            _overwriteOldParticles = overwriteOldParticles;
            _fadeOut = fadeOut;

            _particleShader = new Shader("particles");

            GL.GenBuffers(1, out _billboardVertexBuffer);
            GL.GenBuffers(1, out _particlesPositionBuffer);
            GL.GenBuffers(1, out _particlesColorBuffer);

            _material = new Material(new Texture("test2.png"));
            _material.SetTexture("cutoutMask", new Texture("test2_cutout.png"));

            Initialize();
        }

        public void Initialize() {
            _particulePostitionSizeData = new float[_maxParticles * 4];
            _particuleColorData = new float[_maxParticles * 4];
            _particles = new Particle[_maxParticles];

            for (var i = 0; i < _maxParticles; i++) {
                _particles[i].Life = -1.0f;
                _particles[i].CameraDistance = -1.0f;
            }

            float[] vertexBufferData = {
                    -0.5f, -0.5f, 0.0f,
                    0.5f, -0.5f, 0.0f,
                    -0.5f, 0.5f, 0.0f,
                    0.5f, 0.5f, 0.0f
                };

            GL.BindBuffer(BufferTarget.ArrayBuffer, _billboardVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * vertexBufferData.Length), vertexBufferData, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesPositionBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_maxParticles * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_maxParticles * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);
        }

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            if (shaderType.ToLower() != "base") {
                return;
            }
            if (renderStage.ToLower() == "refract" || renderStage.ToLower() == "reflect") {
                return;
            }

            var newParticles = _newParticlesEachFrame;

            if (newParticles > _maxParticles) {
                newParticles = _maxParticles;
            }

            for (var i = 0; i < newParticles; i++) {
                var particleIndex = FindUnusedParticle();

                if (particleIndex == int.MaxValue) {
                    if (_overwriteOldParticles)
                        particleIndex = 0;
                    else
                        continue;
                }

                _particles[particleIndex].Life = GetRandomNumber(_lifeMin, _lifeMax);

                Vector3 startPostion = new Vector3(
                    Transform.Position.X + GetRandomNumber(_startPostionMin.X, _startPostionMax.X),
                    Transform.Position.Y + GetRandomNumber(_startPostionMin.Y, _startPostionMax.Y),
                    Transform.Position.Z + GetRandomNumber(_startPostionMin.Z, _startPostionMax.Z)
                    );

                _particles[particleIndex].Position = startPostion;


                var newDir = new Vector3();

                newDir = new Vector3(
                    GetRandomNumber(_directionMin.X, _directionMax.X),
                    GetRandomNumber(_directionMin.Y, _directionMax.Y),
                    GetRandomNumber(_directionMin.Z, _directionMax.Z));

                _particles[particleIndex].Speed = newDir * _spread;



                _particles[particleIndex].R = GetRandomNumber(_colorMin.X, _colorMax.X);
                _particles[particleIndex].G = GetRandomNumber(_colorMin.Y, _colorMax.Y);
                _particles[particleIndex].B = GetRandomNumber(_colorMin.Z, _colorMax.Z);
                _particles[particleIndex].A = GetRandomNumber(_colorMin.W, _colorMax.W);

                _particles[particleIndex].Size = GetRandomNumber(_sizeMin, _sizeMax);
            }

            var particleCount = 0;

            for (var i = 0; i < _maxParticles; i++) {
                var p = _particles[i];

                if (!(p.Life > 0.0f)) continue;

                p.Life -= deltaTime;

                if (p.Life > 0.0f) {
                    p.Speed += _gravity * deltaTime * 0.5f;
                    p.Position += p.Speed * deltaTime;

                    p.CameraDistance =
                        (p.Position - CoreEngine.GetCoreEngine.RenderingEngine.MainCamera.Transform.Position)
                            .LengthSquared;

                    _particulePostitionSizeData[4 * particleCount + 0] = p.Position.X;
                    _particulePostitionSizeData[4 * particleCount + 1] = p.Position.Y;
                    _particulePostitionSizeData[4 * particleCount + 2] = p.Position.Z;
                    _particulePostitionSizeData[4 * particleCount + 3] = p.Size;

                    _particuleColorData[4 * particleCount + 0] = p.R;
                    _particuleColorData[4 * particleCount + 1] = p.G;
                    _particuleColorData[4 * particleCount + 2] = p.B;

                    if (_fadeOut && p.Life <= 1) {
                        _particuleColorData[4 * particleCount + 3] = p.A * p.Life;
                    }
                    else {
                        _particuleColorData[4 * particleCount + 3] = p.A;
                    }

                }
                else {
                    p.CameraDistance = -1.0f;
                }

                _particles[i] = p;

                particleCount++;
            }

            if (_allowTransparency) {
                SortParticles();
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesPositionBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_maxParticles * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(particleCount * sizeof(float) * 4), _particulePostitionSizeData);


            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_maxParticles * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(particleCount * sizeof(float) * 4), _particuleColorData);


            if (_allowTransparency) {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            }

            _particleShader.Bind();


            _material.SetVector3("CameraRight_worldspace", -renderingEngine.MainCamera.Transform.Right);
            _material.SetVector3("CameraUp_worldspace", renderingEngine.MainCamera.Transform.Up);

            _particleShader.UpdateUniforms(Transform, _material, renderingEngine);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _billboardVertexBuffer);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);


            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesPositionBuffer);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesColorBuffer);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 0, 0);

            GL.VertexAttribDivisor(0, 0);
            GL.VertexAttribDivisor(1, 1);
            GL.VertexAttribDivisor(2, 1);

            GL.DrawArraysInstanced(BeginMode.TriangleStrip, 0, 4, particleCount);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.VertexAttribDivisor(0, 0);
            GL.VertexAttribDivisor(1, 0);
            GL.VertexAttribDivisor(2, 0);

            GL.Disable(EnableCap.Blend);
        }

        public override void AddToEngine(CoreEngine engine) {
            base.AddToEngine(engine);
            engine.RenderingEngine.AddNonBatched(gameObject);
        }

        public override void OnDestroyed(CoreEngine engine) {
            base.AddToEngine(engine);
            engine.RenderingEngine.RemoveNonBatched(gameObject);
        }


        public float GetRandomNumber(double minimum, double maximum) {
            return (float)(_random.NextDouble() * (maximum - minimum) + minimum);
        }

        public int MaxParticles
        {
            get { return _maxParticles; }
            set
            {
                _maxParticles = value;
                Initialize();
            }
        }

        public static void BubbleSort(Particle[] a) {
            for (var i = 1; i <= a.Length - 1; ++i)
                for (var j = 0; j < a.Length - i; ++j)
                    if (a[j].CameraDistance < a[j + 1].CameraDistance)
                        Swap(ref a[j], ref a[j + 1]);

        }

        public static void Swap(ref Particle x, ref Particle y) {
            var temp = x;
            x = y;
            y = temp;
        }

        private void SortParticles() {
            BubbleSort(_particles);
        }

        private int FindUnusedParticle() {
            for (var i = _lastUsedParticle; i < _maxParticles; i++) {
                if (!(_particles[i].Life < 0)) continue;

                _lastUsedParticle = i;

                return i;
            }

            for (var i = 0; i < _lastUsedParticle; i++) {
                if (!(_particles[i].Life < 0)) continue;

                _lastUsedParticle = i;

                return i;
            }

            if (_particles[0].Life < 0)
                return 0;
            else
                return int.MaxValue;
        }
#region getter/setter

        public Vector3 StartPostionMin
        {
            get { return _startPostionMin; }
            set { _startPostionMin = value; }
        }

        public Vector3 StartPostionMax
        {
            get { return _startPostionMax; }
            set { _startPostionMax = value; }
        }

        public float Spread
        {
            get { return _spread; }
            set { _spread = value; }
        }

        public Vector3 DirectionMin
        {
            get { return _directionMin; }
            set { _directionMin = value; }
        }

        public Vector3 DirectionMax
        {
            get { return _directionMax; }
            set { _directionMax = value; }
        }

        public Vector4 ColorMin
        {
            get { return _colorMin; }
            set { _colorMin = value; }
        }

        public Vector4 ColorMax
        {
            get { return _colorMax; }
            set { _colorMax = value; }
        }

        public Vector3 Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }

        public float SizeMin
        {
            get { return _sizeMin; }
            set { _sizeMin = value; }
        }

        public float SizeMax
        {
            get { return _sizeMax; }
            set { _sizeMax = value; }
        }

        public float LifeMin
        {
            get { return _lifeMin; }
            set { _lifeMin = value; }
        }

        public float LifeMax
        {
            get { return _lifeMax; }
            set { _lifeMax = value; }
        }

        public bool AllowTransparency
        {
            get { return _allowTransparency; }
            set { _allowTransparency = value; }
        }

        public bool OverwriteOldParticles
        {
            get { return _overwriteOldParticles; }
            set { _overwriteOldParticles = value; }
        }

        public bool FadeOut
        {
            get { return _fadeOut; }
            set { _fadeOut = value; }
        }
#endregion
    }
}
