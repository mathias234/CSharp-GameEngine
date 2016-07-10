using System;
using System.Collections.Generic;
using System.IO;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.MeshLoading;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.Rendering {
    public class Mesh : IDisposable {
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
                LogManager.Debug(filename);
                LoadMesh(filename);
                _loadedModels.Add(filename, _resource);
            }
        }

        public Mesh(Vertex[] vertices, int[] indices) {
            _resource = new MeshResource();

            AddVertices(vertices, indices, true);
        }


        public Mesh(Vertex[] vertices, int[] indices, bool calcNormals) {
            _resource = new MeshResource();

            _filename = "";

            AddVertices(vertices, indices, calcNormals);
        }


        private void AddVertices(Vertex[] vertices, int[] indices, bool calcNormals) {
            if (calcNormals) {
                CalculateNormals(vertices, indices);
            }

            _resource.Size = indices.Length;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * (Vertex.Size * 4)), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _resource.Ibo);

            int[] reversedIndices = (int[])indices.Clone();

            Array.Reverse(reversedIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_resource.Size * 4 /* size of int */), reversedIndices, BufferUsageHint.StaticDraw);
        }

        public void Draw() {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.Vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.Size * 4, 12);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.Size * 4, 20);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _resource.Ibo);

            GL.DrawElements(BeginMode.Triangles, _resource.Size, DrawElementsType.UnsignedInt, 0);

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

            ObjModel test = new ObjModel(Path.Combine("./res/models", filename));
            IndexedModel model = test.ToIndexedModel();
            model.CalculateNormals();

            List<Vertex> vertices = new List<Vertex>();
            for (int i = 0; i < model.Positions.Count; i++) {
                vertices.Add(new Vertex(model.Positions[i], model.TexCoords[i], model.Normals[i]));
            }


            AddVertices(vertices.ToArray(), Util.FromNullableIntArray(model.Indices.ToArray()), false);
        }

        public void Dispose() {
            if (_resource.RemoveReference() && _filename != "") {
                _loadedModels.Remove(_filename);
            }
        }
    }
}
