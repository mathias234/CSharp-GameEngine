using System;
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

namespace NewEngine.Engine.components
{
    public enum DrawBrush
    {
        Circle,
    }
    public class TerrainMesh : GameComponent
    {
        private float _dispOffset;
        private float _dispScale;

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

        private int _height;
        private int _width;

        private Vector3 _worldPos;
        private Terrain _terrainCollider;

        public TerrainMesh(float[] heights, int width, int height, Vector3 worldPos)
        {
            _worldPos = worldPos;
            _heights = heights;
            _imageHeight = height;
            _imageWidth = width;

            _tex1 = Texture.GetTexture("bricks.png");
            _tex1Nrm = Texture.GetTexture("bricks.png");
            _tex2 = Texture.GetTexture("bricks.png");
            _tex2Nrm = Texture.GetTexture("bricks.png");
            _layer1 = Texture.GetTexture("bricks.png");
            _tex3 = Texture.GetTexture("bricks.png");
            _tex3Nrm = Texture.GetTexture("bricks.png");
            _layer2 = Texture.GetTexture("bricks.png");

            UpdateMesh();
        }

        public TerrainMesh(string heightmapTextureFilename, int width, int height, float heigtmapStrength, string tex1,
            string tex1Nrm, string tex2, string tex2Nrm, string layer1, string tex3, string tex3Nrm, string layer2,
            float specularIntensity = 0.5f, float specularPower = 32.0f, float dispScale = 0.0f, float dispOffset = 0.0f)
        {
            if (File.Exists(Path.Combine("./res/textures", heightmapTextureFilename)) == false)
            {
                LogManager.Error("Terrain Mesh: Heightmap does not exists");
            }

            _worldPos = new Vector3(0, 0, 0);
            _specularIntensity = specularIntensity;
            _specularPower = specularPower;
            _dispScale = dispScale;
            _dispOffset = dispOffset;
            _width = width;
            _height = height;
            _heigtmapStrength = heigtmapStrength;


            _tex1 = Texture.GetTexture(tex1);
            _tex1Nrm = Texture.GetTexture(tex1Nrm);
            _tex2 = Texture.GetTexture(tex2);
            _tex2Nrm = Texture.GetTexture(tex2Nrm);
            _layer1 = Texture.GetTexture(layer1);
            _tex3 = Texture.GetTexture(tex3);
            _tex3Nrm = Texture.GetTexture(tex3Nrm);
            _layer2 = Texture.GetTexture(layer2);

            var image = new Bitmap(Path.Combine("./res/textures", heightmapTextureFilename));

            var lbmap = new LockBitmap(image);
            lbmap.LockBits();

            _heights = new float[image.Height * image.Width];

            for (var j = 0; j < image.Height; j++)
            {
                for (var i = 0; i < image.Width; i++)
                {
                    _heights[i + j * image.Width] = lbmap.GetPixel(i, j).R;
                }
            }

            _imageHeight = image.Height;
            _imageWidth = image.Width;

            lbmap.UnlockBits();

            image.Dispose();

            UpdateMesh();
        }

        public float[] GetHeightmap()
        {
            return _heights;
        }

        public void DrawOnTerrain(DrawBrush brush, float posX, float posY, int size, float strength)
        {
            Console.WriteLine(posX + ":" + posY);
            int idx = 0;
            for (var x = 0; x < _imageWidth + 1; x++)
            {
                for (var y = 0; y < _imageHeight + 1; y++)
                {
                    var distance = new Vector3(posX, 0, posY).Distance(new Vector3(x, 0, y));
                    if (distance <= size)
                    {

                        float falloff = size - (float)distance;
                        _heights[idx] += falloff * strength;
                    }

                    idx++;
                }
            }

            UpdateMesh();
        }

        /// <summary>
        /// This is almost double the speed as the old one
        /// </summary>
        public void UpdateMesh()
        {
            var tris = new int[(_imageWidth * _imageHeight) * 6];

            _imageHeight += 1;
            _imageWidth += 1;
            var verts = new Vertex[_imageWidth * _imageHeight];

            int idx = 0;
            int indicesIdx = 0;
            for (var i = 0; i < _imageWidth; i++)
            {
                for (var j = 0; j < _imageHeight; j++)
                {
                    verts[idx] = (
                        new Vertex(
                            new Vector3(
                                 j - _imageHeight / 2.0f,
                                _heights[i + j * _imageWidth],
                                i - _imageWidth / 2.0f),

                            new Vector2(i, j)));

                    idx++;

                    if (i == 0 || j == 0) continue;
                    tris[indicesIdx] = (_imageWidth * i + j); //Top right
                    tris[indicesIdx + 1] = (_imageWidth * i + j - 1); //Bottom right
                    tris[indicesIdx + 2] = (_imageWidth * (i - 1) + j - 1); //Bottom left - First triangle
                    tris[indicesIdx + 3] = (_imageWidth * (i - 1) + j - 1); //Bottom left 
                    tris[indicesIdx + 4] = (_imageWidth * (i - 1) + j); //Top left
                    tris[indicesIdx + 5] = (_imageWidth * i + j); //Top right - Second triangle

                    indicesIdx += 6;
                }
            }

            if (_mesh != null)
            {
                // Update Mesh
                _mesh.UpdateMesh(verts, tris, true);

                float[,] heights3d = new float[_imageWidth, _imageHeight];

                int g = 0;
                for (var i = 0; i < _imageWidth; i++)
                {
                    for (var j = 0; j < _imageHeight; j++)
                    {
                        heights3d[i, j] = _heights[g];
                        g++;
                    }
                }


                var transform = new BEPUutilities.AffineTransform(
                    new BEPUutilities.Vector3(
                       _worldPos.X - _imageWidth / 2,
                        _worldPos.Y,
                        _worldPos.Z - _imageHeight / 2
                        ));


                _terrainCollider.Shape.Heights = heights3d;

            }
            else
            {
                _mesh = Mesh.GetMesh(verts, tris, true);

                float[,] heights3d = new float[_imageWidth, _imageHeight];

                int g = 0;
                for (var i = 0; i < _imageWidth; i++)
                {
                    for (var j = 0; j < _imageHeight; j++)
                    {
                        heights3d[i, j] = _heights[g];
                        g++;
                    }
                }


                var transform = new BEPUutilities.AffineTransform(
                    new BEPUutilities.Vector3(
                       _worldPos.X - _imageWidth / 2,
                        _worldPos.Y,
                        _worldPos.Z - _imageHeight / 2
                        ));


                _terrainCollider = new Terrain(heights3d, transform);


                PhysicsEngine.AddToPhysicsEngine(_terrainCollider);

                _material = new Material(Shader.GetShader("terrain/terrain"));
                _material.SetMainTexture(_tex1);
                _material.SetFloat("specularIntensity", _specularIntensity);
                _material.SetFloat("specularPower", _specularPower);
                _material.SetTexture("normalMap", _tex1Nrm);
                _material.SetTexture("tex2", _tex2);
                _material.SetTexture("tex2Nrm", _tex2Nrm);
                _material.SetTexture("layer1", _layer1);
                _material.SetTexture("tex3", _tex3);
                _material.SetTexture("tex3Nrm", _tex3Nrm);
                _material.SetTexture("layer2", _layer2);
                _material.SetFloat("dispMapScale", _dispScale);
                _material.SetFloat("dispMapBias", _dispOffset);
            }

            _imageHeight -= 1;
            _imageWidth -= 1;
        }

        public override void Render(string shader, string shaderType, float deltaTime, BaseRenderingEngine renderingEngine, string renderStage)
        {
            if (!_material.Shader.GetShaderTypes.Contains(shaderType))
                return;

            _material.Shader.Bind(shaderType);
            _material.Shader.UpdateUniforms(Transform, _material, renderingEngine, shaderType);
            _mesh.Draw();
        }

        public override void AddToEngine(ICoreEngine engine)
        {
            base.AddToEngine(engine);

            engine.RenderingEngine.AddToEngine(this);
        }
    }
}