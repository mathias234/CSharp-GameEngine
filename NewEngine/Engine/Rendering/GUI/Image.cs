using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.components;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.Rendering.GUI {
    public class Image : GameComponent {

        public Image(Texture texture) {
            Texture = texture;

            if (ImageMesh == null) {
                // create the image mesh. a 1x1 square
                Vertex[] vertices = {
                    new Vertex(new Vector3(-1, -1, 0), new Vector2(1, 0)),
                    new Vertex(new Vector3(-1, 1, 0), new Vector2(1, 1)),
                    new Vertex(new Vector3(1, 1, 0), new Vector2(0, 1)),
                    new Vertex(new Vector3(1, -1, 0), new Vector2(0, 0))
                };

                int[] indices = {
                    2, 0, 1,
                    3, 0, 2
                };

                ImageMesh = Mesh.GetMesh(vertices, indices, true);
            }

            Material = new Material(Shader.GetShader("UI/UIImage"));
            Material.SetMainTexture(texture);
            Material.SetVector4("color", Vector4.One);

        }

        public Texture Texture { get; set; }
        public Material Material { get; set; }
        public static Mesh ImageMesh;


        public override void Render(string shader, string shaderType, float deltaTime, BaseRenderingEngine renderingEngine, string renderStage) {
            if (!Material.Shader.GetShaderTypes.Contains(shaderType))
                return;

            Material.Shader.Bind(shaderType);
            Material.Shader.UpdateUniforms(Transform, Material, renderingEngine, shaderType);
            ImageMesh.Draw();
        }

        public override void AddToEngine(ICoreEngine engine) {
            engine.GUIRenderingEngine.AddToEngine(this);
        }

        public override void OnDestroyed(ICoreEngine engine) {
        }
    }
}
