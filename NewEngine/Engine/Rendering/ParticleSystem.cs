using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

    public class ParticleSystem {
        public int Amount { get; set; }

        int _billboardVertexBuffer;
        int _particlesPositionBuffer;
        int _particlesColorBuffer;

        private Particle[] particles;

        private static float[] particulePostitionSizeData;
        private static float[] particuleColorData;

        private Shader particleShader;

        public ParticleSystem(int amount) {
            Amount = amount;

            particleShader = new Shader("particles");

            particulePostitionSizeData = new float[amount * 4];
            particuleColorData = new float[amount * 4];
            particles = new Particle[amount];

            for (int i = 0; i < amount; i++) {
                particles[i].life = -1.0f;
                particles[i].cameraDistance = -1.0f;
            }


            float[] vertexBufferData = new[] {
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                -0.5f, 0.5f, 0.0f,
                0.5f, 0.5f, 0.0f
            };


            GL.GenBuffers(1, out _billboardVertexBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _billboardVertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * vertexBufferData.Length), vertexBufferData, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, out _particlesPositionBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesPositionBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Amount * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);

            GL.GenBuffers(1, out _particlesColorBuffer);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Amount * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);


        }

        public void Draw(RenderingEngine renderingEngine, float deltaTime) {


            int newParticles = (int)(deltaTime * 10000.0f);
            if (newParticles > (int)(0.016f * 10000.0f)) {
                newParticles = (int)(0.015f * 10000.0f);
            }

            for (int i = 0; i < newParticles; i++) {

                int particleIndex = FindUnusedParticle();
                particles[particleIndex].life = GetRandomNumber(0, 5);
                particles[particleIndex].position = new Vector3(GetRandomNumber(0, 5), GetRandomNumber(0,5), GetRandomNumber(0, 5));

                float spread = 1.5f;
                Vector3 mainDir = new Vector3(0, 0, 2);


                Vector3 randomDir = new Vector3(
                    GetRandomNumber(-1, 1),
                    GetRandomNumber(-1, 1),
                    GetRandomNumber(0.9, 1)
                );

                particles[particleIndex].speed = mainDir + randomDir * spread;

                float color = GetRandomNumber(0.3f, 1);


                particles[particleIndex].r = color;
                particles[particleIndex].g = color / 1.2f;
                particles[particleIndex].b = color / 2;
                particles[particleIndex].a = 1;

                particles[particleIndex].size = GetRandomNumber(5, 10) + 0.1f;
            }

            int particleCount = 0;

            for (int i = 0; i < Amount; i++) {
                Particle p = particles[i];

                if (p.life > 0.0f) {
                    p.life -= deltaTime;

                    if (p.life > 0.0f) {
                        p.speed = new Vector3(0.0f, 1.2f, 0.0f) * deltaTime * 0.5f;
                        p.position += p.speed * deltaTime;

                        p.cameraDistance =
                            (p.position - CoreEngine.GetCoreEngine.RenderingEngine.MainCamera.Transform.Position)
                                .LengthSquared;

                        particulePostitionSizeData[4 * particleCount + 0] = p.position.X;
                        particulePostitionSizeData[4 * particleCount + 1] = p.position.Y;
                        particulePostitionSizeData[4 * particleCount + 2] = p.position.Z;
                        particulePostitionSizeData[4 * particleCount + 3] = p.size;

                        particuleColorData[4 * particleCount + 0] = p.r;
                        particuleColorData[4 * particleCount + 1] = p.g;
                        particuleColorData[4 * particleCount + 2] = p.b;
                        particuleColorData[4 * particleCount + 3] = p.a;


                    }
                    else {
                        p.cameraDistance = -1.0f;
                    }
                    particles[i] = p;

                    particleCount++;
                }
            }

            SortParticles();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesPositionBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Amount * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(particleCount * sizeof(float) * 4), particulePostitionSizeData);


            GL.BindBuffer(BufferTarget.ArrayBuffer, _particlesColorBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Amount * 4 * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(particleCount * sizeof(float) * 4), particuleColorData);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            particleShader.Bind();
            Material material = new Material(new Texture("ParticleAtlas.png"));
            material.SetTexture("cutoutMask", new Texture("ParticleAtlas_cutout.png"));
            material.SetVector3("CameraRight_worldspace", -renderingEngine.MainCamera.Transform.Right);
            material.SetVector3("CameraUp_worldspace", renderingEngine.MainCamera.Transform.Up);

            particleShader.UpdateUniforms(new Transform(), material, renderingEngine);

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
        }

        private void SortParticles() {
            BubbleSort(particles);
        }

        public static void BubbleSort(Particle[] a) {
            for (int i = 1; i <= a.Length - 1; ++i)
                for (int j = 0; j < a.Length - i; ++j)
                    if (a[j].cameraDistance < a[j + 1].cameraDistance)
                        Swap(ref a[j], ref a[j + 1]);

        }

        public static void Swap(ref Particle x, ref Particle y) {
            Particle temp = x;
            x = y;
            y = temp;
        }

        private int lastUsedParticle = 0;

        int FindUnusedParticle() {
            for (int i = lastUsedParticle; i < Amount; i++) {
                if (particles[i].life < 0) {
                    lastUsedParticle = i;
                    return i;
                }
            }

            for (int i = 0; i < lastUsedParticle; i++) {
                if (particles[i].life < 0) {
                    lastUsedParticle = i;
                    return i;
                }
            }

            return 0;
        }

        static Random random = new Random();

        public float GetRandomNumber(double minimum, double maximum) {
            return (float)(random.NextDouble() * (maximum - minimum) + minimum);
        }
    }
}
