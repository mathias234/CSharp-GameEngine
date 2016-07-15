using System;
using System.Collections.Generic;
using System.IO;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.MeshLoading;
using NewEngine.Engine.Rendering.ResourceManagament;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using NewEngine.Engine.Rendering.MeshLoading.FBX;

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

        public Mesh(Vertex[] vertices, int[] indices) {
            _resource = new MeshResource();

            AddVertices(vertices, indices, true);
        }


        public Mesh(Vertex[] vertices, int[] indices, bool calcNormals) {
            _resource = new MeshResource();

            _filename = "";

            AddVertices(vertices, indices, calcNormals);
        }

        ~Mesh() {
            if (_resource.RemoveReference() && _filename != "") {
                _loadedModels.Remove(_filename);
            }
        }

        private void AddVertices(Vertex[] vertices, int[] indices, bool calcNormals) {
            if (calcNormals) {
                CalculateNormals(vertices, indices);
            }

            _resource.Size = indices.Length;

            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Vertex.SizeInBytes), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _resource.Ibo);

            int[] reversedIndices = (int[])indices.Clone();

            Array.Reverse(reversedIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_resource.Size * 4 /* size of int */), reversedIndices, BufferUsageHint.StaticDraw);
        }

        public void Draw() {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _resource.Vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes + Vector2.SizeInBytes);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes + Vector2.SizeInBytes + Vector3.SizeInBytes );

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _resource.Ibo);

            GL.DrawElements(BeginMode.Triangles, _resource.Size, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
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


        public void CalculateTangents(Vertex[] vertices) {
            for (var i = 0; i < vertices.Length; i += 3) {
                Vector3 v0 = vertices[i + 0].Position;

                Vector3 v1 = Vector3.One;
                if (i + 1 < vertices.Length)
                    v1 = vertices[i + 1].Position;

                Vector3 v2 = Vector3.One;
                if (i + 2 < vertices.Length)
                    v2 = vertices[i + 2].Position;

                Vector2 uv0 = vertices[i].TexCoord;

                Vector2 uv1 = Vector2.One;
                if (i + 1 < vertices.Length)
                    uv1 = vertices[i+1].TexCoord;

                Vector2 uv2 = Vector2.One;
                if (i + 2 < vertices.Length)
                    uv2 = vertices[i+2].TexCoord;

                Vector3 deltaPos1 = v1 - v0;
                Vector3 deltaPos2 = v2 - v0;

                Vector2 deltaUV1 = uv1 - uv0;
                Vector2 deltaUV2 = uv2 - uv0;

                float r = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV1.Y * deltaUV2.X);
                Vector3 tangent = (deltaPos1 * deltaUV2.Y - deltaPos2 * deltaUV1.Y) * r;
                Vector3 bitangent = (deltaPos2 * deltaUV1.X - deltaPos1 * deltaUV2.X) * r;
                vertices[i].Tangent = tangent;
                if (i + 1 < vertices.Length)
                    vertices[i+1].Tangent = tangent;
                if (i + 2 < vertices.Length)
                    vertices[i+2].Tangent = tangent;
            }
        }

        private void LoadMesh(string filename) {
            string[] splitArray = filename.Split('.');
            string ext = splitArray[splitArray.Length - 1];

            LogManager.Debug("Loading Mesh: " + filename);

            switch (ext) {
                case "obj":
                    ObjModel test = new ObjModel(Path.Combine("./res/models", filename));
                    IndexedModel model = test.ToIndexedModel();

                    List<Vertex> vertices = new List<Vertex>();
                    for (int i = 0; i < model.Positions.Count; i++) {
                        vertices.Add(new Vertex(model.Positions[i], model.TexCoords[i], model.Normals[i], model.Tangents[i]));
                    }

                    AddVertices(vertices.ToArray(), Util.FromNullableIntArray(model.Indices.ToArray()), false);
                    break;
                case "fbx":
                    FbxModel fbxModel = new FbxModel(Path.Combine("./res", "models", filename));
                    break;
                default:
                    LogManager.Error("Error: File format not supported for mesh data: " + ext);
                    break;
            }

        }
    }
}
