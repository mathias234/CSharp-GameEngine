using System;
using System.Diagnostics;
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
                //Vert buffer
                _vertexBuffer = new VertexBuffer(CoreEngine.instance.GraphicsDevice, typeof(VertexPositionNormalTexture), _mesh.Vertices.Length, BufferUsage.WriteOnly);
                _vertexBuffer.SetData(_mesh.Vertices);

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
                if (_basicEffect != null)
                    _basicEffect.DiffuseColor = new Vector3((float)value.R / 255, (float)value.G / 255, (float)value.B / 255);
            }
            get { return _color; }

        }

        [XmlIgnore]
        private BasicEffect _basicEffect;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;


        public MeshRenderer() { }

        public override void Init() {
            base.Init();
            _basicEffect = new BasicEffect(CoreEngine.instance.GraphicsDevice);
            _basicEffect.Alpha = 1f;

            _basicEffect.EnableDefaultLighting();
            _basicEffect.DiffuseColor = new Vector3((float)Color.R / 255, (float)Color.G / 255, (float)Color.B / 255);

            if (GameObject.FindGameObjectOfType<Camera>() == null) {
                Debug.WriteLine("No Camera");
                return;
            }
        }

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


            _basicEffect.Projection = Camera.Main.ProjectionMatrix;

            _basicEffect.View = GameObject.FindGameObjectOfType<Camera>().ViewMatrix;
            _basicEffect.World = GameObject.Transform.WorldMatrix;

            CoreEngine.instance.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            CoreEngine.instance.GraphicsDevice.Indices = _indexBuffer;

            foreach (var pass in _basicEffect.CurrentTechnique.Passes) {
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
