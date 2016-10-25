using System.Drawing;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.components.UIComponents {
    public class Image : UiComponent {
        private Material _material;
        private Mesh _mesh;
        public RectTransform _rectTransform; //TODO: LD36 Hack
        private Shader _imageShader;
        private Texture _defaultTexture;

        public Image(RectTransform rect, Color color, Texture texture) {
            _defaultTexture = Texture.GetTexture("default_mask.png");
            _rectTransform = rect;

            _imageShader = Shader.GetShader("UI/UIImage");

            _material = new Material(_imageShader);

            if (texture == null)
                _material.SetMainTexture(_defaultTexture);
            else
                _material.SetMainTexture(texture);

            _material.SetVector4("color", new Vector4((float)color.R / 256, (float)color.G / 256, (float)color.B / 256, (float)color.A / 256));


            UpdateMesh();
        }


        public new Transform Transform
        {
            set
            {
                gameObject.Transform = value;
                UpdateMesh();
            }
            get { return gameObject.Transform; }
        }

        public float Biggest(float a, float b) {
            return a > b ? a : b;
        }

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            if (gameObject.IsActive == false)
                return;

            if (renderStage == "ui") {
                gameObject.Transform = _rectTransform;
                // we only need the ImageShader here so replace the shader being passed

                var t = (RectTransform)Transform;

                t.Position = new Vector3(t.Position);

                _imageShader.Bind();
                _imageShader.UpdateUniforms(t, _material, renderingEngine, renderStage);

                Begin2D();

                _mesh.Draw();

                End2D();
            }
        }

        private void UpdateMesh() {
            _mesh = Mesh.GetMesh(new[] {
                new Vertex(new Vector3(-1, -1, 0),
                    new Vector2(-1, 1)),
                new Vertex(new Vector3(-1, 1, 0) ,
                    new Vector2(-1, 0)),
                new Vertex(new Vector3(1, 1, 0),
                    new Vector2(0, 0)),
                new Vertex(new Vector3(1, -1, 0) ,
                    new Vector2(0, 1))
            }, new[] {
                2, 3, 0,
                0, 1, 2
            }, false);
        }

        private void Begin2D() {
            GL.Viewport(0, 0, (int)CoreEngine.GetWidth(), (int)CoreEngine.GetHeight());
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        private void End2D() {
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
        }


        public override void AddToEngine(ICoreEngine engine) {
            base.AddToEngine(engine);
            engine.RenderingEngine.AddUI(this);
        }

        public override void OnDestroyed(ICoreEngine engine) {
            base.OnDestroyed(engine);
            engine.RenderingEngine.RemoveUI(this);
        }
    }
}