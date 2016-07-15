using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using NewEngine.Engine.Rendering.Shading.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace NewEngine.Engine.components.UIComponents {
    //TODO Implement texture rendering!
    public class Image : UiComponent {
        private RectTransform _rectTransform;
        private Texture _texture;
        private Material _material;
        private Mesh _mesh;

        public Image(RectTransform rect, Texture texture) {
            _rectTransform = rect;
            _texture = texture;
            _material = new Material(_texture);

            float texCoordY = 1.0f- (1/Biggest(rect.size.X, rect.size.Y) * (Biggest(rect.size.X, rect.size.Y) - rect.size.Y));
            float texCoordX = 1.0f - (1 / Biggest(rect.size.X, rect.size.Y) * (Biggest(rect.size.X, rect.size.Y) - rect.size.X));

            _mesh = new Mesh(new[] {
                new Vertex(new Vector3(-rect.size.X,-rect.size.Y,0), new Vector2(0,0)),
                new Vertex(new Vector3(rect.size.X,-rect.size.Y,0), new Vector2(texCoordX,0)),
                new Vertex(new Vector3(rect.size.X,rect.size.Y,0), new Vector2(texCoordX,texCoordY)),
                new Vertex(new Vector3(-rect.size.X,rect.size.Y,0), new Vector2(0,texCoordY)),
            }, new[] {
                0,1,2,
                2,3,0
            });
        }

        public float Biggest(float a, float b) {
            if (a > b)
                return a;
            else {
                return b;
            }
        }

        public override void Render(Shader shader, RenderingEngine renderingEngine) {
            Parent.Transform = _rectTransform;
            // we only need the ImageShader here so replace the shader being passed
            UIImage imageShader = UIImage.Instance;

            RectTransform t = (RectTransform)Transform;
            
            t.Position = new Vector3(t.Position);

            imageShader.Bind();
            imageShader.UpdateUniforms(t, _material, renderingEngine);

            Begin2D();

            _mesh.Draw();

            End2D();
        }

        void Begin2D() {
            GL.Viewport(0, 0, (int)CoreEngine.GetWidth(), (int)CoreEngine.GetHeight());

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, CoreEngine.GetWidth(), CoreEngine.GetHeight(), 0, -1.0, 1.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);
            GL.Disable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
        }
        void End2D() {
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
        }
    }
}

