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
        private Vertex[] _vertices;
        private int[] _indices;

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
            CalculateTangents(vertices, indices);

            _vertices = vertices;
            _indices = indices;

            _resource = new MeshResource();

            AddVertices(vertices, indices, true);
        }


        public Mesh(Vertex[] vertices, int[] indices, bool calcNormals) {
            CalculateTangents(vertices, indices);

            _vertices = vertices;
            _indices = indices;

            _resource = new MeshResource();

            _filename = "";

            AddVertices(vertices, indices, calcNormals);
        }

        public Vertex[] Vertices
        {
            get { return _vertices; }
        }

        public int[] Indices
        {
            get { return _indices; }
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
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, Vector3.SizeInBytes + Vector2.SizeInBytes + Vector3.SizeInBytes);

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



        public void CalculateTangents(Vertex[] vertices, int[] indices) {
            for (var i = 0; i < indices.Length; i += 3) {
                int i0 = indices[i];
                int i1 = indices[i + 1];
                int i2 = indices[i + 2];


                Vector3 edge1 = vertices[i1].Position - vertices[i0].Position;
                Vector3 edge2 = vertices[i2].Position - vertices[i0].Position;


                float deltaU1 = vertices[i1].TexCoord.X - vertices[i0].TexCoord.X;
                float deltaV1 = vertices[i1].TexCoord.Y - vertices[i0].TexCoord.Y;
                float deltaU2 = vertices[i2].TexCoord.X - vertices[i0].TexCoord.X;
                float deltaV2 = vertices[i2].TexCoord.Y - vertices[i0].TexCoord.Y;

                float dividend = (deltaU1 * deltaV2 - deltaU2 * deltaV1);
                //TODO: The first 0.0f may need to be changed to 1.0f here.
                float f = dividend == 0 ? 0.0f : 1.0f / dividend;

                Vector3 tangent = new Vector3(0, 0, 0);
                tangent.X = (f * (deltaV2 * edge1.X - deltaV1 * edge2.X));
                tangent.Y = (f * (deltaV2 * edge1.Y - deltaV1 * edge2.Y));
                tangent.Z = (f * (deltaV2 * edge1.Z - deltaV1 * edge2.Z));

                vertices[i0].Tangent = (vertices[i0].Tangent + tangent);
                vertices[i1].Tangent = (vertices[i1].Tangent + tangent);
                vertices[i2].Tangent = (vertices[i2].Tangent + tangent);
            }

            for (int i = 0; i < vertices.Length; i++) {
                vertices[i].Tangent = vertices[i].Tangent.Normalized();
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

                    _vertices = vertices.ToArray();
                    _indices = Util.FromNullableIntArray(model.Indices.ToArray());


                    AddVertices(_vertices, _indices, false);
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
