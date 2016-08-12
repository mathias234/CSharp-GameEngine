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
        private readonly Mesh _mesh;
        private static Random _random = new Random();

        private int _batchCount;

        private int _vertexBuffer;
        private int _particlesPositionsBuffer;

        private Batch[] _batches;

        private static float[] _particulePostitionSizeData;
        private static float[] _particuleColorData;

        private Shader _particleShader;

        private Material _material;

        public BatchMeshRenderer(Material[] material, Transform[] transform, Mesh mesh, Shader shader) {
            _mesh = mesh;
            _particleShader = new Shader("particles");

            GL.GenBuffers(1, out _vertexBuffer);
            GL.GenBuffers(1, out _particlesPositionsBuffer);

            _material = new Material(new Texture("test2.png"));
            _material.SetTexture("cutoutMask", new Texture("test2_cutout.png"));

            Initialize();
        }

        public void Initialize() {
            _particulePostitionSizeData = new float[_batchCount * 4];
            _particuleColorData = new float[_batchCount * 4];
            _batches = new Batch[_batchCount];

            for (var i = 0; i < _batchCount; i++) {
                _batches[i].Life = -1.0f;
                _batches[i].CameraDistance = -1.0f;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)( 4 * sizeof(float) * _mesh.Vertices.Length), _mesh.Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesPositionsBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_batchCount * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);
        }

        public void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesPositionsBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(_batchCount * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(_batchCount * sizeof(float) * 4), _particulePostitionSizeData);

            _particleShader.Bind();

            _particleShader.UpdateUniforms(new Transform(), _material, renderingEngine);

            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);


            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesPositionsBuffer);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);


            GL.VertexAttribDivisor(0, 0);
            GL.VertexAttribDivisor(1, 1);

            GL.DrawArraysInstanced(BeginMode.TriangleStrip, 0, 4, _batchCount);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);

            GL.VertexAttribDivisor(0, 0);
            GL.VertexAttribDivisor(1, 0);
        }

        public float GetRandomNumber(double minimum, double maximum) {
            return (float)(_random.NextDouble() * (maximum - minimum) + minimum);
        }

        public int BatchCount
        {
            get { return _batchCount; }
            set
            {
                _batchCount = value;
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

#region getter/setter

#endregion
    }
}
