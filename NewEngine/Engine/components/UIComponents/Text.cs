using System;
using System.Drawing;
using System.Drawing.Imaging;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.components.UIComponents {
    public class Text : UiComponent, IDisposable {
        private Shader _imageShader;
        private Material _material;
        private Mesh _mesh;
        private RectTransform _rectTransform;
        private string _text;
        private Bitmap _textBmp;

        public Text(RectTransform rect, string text, int samples) {
            _rectTransform = rect;
            _text = text;
            _material = new Material(null);
            _imageShader = new Shader("UI/UIText");
            UpdateMesh();
        }


        public new Transform Transform {
            set {
                Parent.Transform = value;
                UpdateMesh();
            }
            get { return Parent.Transform; }
        }

        public string text {
            set {
                _text = value;
                UpdateText();
            }
            get { return _text; }
        }



        public float Biggest(float a, float b) {
            return a > b ? a : b;
        }

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine) {
            Parent.Transform = _rectTransform;
            // we only need the ImageShader here so replace the shader being passed

            var t = (RectTransform) Transform;

            t.Position = new Vector3(t.Position);

            _imageShader.Bind();
            _imageShader.UpdateUniforms(t, _material, renderingEngine);

            Begin2D();

            _mesh.Draw();

            End2D();
        }

        private void UpdateMesh() {
            UpdateText();
            var texCoordY = 1.0f -
                            1/Biggest(_rectTransform.Size.X, _rectTransform.Size.Y)*
                            (Biggest(_rectTransform.Size.X, _rectTransform.Size.Y) - _rectTransform.Size.Y);
            var texCoordX = 1.0f -
                            1/Biggest(_rectTransform.Size.X, _rectTransform.Size.Y)*
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

        private void UpdateText() {
            _textBmp = new Bitmap(512, 512);
            using (var gfx = Graphics.FromImage(_textBmp)) {
                gfx.Clear(Color.Transparent);
                gfx.DrawString(_text, new Font(FontFamily.GenericSerif, 30, FontStyle.Regular),
                    new SolidBrush(Color.Red), 0, 0);
            }
            _textBmp.Save("image.png", ImageFormat.Png);
            _material = new Material(new Texture(_textBmp));
        }


        private void Begin2D() {
            GL.Viewport(0, 0, (int) CoreEngine.GetWidth(), (int) CoreEngine.GetHeight());
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
        }

        private void End2D() {
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing)
                _textBmp.Dispose();
        }
    }
}