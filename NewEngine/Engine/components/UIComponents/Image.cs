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
        private RectTransform _rectTransform;
        private Shader _imageShader;
        private Texture _defaultTexture;

        public Image(RectTransform rect, Color color, Texture texture) {
            _defaultTexture = new Texture("default_mask.png");
            _rectTransform = rect;

            if(texture == null)
                _material = new Material(_defaultTexture);
            else
                _material = new Material(texture);

            _material.SetVector4("color", new Vector4((float)color.R/256, (float)color.G / 256, (float)color.B / 256, (float)color.A / 256));

            _imageShader = new Shader("UI/UIImage");

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

            if (renderStage == "ui") {

                gameObject.Transform = _rectTransform;
                // we only need the ImageShader here so replace the shader being passed

                var t = (RectTransform) Transform;

                t.Position = new Vector3(t.Position);

                _imageShader.Bind();
                _imageShader.UpdateUniforms(t, _material, renderingEngine);

                Begin2D();

                _mesh.Draw();

                End2D();
            }
        }

        private void UpdateMesh() {
            var texCoordY = 1.0f -
                            1 / Biggest(_rectTransform.Size.X, _rectTransform.Size.Y) *
                            (Biggest(_rectTransform.Size.X, _rectTransform.Size.Y) - _rectTransform.Size.Y);
            var texCoordX = 1.0f -
                            1 / Biggest(_rectTransform.Size.X, _rectTransform.Size.Y) *
                            (Biggest(_rectTransform.Size.X, _rectTransform.Size.Y) - _rectTransform.Size.X);

            // the Texture coords work, but they might be changed to look better? idk all this flipping and stuff seems wrong TODO: Fixme
            _mesh = new Mesh(new[] {
                new Vertex(new Vector3(-_rectTransform.Size.X, -_rectTransform.Size.Y, 0) + _rectTransform.Position,
                    new Vector2(-texCoordX, texCoordY)),
                new Vertex(new Vector3(-_rectTransform.Size.X, _rectTransform.Size.Y, 0) + _rectTransform.Position,
                    new Vector2(-texCoordX, 0)),
                new Vertex(new Vector3(_rectTransform.Size.X, _rectTransform.Size.Y, 0) + _rectTransform.Position,
                    new Vector2(0, 0)),
                new Vertex(new Vector3(_rectTransform.Size.X, -_rectTransform.Size.Y, 0) + _rectTransform.Position,
                    new Vector2(0, texCoordY))
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


        public override void AddToEngine(CoreEngine engine) {
            base.AddToEngine(engine);
            engine.RenderingEngine.AddUI(this);
        }

        public override void OnDestroyed(CoreEngine engine) {
            base.OnDestroyed(engine);
            engine.RenderingEngine.RemoveUI(this);
        }
    }
}