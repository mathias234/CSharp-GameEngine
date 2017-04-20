using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Fonts;
using NewEngine.Engine.Rendering.Shading;
using OpenTK.Graphics.OpenGL;

namespace NewEngine.Engine.components {
    public class TextMaster : GameComponent {
        private int _vao;
        private static Dictionary<FontType, List<GUIText>> _texts = new Dictionary<FontType, List<GUIText>>();

        public static void LoadText(GUIText text) {
            FontType font = text.Font;
            TextMeshData data = font.LoadText(text);
            int vao = LoadToVAO(data.VertexPositions, data.TextureCoords);
            text.SetMeshInfo(vao, data.VertexCount);

            List<GUIText> textBatch = null;
            if (_texts.ContainsKey(font))
                textBatch = _texts[font];

            if (textBatch == null) {
                textBatch = new List<GUIText>();
                _texts.Add(font, textBatch);
            }

            textBatch.Add(text);
        }

        public static void Render() {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);
            foreach (var font in _texts.Keys) {

                foreach (var guiText in _texts[font]) {
                    var mat = new Material(Shader.GetShader("font"));
                    mat.SetMainTexture(font.TextureAtlas);
                    mat.SetVector3("color", guiText.Color);
                    mat.SetVector2("translation", guiText.Position);
                    mat.Shader.Bind("default");
                    mat.Shader.UpdateUniforms(new Transform(), mat, CoreEngine.GetCoreEngine.RenderingEngine, "default");
                    RenderText(guiText, mat);
                }
            }

        }

        public static void RenderText(GUIText text, Material mat) {
            GL.BindVertexArray(text.Mesh);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.DrawArrays(PrimitiveType.Triangles, 0, text.VertexCount);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }


        public static void RemoveText(GUIText text) {
            List<GUIText> textBatch = _texts[text.Font];
            textBatch.Remove((text));
            if (textBatch.Count <= 0) {
                _texts.Remove(text.Font);
            }
        }

        public static int LoadToVAO(float[] positions, float[] textureCoords) {
            int vaoId;
            GL.GenVertexArrays(1, out vaoId);
            GL.BindVertexArray(vaoId);

            StoreDataInAttrib(0, 2, positions);
            StoreDataInAttrib(1, 2, textureCoords);

            GL.BindVertexArray(0);

            return vaoId;
        }

        private static void StoreDataInAttrib(int attributeNumber, int coordinateSize, float[] data) {
            int vboId;
            GL.GenBuffers(1, out vboId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * 4), data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attributeNumber, coordinateSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
