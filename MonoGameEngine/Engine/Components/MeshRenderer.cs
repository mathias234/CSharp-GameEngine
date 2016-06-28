using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameEngine.Engine.Components {
    public class MeshRenderer : Component, IDisposable {
        private Mesh _mesh;

        public Mesh Mesh
        {
            set
            {
                _mesh = value;
                VertexPositionNormalTexture[] vpntTemp = new VertexPositionNormalTexture[_mesh.Vertices.Length];
                for (int i = 0; i < _mesh.Vertices.Length; i++) {
                    if (_mesh.Normals == null || _mesh.Normals.Length < _mesh.Vertices.Length) {
                        _mesh.Normals = new Vector3[_mesh.Vertices.Length];
                    }

                    if (_mesh.Uvs == null || _mesh.Uvs.Length < _mesh.Vertices.Length) {
                        _mesh.Uvs = new Vector2[_mesh.Vertices.Length];
                    }


                    vpntTemp[i] = (new VertexPositionNormalTexture(_mesh.Vertices[i], _mesh.Normals[i], _mesh.Uvs[i]));
                }
                if (vpntTemp.Length == 0)
                    return;

                //Vert buffer
                _vertexBuffer = new VertexBuffer(CoreEngine.instance.GraphicsDevice, typeof(VertexPositionNormalTexture), vpntTemp.Length, BufferUsage.WriteOnly);
                _vertexBuffer.SetData(vpntTemp);

                //Vert buffer
                _indexBuffer = new IndexBuffer(CoreEngine.instance.GraphicsDevice, typeof(short), _mesh.Indices.Length, BufferUsage.WriteOnly);
                _indexBuffer.SetData(_mesh.Indices);
            }
            get { return _mesh; }

        }

        private Color _color;

        public Color Color
        {
            set
            {
                _color = value;
                if (BasicEffect != null)
                    BasicEffect.DiffuseColor = new Vector3((float)value.R / 255, (float)value.G / 255, (float)value.B / 255);
            }
            get { return _color; }

        }

        public Texture2D _texture;

        public Texture2D Texture
        {
            set
            {
                _texture = value;
                if (BasicEffect != null) {
                    if (value != null) {
                        BasicEffect.TextureEnabled = true;
                        BasicEffect.Texture = value;
                    }
                }
            }
            get { return _texture; }

        }

        private BasicEffect _basicEffect;

        [XmlIgnore]
        private BasicEffect BasicEffect
        {
            set { _basicEffect = value; }
            get {
                if (_basicEffect == null) {
                    _basicEffect = new BasicEffect(CoreEngine.instance.GraphicsDevice);
                    _basicEffect.EnableDefaultLighting();
                }
                return _basicEffect;
            }
        }

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;


        public MeshRenderer() { }

        /// <summary>
        /// Should only be called by the renderer
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public override void Draw(GraphicsDevice graphicsDevice) {
            if (Mesh == null) {
                Debug.WriteLine("No Camera");
                return;
            }
            if (GameObject.FindGameObjectOfType<Camera>() == null) {
                Debug.WriteLine("No Camera");
                return;
            }
            if (Mesh.Vertices.Length == 0) {
                Debug.WriteLine("NO VERTICES TO RENDER!!");
                return;
            }

            BasicEffect.Projection = Camera.Main.ProjectionMatrix;

            BasicEffect.View = GameObject.FindGameObjectOfType<Camera>().ViewMatrix;
            BasicEffect.World = GameObject.Transform.WorldMatrix;

            CoreEngine.instance.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            CoreEngine.instance.GraphicsDevice.Indices = _indexBuffer;

            foreach (var pass in BasicEffect.CurrentTechnique.Passes) {
                CoreEngine.instance.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

                pass.Apply();

                CoreEngine.instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Mesh.Vertices.Length * 2);
            }
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        protected virtual void Dispose(bool dispossing) {
            _mesh = null;
            _basicEffect.Dispose();
            _indexBuffer.Dispose();
            _vertexBuffer.Dispose();
        }
    }
}
