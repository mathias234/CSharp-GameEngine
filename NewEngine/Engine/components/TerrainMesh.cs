using System.Collections.Generic;
using System.Drawing;
using System.IO;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.CollisionShapes;
using NewEngine.Engine.Core;
using NewEngine.Engine.Physics;
using NewEngine.Engine.Rendering;
using NewEngine.Engine.Rendering.Shading;
using OpenTK;

namespace NewEngine.Engine.components {
    public class TerrainMesh : GameComponent {
        private Dictionary<string, Shader> _loadedShaders = new Dictionary<string, Shader>();
        private Shader _baseShader;

        private float _dispOffset;
        private float _dispScale;
        private int _height;

        private float[] _heights;
        private float _heigtmapStrength;
        private int _imageHeight;

        private int _imageWidth;
        private Texture _layer1;
        private Texture _layer2;
        private Mesh _mesh;
        private float _specularIntensity;
        private float _specularPower;

        private Texture _tex1;
        private Texture _tex1Nrm;
        private Texture _tex2;
        private Texture _tex2Nrm;
        private Texture _tex3;
        private Texture _tex3Nrm;

        private Material _material;

        private int _width;

        public TerrainMesh(string heightmapTextureFilename, int width, int height, float heigtmapStrength, string tex1,
            string tex1Nrm, string tex2, string tex2Nrm, string layer1, string tex3, string tex3Nrm, string layer2,
            float specularIntensity = 0.5f, float specularPower = 32.0f, float dispScale = 0.0f, float dispOffset = 0.0f) {
            if (File.Exists(Path.Combine("./res/textures", heightmapTextureFilename)) == false) {
                LogManager.Error("Terrain Mesh: Heightmap does not exists");
            }

            _specularIntensity = specularIntensity;
            _specularPower = specularPower;
            _dispScale = dispScale;
            _dispOffset = dispOffset;
            _width = width;
            _height = height;
            _heigtmapStrength = heigtmapStrength;


            _tex1 = new Texture(tex1);
            _tex1Nrm = new Texture(tex1Nrm);
            _tex2 = new Texture(tex2);
            _tex2Nrm = new Texture(tex2Nrm);
            _layer1 = new Texture(layer1);
            _tex3 = new Texture(tex3);
            _tex3Nrm = new Texture(tex3Nrm);
            _layer2 = new Texture(layer2);

            _baseShader = new Shader("terrain/baseTerrain");

            var image = new Bitmap(Path.Combine("./res/textures", heightmapTextureFilename));

            var lbmap = new LockBitmap(image);
            lbmap.LockBits();

            _heights = new float[image.Height*image.Width];

            for (var j = 0; j < image.Height; j++) {
                for (var i = 0; i < image.Width; i++) {
                    _heights[i + j*image.Width] = lbmap.GetPixel(i, j).R;
                }
            }

            _imageHeight = image.Height;
            _imageWidth = image.Width;

            lbmap.UnlockBits();

            image.Dispose();

            UpdateMesh();
        }

        public void UpdateMesh() {
            var verts = new List<Vertex>();
            var tris = new List<int>();

            //Bottom left section of the map, other sections are similar
            for (var i = 0; i < _imageWidth; i++) {
                for (var j = 0; j < _imageHeight; j++) {
                    //Add each new vertex in the plane
                    verts.Add(
                        new Vertex(
                            new Vector3((float) i/_imageWidth*_width, _heights[i + j*_imageWidth]*_heigtmapStrength,
                                (float) j/_imageHeight*_height),
                            new Vector2((float) i/_imageWidth, (float) j/_imageHeight)));

                    //Skip if a new square on the plane hasn't been formed
                    if (i == 0 || j == 0) continue;
                    //Adds the index of the three vertices in order to make up each of the two tris
                    tris.Add(_imageWidth*i + j); //Top right
                    tris.Add(_imageWidth*i + j - 1); //Bottom right
                    tris.Add(_imageWidth*(i - 1) + j - 1); //Bottom left - First triangle
                    tris.Add(_imageWidth*(i - 1) + j - 1); //Bottom left 
                    tris.Add(_imageWidth*(i - 1) + j); //Top left
                    tris.Add(_imageWidth*i + j); //Top right - Second triangle
                }
            }

            BEPUutilities.Vector3[] vertsVec3 = new BEPUutilities.Vector3[verts.Count];

            int g = 0;
            foreach (var vertex in verts) {
                vertsVec3[g] = new BEPUutilities.Vector3( vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
                g++;
            }

            // using physics slows down the game once the physics interaction starts by a lot because a terrain has alot of vertices this is only temporary, a mesh will be split up into chunks for better prefomance later
            PhysicsEngine.AddToPhysicsEngine(new StaticMesh(vertsVec3, tris.ToArray()));

            _material = new Material(_tex1, _specularIntensity, _specularPower, _tex1Nrm);
            _material.SetTexture("tex2", _tex2);
            _material.SetTexture("tex2Nrm", _tex2Nrm);
            _material.SetTexture("layer1", _layer1);
            _material.SetTexture("tex3", _tex3);
            _material.SetTexture("tex3Nrm", _tex3Nrm);
            _material.SetTexture("layer2", _layer2);
            _material.SetFloat("dispMapScale", _dispScale);
            _material.SetFloat("dispMapBias", _dispOffset);


            _mesh = new Mesh(verts.ToArray(), tris.ToArray(), true);
        }

        public override void Render(string shader, string shaderType, float deltaTime, RenderingEngine renderingEngine, string renderStage) {
            Shader shaderToUse;

            if (shaderType == "base") {
                shaderToUse = _baseShader;
            }
            else if (shaderType != "light" && shaderType != "shadowMap") {
                return;
            }
            else if (_loadedShaders.ContainsKey(shader)) {
                shaderToUse = _loadedShaders[shader];
            }
            else {
                shaderToUse = new Shader("terrain/terrain-" + shader);
                _loadedShaders.Add(shader, shaderToUse);
            }


            shaderToUse.Bind();
            shaderToUse.UpdateUniforms(Transform, _material, renderingEngine);
            _mesh.Draw();
        }

        public override void AddToEngine(CoreEngine engine) {
            base.AddToEngine(engine);

            engine.RenderingEngine.AddNonBatched(gameObject);
        }

        public override void OnDestroyed(CoreEngine engine) {
            base.OnDestroyed(engine);

            engine.RenderingEngine.RemoveNonBatched(gameObject);
        }
    }
}