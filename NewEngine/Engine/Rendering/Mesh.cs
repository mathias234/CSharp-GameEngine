using System;
using System.Collections.Generic;
using System.IO;
using NewEngine.Engine.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class Mesh {
        private int _vbo;
        private int _ibo;
        private int _size;

        public Mesh(string fileName) {
            GL.GenBuffers(1, out _vbo);
            GL.GenBuffers(1, out _ibo);
            _size = 0;

            LoadMesh(fileName);
        }

        public Mesh(Vertex[] vertices, int[] indices) {
            InitMeshData();

            AddVertices(vertices, indices, true);
        }


        public Mesh(Vertex[] vertices, int[] indices, bool calcNormals) {
            InitMeshData();

            AddVertices(vertices, indices, calcNormals);
        }

        public void InitMeshData() {
            GL.GenBuffers(1, out _vbo);
            GL.GenBuffers(1, out _ibo);
            _size = 0;

        }

        private void AddVertices(Vertex[] vertices,  int[] indices, bool calcNormals) {
            if (calcNormals) {
                CalculateNormals(vertices, indices);
            }

            _size = indices.Length;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * (Vertex.Size * 4)), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            Array.Reverse(indices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_size * 4 /* size of int */), indices, BufferUsageHint.StaticDraw);
        }

        public void Draw() {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.Size * 4, 12);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 20);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);

            GL.DrawElements(BeginMode.Triangles, _size, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
        }

        private void CalculateNormals(Vertex[] vertices, int[] indices) {
            for (var i = 0; i < indices.Length; i += 3) {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                Vector3 v1 = vertices[i1].Position - vertices[i0].Position;
                Vector3 v2 = vertices[i2].Position - vertices[i0].Position;

                Vector3 normal = Vector3.Cross(v1, v2);
                normal.Normalize();

                vertices[i0].Normal = vertices[i0].Normal + normal;
                vertices[i1].Normal = vertices[i1].Normal + normal;
                vertices[i2].Normal = vertices[i2].Normal + normal;
            }

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i].Normal = vertices[i].Normal.Normalized();
            }
        }


        private void LoadMesh(string filename) {
            string[] splitArray = filename.Split('.');
            string ext = splitArray[splitArray.Length - 1];

            if (ext != "obj") {
                LogManager.Error("Error: File format not supported for mesh data: " + ext);
            }

            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();

            try {
                var reader = new StreamReader(Path.Combine("./res/models", filename));
                string line;

                while ((line = reader.ReadLine()) != null) {
                    string[] tokens = line.Split(' ');
                    tokens = Util.RemoveEmptyStrings(tokens);

                    switch (tokens[0]) {
                        case "v":
                            vertices.Add(
                                new Vertex(new Vector3(tokens[1].ParseInvariantFloat(), tokens[2].ParseInvariantFloat(), tokens[3].ParseInvariantFloat())));
                            break;
                        case "f":
                            indices.Add(int.Parse(tokens[1].Split('/')[0]) - 1);
                            indices.Add(int.Parse(tokens[2].Split('/')[0]) - 1);
                            indices.Add(int.Parse(tokens[3].Split('/')[0]) - 1);

                            if (tokens.Length > 4) {
                                indices.Add(int.Parse(tokens[1].Split('/')[0]) - 1);
                                indices.Add(int.Parse(tokens[3].Split('/')[0]) - 1);
                                indices.Add(int.Parse(tokens[4].Split('/')[0]) - 1);
                            }
                            break;
                    }
                }
            }
            catch (Exception e) {
                LogManager.Error("Failed to load model " + e.Message + " at " + Path.Combine("./res/models", filename));
                LogManager.Error(e.StackTrace);
            }

            AddVertices(vertices.ToArray(), indices.ToArray(), false);
        }
    }
}
