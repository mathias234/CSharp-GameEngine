using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEngine.Engine.Core;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class TerrainMesh : GameComponent {
        private Mesh _mesh;
        private string _splatmapTextureFilename;
        private string _splatmapNormalTextureFilename;
        private string _splatmapDisplacementTextureFilename;
        private float _specularIntensity;
        private float _specularPower;
        private float _dispScale;
        private float _dispOffset;


        public TerrainMesh(string heightmapTextureFilename,  int width, int height, float heigtmapStrength, string splatmapTextureFilename, string splatmapNormalTextureFilename = "default_normal.png", string splatmapDisplacementTextureFilename = "default_disp.png", float specularIntensity = 0.5f, float specularPower = 32.0f, float dispScale = 0.0f, float dispOffset = 0.0f) {
            if (File.Exists(Path.Combine("./res/textures", heightmapTextureFilename)) == false) {
                LogManager.Error("Terrain Mesh: Heightmap does not exists");
            }
            if (File.Exists(Path.Combine("./res/textures", splatmapTextureFilename)) == false) {
                LogManager.Error("Terrain Mesh: Splatmap does not exists");
            }

            _splatmapTextureFilename = splatmapTextureFilename;
            _splatmapNormalTextureFilename = splatmapNormalTextureFilename;
            _splatmapDisplacementTextureFilename = splatmapDisplacementTextureFilename;
            _specularIntensity = specularIntensity;
            _specularPower = specularPower;
            _dispScale = dispScale;
            _dispOffset = dispOffset;

            Bitmap image = new Bitmap(Path.Combine("./res/textures", heightmapTextureFilename));

            LockBitmap lbmap = new LockBitmap(image);
            lbmap.LockBits();

            List<Vertex> verts = new List<Vertex>();
            List<int> tris = new List<int>();

            //Bottom left section of the map, other sections are similar
            for (int i = 0; i < image.Width; i++) {
                for (int j = 0; j < image.Height; j++) {
                    //Add each new vertex in the plane
                    verts.Add(new Vertex(new Vector3((float)i / image.Width * width, lbmap.GetPixel(i, j).R * heigtmapStrength, (float)j / image.Height * height), 
                        new Vector2((float)i / image.Width , (float)j / image.Height )));

                    //Skip if a new square on the plane hasn't been formed
                    if (i == 0 || j == 0) continue;
                    //Adds the index of the three vertices in order to make up each of the two tris
                    tris.Add(image.Width * i + j); //Top right
                    tris.Add(image.Width * i + j - 1); //Bottom right
                    tris.Add(image.Width * (i - 1) + j - 1); //Bottom left - First triangle
                    tris.Add(image.Width * (i - 1) + j - 1); //Bottom left 
                    tris.Add(image.Width * (i - 1) + j); //Top left
                    tris.Add(image.Width * i + j); //Top right - Second triangle
                }
            }


            _mesh = new Mesh(verts.ToArray(), tris.ToArray(), true);
            lbmap.UnlockBits();

            image.Dispose();
        }

        public override void OnEnable() {
            base.OnEnable();
        }

        public override void Render(Shader shader, RenderingEngine renderingEngine) {
            shader.Bind();
            shader.UpdateUniforms(new Transform(), new Material(new Texture(_splatmapTextureFilename), _specularIntensity, _specularPower, new Texture(_splatmapNormalTextureFilename), new Texture(_splatmapDisplacementTextureFilename), _dispScale, _dispOffset), renderingEngine);
            _mesh.Draw();
        }
    }
}
