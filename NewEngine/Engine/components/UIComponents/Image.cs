﻿using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace NewEngine.Engine.components.UIComponents {
    public class Image : UiComponent {
        private RectTransform _rectTransform;
        private Texture _texture;
        private Material _material;
        private Mesh _mesh;
        private Shader imageShader;

        public Image(RectTransform rect, Texture texture) {
            _rectTransform = rect;
            _texture = texture;
            _material = new Material(_texture);
            imageShader = new Shader("UI/UIImage");

            UpdateMesh();

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

            RectTransform t = (RectTransform)Transform;

            t.Position = new Vector3(t.Position);

            imageShader.Bind();
            imageShader.UpdateUniforms(t, _material, renderingEngine);

            Begin2D();

            _mesh.Draw();

            End2D();
        }

        void UpdateMesh() {
            float texCoordY = 1.0f - (1 / Biggest(_rectTransform.size.X, _rectTransform.size.Y) * (Biggest(_rectTransform.size.X, _rectTransform.size.Y) - _rectTransform.size.Y));
            float texCoordX = 1.0f - (1 / Biggest(_rectTransform.size.X, _rectTransform.size.Y) * (Biggest(_rectTransform.size.X, _rectTransform.size.Y) - _rectTransform.size.X));

            // the Texture coords work, but they might be changed to look better? idk all this flipping and stuff seems wrong TODO: Fixme
            _mesh = new Mesh(new[] {
                new Vertex(new Vector3(-_rectTransform.size.X,-_rectTransform.size.Y,0) + _rectTransform.Position, new Vector2(-texCoordX,texCoordY)),
                new Vertex(new Vector3(-_rectTransform.size.X,_rectTransform.size.Y,0) + _rectTransform.Position, new Vector2(-texCoordX,0)),
                new Vertex(new Vector3(_rectTransform.size.X,_rectTransform.size.Y,0) + _rectTransform.Position, new Vector2(0,0)),
                new Vertex(new Vector3(_rectTransform.size.X,-_rectTransform.size.Y,0) + _rectTransform.Position, new Vector2(0,texCoordY)),
            }, new[] {
                2,3,0,
                0,1,2,
            }, false);
        }



        public new Transform Transform {
            set {
                Parent.Transform = value;
                UpdateMesh();
            }
            get { return Parent.Transform; }
        }

        void Begin2D() {
            GL.Viewport(0, 0, (int)CoreEngine.GetWidth(), (int)CoreEngine.GetHeight());
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
        }
        void End2D() {
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.CullFace);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);
        }
    }
}

