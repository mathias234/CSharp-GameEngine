using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.MeshLoading.Obj;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class Mesh {
        private static Dictionary<string, MeshResource> _loadedModels = new Dictionary<string, MeshResource>();
        private MeshResource _resource;
        private string _filename;

        public Mesh(string filename) {
            _filename = filename;
            if (_loadedModels.ContainsKey(filename)) {
                _resource = _loadedModels[filename];
                _resource.AddReference();
            }
            else {
                _resource = new MeshResource();
                LoadMesh(filename);
                _loadedModels.Add(filename, _resource);
            }
        }

        ~Mesh() {
            LogManager.Debug("removing mesh : " + _filename);
            if (_resource != null && _resource.RemoveReference()) {
                if (_filename != null) {
                    _loadedModels.Remove(_filename);
                }
            }
        }

        public Mesh(Vertex[] vertices, int[] indices) {
            CalculateTangents(vertices, indices);

            Vertices = vertices;
            Indices = indices;

            _resource = new MeshResource();

            AddVertices(vertices, indices, true);
        }


        public Mesh(Vertex[] vertices, int[] indices, bool calcNormals) {
            Vertices = vertices;
            Indices = indices;

            _resource = new MeshResource();

            AddVertices(vertices, indices, calcNormals);
        }

        public Vertex[] Vertices { get; private set; }

        public int[] Indices { get; private set; }

        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        private void AddVertices(Vertex[] vertices, int[] indices, bool calcNormals) {
            if (calcNormals) {
                CalculateNormals(vertices, indices);
            }

            _resource.Size = indices.Length;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.SizeInBytes), vertices,
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _resource.Ibo);

            var reversedIndices = (int[])indices.Clone();

            Array.Reverse(reversedIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_resource.Size * 4 /* size of int */),
                reversedIndices, BufferUsageHint.StaticDraw);
        }

        public void Draw() {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.Vbo);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes,
                Vector3.SizeInBytes + Vector2.SizeInBytes);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes,
                Vector3.SizeInBytes + Vector2.SizeInBytes + Vector3.SizeInBytes);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _resource.Ibo);
            GL.DrawElements(BeginMode.Triangles, _resource.Size, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
        }

        public void DrawInstanced(int amount) {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);
            GL.EnableVertexAttribArray(5);
            GL.EnableVertexAttribArray(6);
            GL.EnableVertexAttribArray(7);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.Vbo);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes + Vector2.SizeInBytes);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes + Vector2.SizeInBytes + Vector3.SizeInBytes);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.MatrixBuffer);

            GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 64, 0);  // c0
            GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, 64, 16); // c1
            GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, 64, 32); // c2
            GL.VertexAttribPointer(7, 4, VertexAttribPointerType.Float, false, 64, 48); // c3

            GL.VertexAttribDivisor(4, 1);
            GL.VertexAttribDivisor(5, 1);
            GL.VertexAttribDivisor(6, 1);
            GL.VertexAttribDivisor(7, 1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _resource.Ibo);

            GL.DrawElementsInstanced(BeginMode.Triangles, _resource.Size, DrawElementsType.UnsignedInt, IntPtr.Zero, amount);

            GL.VertexAttribDivisor(4, 0);
            GL.VertexAttribDivisor(5, 0);
            GL.VertexAttribDivisor(6, 0);
            GL.VertexAttribDivisor(7, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
            GL.DisableVertexAttribArray(7);
        }

        private void CalculateNormals(Vertex[] vertices, int[] indices) {
            for (var i = 0; i < indices.Length; i += 3) {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var v1 = vertices[i1].Position - vertices[i0].Position;
                var v2 = vertices[i2].Position - vertices[i0].Position;

                var normal = Vector3.Cross(v1, v2);
                normal.Normalize();

                vertices[i0].Normal = vertices[i0].Normal + normal;
                vertices[i1].Normal = vertices[i1].Normal + normal;
                vertices[i2].Normal = vertices[i2].Normal + normal;
            }

            for (var i = 0; i < vertices.Length; i++) {
                vertices[i].Normal = vertices[i].Normal.Normalized();
            }
        }


        public void CalculateTangents(Vertex[] vertices, int[] indices) {
            for (var i = 0; i < indices.Length; i += 3) {
                var i0 = indices[i];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];


                var edge1 = vertices[i1].Position - vertices[i0].Position;
                var edge2 = vertices[i2].Position - vertices[i0].Position;


                var deltaU1 = vertices[i1].TexCoord.X - vertices[i0].TexCoord.X;
                var deltaV1 = vertices[i1].TexCoord.Y - vertices[i0].TexCoord.Y;
                var deltaU2 = vertices[i2].TexCoord.X - vertices[i0].TexCoord.X;
                var deltaV2 = vertices[i2].TexCoord.Y - vertices[i0].TexCoord.Y;

                var dividend = deltaU1 * deltaV2 - deltaU2 * deltaV1;
                //TODO: The first 0.0f may need to be changed to 1.0f here.
                var f = Math.Abs(dividend) < 0.001f ? 0.0f : 1.0f / dividend;

                var tangent = new Vector3(0, 0, 0);
                tangent.X = f * (deltaV2 * edge1.X - deltaV1 * edge2.X);
                tangent.Y = f * (deltaV2 * edge1.Y - deltaV1 * edge2.Y);
                tangent.Z = f * (deltaV2 * edge1.Z - deltaV1 * edge2.Z);

                vertices[i0].Tangent = vertices[i0].Tangent + tangent;
                vertices[i1].Tangent = vertices[i1].Tangent + tangent;
                vertices[i2].Tangent = vertices[i2].Tangent + tangent;
            }

            for (var i = 0; i < vertices.Length; i++) {
                vertices[i].Tangent = vertices[i].Tangent.Normalized();
            }
        }

        private void LoadMesh(string filename) {
            var splitArray = filename.Split('.');
            var ext = splitArray[splitArray.Length - 1];

            LogManager.Debug("Loading Mesh: " + filename);

            switch (ext) {
                case "obj":
                    var test = new ObjModel(Path.Combine("./res/models", filename));
                    var model = test.ToIndexedModel();

                    Vertices = model.Positions.Select((t, i) => new Vertex(t, model.TexCoords[i], model.Normals[i], model.Tangents[i])).ToArray();
                    Indices = Util.FromNullableIntArray(model.Indices.ToArray());


                    AddVertices(Vertices, Indices, false);
                    break;
                case "fbx":
                    //var fbxModel = new FbxModel(Path.Combine("./res", "models", filename));
                    break;
                default:
                    LogManager.Error("Error: File format not supported for mesh data: " + ext);
                    break;
            }
        }

        public void BindBatch(Matrix4[] matrices, int amount) {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.MatrixBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(64 * amount), matrices, BufferUsageHint.StaticDraw);
        }
    }
}