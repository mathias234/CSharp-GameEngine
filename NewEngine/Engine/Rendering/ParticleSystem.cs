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
    public struct Particle : IEquatable<Particle>, IComparable<Particle> {
        public Vector3 position, speed;
        public float r, g, b, a;
        public float size, angle, weight;
        public float life;
        public float cameraDistance;


        public bool Equals(Particle other) {
            if (Math.Abs(other.cameraDistance - cameraDistance) < 0.05f) return true;
            return false;
        }

        public int CompareTo(Particle other) {
            if (cameraDistance < other.cameraDistance)
                return 1;
            return 0;
        }
    }

    public class ParticleSystem : GameComponent {
        private static Random _random = new Random();

        private int _maxParticles;
        private readonly Vector3 _startPostionMin;
        private readonly Vector3 _startPostionMax;
        private float _spread;
        private Vector3 _directionMin;
        private Vector3 _directionMax;
        private Vector4 _colorMin;
        private readonly Vector4 _colorMax;
        private Vector3 _gravity;

        private float _sizeMin;
        private float _sizeMax;
        private readonly float _lifeMin;
        private readonly float _lifeMax;
        private readonly bool _allowTransparency;
        private readonly bool _overwriteOldParticles;

        private int _billboardVertexBuffer;
        private int _particlesPositionBuffer;
        private int _particlesColorBuffer;

        private Particle[] _particles;

        private static float[] _particulePostitionSizeData;
        private static float[] _particuleColorData;

        private Shader _particleShader;

        private int _lastUsedParticle = 0;
        private int _newParticlesEachFrame;


        public int MaxParticles
        {
            get { return _maxParticles; }
            set
            {
                _maxParticles = value;
                Initialize();
            }
        }

        public float Spread
        {
            get { return _spread; }
            set { _spread = value; }
        }

        public float SizeMin
        {
            get { return _sizeMin; }
            set { _sizeMin = value; }
        }

        public Vector3 DirectionMin
        {
            get { return _directionMin; }
            set { _directionMin = value; }
        }

        public Vector3 Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }

        public Vector4 ColorMin
        {
            get { return _colorMin; }
            set { _colorMin = value; }
        }

        public ParticleSystem(int maxParticles, Vector3 startPostionMin, Vector3 startPostionMax, Vector4 colorMin, Vector4 colorMax, float spread, Vector3 gravity, Vector3 directionMin, Vector3 directionMax, float sizeMin, float sizeMax, float lifeMin, float lifeMax, int newParticlesEachFrame, bool allowTransparency, bool overwriteOldParticles) {
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

            _particleShader = new Shader("particles");

            GL.GenBuffers(1, out _billboardVertexBuffer);
            GL.GenBuffers(1, out _particlesPositionBuffer);
            GL.GenBuffers(1, out _particlesColorBuffer);

            Initialize();
        }

        public void Initialize() {
            _particulePostitionSizeData = new float[_maxParticles * 4];
            _particuleColorData = new float[_maxParticles * 4];
            _particles = new Particle[_maxParticles];

            for (var i = 0; i < _maxParticles; i++) {
                _particles[i].life = -1.0f;
                _particles[i].cameraDistance = -1.0f;
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

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine) {
            if (shaderType != "ParticleSystem")
                return;

            var newParticles = _newParticlesEachFrame;

            if (newParticles > MaxParticles) {
                newParticles = MaxParticles;
            }

            for (var i = 0; i < newParticles; i++) {
                var particleIndex = FindUnusedParticle();

                if (particleIndex == int.MaxValue) {
                    if (_overwriteOldParticles)
                        particleIndex = 0;
                    else
                        continue;
                }

                _particles[particleIndex].life = GetRandomNumber(_lifeMin, _lifeMax);

                Vector3 startPostion = new Vector3(
                    Transform.Position.X + GetRandomNumber(_startPostionMin.X, _startPostionMax.X),
                    Transform.Position.Y + GetRandomNumber(_startPostionMin.Y, _startPostionMax.Y),
                    Transform.Position.Z + GetRandomNumber(_startPostionMin.Z, _startPostionMax.Z)
                    );

                _particles[particleIndex].position = startPostion;


                var newDir = new Vector3();

                newDir = new Vector3(
                    GetRandomNumber(_directionMin.X, _directionMax.X),
                    GetRandomNumber(_directionMin.Y, _directionMax.Y),
                    GetRandomNumber(_directionMin.Z, _directionMax.Z));

                _particles[particleIndex].speed = newDir * _spread;



                _particles[particleIndex].r = GetRandomNumber(_colorMin.X, _colorMax.X);
                _particles[particleIndex].g = GetRandomNumber(_colorMin.Y, _colorMax.Y);
                _particles[particleIndex].b = GetRandomNumber(_colorMin.Z, _colorMax.Z);
                _particles[particleIndex].a = GetRandomNumber(_colorMin.W, _colorMax.W);

                _particles[particleIndex].size = GetRandomNumber(_sizeMin, _sizeMax);
            }

            var particleCount = 0;

            for (var i = 0; i < _maxParticles; i++) {
                var p = _particles[i];

                if (!(p.life > 0.0f)) continue;

                p.life -= deltaTime;

                if (p.life > 0.0f) {
                    p.speed += _gravity * deltaTime * 0.5f;
                    p.position += p.speed * deltaTime;

                    p.cameraDistance =
                        (p.position - CoreEngine.GetCoreEngine.RenderingEngine.MainCamera.Transform.Position)
                            .LengthSquared;

                    _particulePostitionSizeData[4 * particleCount + 0] = p.position.X;
                    _particulePostitionSizeData[4 * particleCount + 1] = p.position.Y;
                    _particulePostitionSizeData[4 * particleCount + 2] = p.position.Z;
                    _particulePostitionSizeData[4 * particleCount + 3] = p.size;

                    _particuleColorData[4 * particleCount + 0] = p.r;
                    _particuleColorData[4 * particleCount + 1] = p.g;
                    _particuleColorData[4 * particleCount + 2] = p.b;
                    _particuleColorData[4 * particleCount + 3] = p.a;
                }
                else {
                    p.cameraDistance = -1.0f;
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

            var material = new Material(new Texture("grassstraw.png"));
            material.SetTexture("cutoutMask", new Texture("grassstraw_cutout.png"));
            material.SetVector3("CameraRight_worldspace", -renderingEngine.MainCamera.Transform.Right);
            material.SetVector3("CameraUp_worldspace", renderingEngine.MainCamera.Transform.Up);

            _particleShader.UpdateUniforms(Transform, material, renderingEngine);

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


        private void SortParticles() {
            BubbleSort(_particles);
        }

        public static void BubbleSort(Particle[] a) {
            for (var i = 1; i <= a.Length - 1; ++i)
                for (var j = 0; j < a.Length - i; ++j)
                    if (a[j].cameraDistance < a[j + 1].cameraDistance)
                        Swap(ref a[j], ref a[j + 1]);

        }

        public static void Swap(ref Particle x, ref Particle y) {
            var temp = x;
            x = y;
            y = temp;
        }


        private int FindUnusedParticle() {
            for (var i = _lastUsedParticle; i < _maxParticles; i++) {
                if (!(_particles[i].life < 0)) continue;

                _lastUsedParticle = i;

                return i;
            }

            for (var i = 0; i < _lastUsedParticle; i++) {
                if (!(_particles[i].life < 0)) continue;

                _lastUsedParticle = i;

                return i;
            }

            if (_particles[0].life < 0)
                return 0;
            else
                return int.MaxValue;
        }

        public float GetRandomNumber(double minimum, double maximum) {
            return (float)(_random.NextDouble() * (maximum - minimum) + minimum);
        }
    }
}
