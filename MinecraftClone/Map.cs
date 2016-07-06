using System;
using System.Collections;
using System.Diagnostics;
using Data.SimplexNoise;
using NewEngine.Engine.Core;
using OpenTK;

namespace Data.Voxel.Map {
    public enum BlockTypes {
        Air,
        Grass,
        Stone,
        Water,
        Lava,
        Sand,
        Diamond
    }

    public class Map {
        private BlockTypes[,,] _voxels;
        private int _width;
        private int _height;
        private int _depth;
        private float _scale;

        // this will always be the zero position of the world
        private int _digDepth = 0;

        private Vector3 _chunkPosition;
        private int _seed;
        private float _heightFactor;


        public Map(int seed, Vector3 chunkPosition, int width, int height, int depth, float heightFactor, int digDepth, float noiseScale) {
            _chunkPosition = chunkPosition;
            _voxels = new BlockTypes[width, height, depth];
            _width = width;
            _height = height;
            _digDepth = digDepth;
            _depth = depth;
            _scale = noiseScale;
            _heightFactor = heightFactor;
            _seed = seed;

            // terrain pass
            for (var x = 0; x < _width; x++) {
                for (var y = 0; y < _height; y++) {
                    for (var z = 0; z < _depth; z++) {
                        GenerateBaseTerrain(x, y, z);
                    }
                }
            }


            // lake + grass pass
            for (var x = 0; x < _width; x++) {
                for (var y = 0; y < _height; y++) {
                    for (var z = 0; z < _depth; z++) {
                        GenerateGrassLayer(x, y, z);
                        GenerateLakes(x, y, z);
                    }
                }
            }

            // Beaches pass
            for (var x = 0; x < _width; x++) {
                for (var y = 0; y < _height; y++) {
                    for (var z = 0; z < _depth; z++) {
                        GenerateBeaches(x, y, z);
                    }
                }
            }



            // Overhang pass
            for (var x = 0; x < _width; x++) {
                for (var y = 0; y < _height; y++) {
                    for (var z = 0; z < _depth; z++) {
                        GenerateOverhang(x, y, z);
                        GenerateDiamonds(x, y, z);
                    }
                }
            }
        }

        public void GenerateBaseTerrain(int x, int y, int z) {
            // generate base terrain

            float xCoord = ((_chunkPosition.X + _seed + x / (float)_width) / _scale) - _width;
            float yCoord = ((_chunkPosition.Y + _seed + y / (float)_height) / _scale) - _height;
            float zCoord = ((_chunkPosition.Z + _seed + z / (float)_depth) / _scale) - _depth;

            int curHeight = (int)(Noise.GetNoise(xCoord, yCoord, zCoord) * _heightFactor) + _digDepth;

            int yPos = y;

            if (yPos < curHeight) {
                _voxels[x, y, z] = BlockTypes.Stone;
            }
        }

        public void GenerateGrassLayer(int x, int y, int z) {
            if (GetVoxel(x, y + 1, z) == BlockTypes.Air && GetVoxel(x, y, z) == BlockTypes.Stone) {
                _voxels[x, y, z] = BlockTypes.Grass;
            }
        }

        public void GenerateLakes(int x, int y, int z) {
            if (y <= _digDepth + 2 && y >= _digDepth && GetVoxel(x, y, z) == BlockTypes.Air) {
                _voxels[x, y, z] = BlockTypes.Water;
            }
        }

        public void GenerateBeaches(int x, int y, int z) {
            if (y <= _digDepth + 2 && y >= _digDepth &&
                (GetVoxel(x, y, z) != BlockTypes.Air && GetVoxel(x, y, z) != BlockTypes.Water)
                && (GetVoxel(x, y + 1, z) == BlockTypes.Air || GetVoxel(x, y + 1, z) == BlockTypes.Water)) {
                _voxels[x, y, z] = BlockTypes.Sand;
            }
        }

        public void GenerateOverhang(int x, int y, int z) {
            if (y >= 30 + _digDepth)
                return;

            float xCoord = ((_chunkPosition.X + _seed + x / (float)_width)) - _width;
            float yCoord = ((_chunkPosition.Y + _seed + y / (float)_height) * 30) - _height;
            float zCoord = ((_chunkPosition.Z + _seed + z / (float)_depth)) - _depth;

            int curHeight = (int)(Noise.GetNoise(xCoord, yCoord, zCoord) * _heightFactor);

            if (curHeight < 2) {
                if (_voxels[x, y, z] != BlockTypes.Sand && _voxels[x, y, z] != BlockTypes.Water)
                    _voxels[x, y, z] = BlockTypes.Air;
            }
        }


        public void GenerateDiamonds(int x, int y, int z) {
            if (y >= 30 + _digDepth)
                return;

            float xCoord = ((_chunkPosition.X + _seed + 2391 + x / (float)_width) * 2) - _width;
            float yCoord = ((_chunkPosition.Y + _seed + 2391 + y / (float)_height) * 300) - _height;
            float zCoord = ((_chunkPosition.Z + _seed + 2391 + z / (float)_depth) * 2) - _depth;

            float curHeight = (Noise.GetNoise(xCoord, yCoord, zCoord) * _heightFactor);

            if (curHeight < 0.3f && y <= 10) {
                GenerateCluster(3, x, y, z, BlockTypes.Diamond);
            }
        }

        public void GenerateCluster(int clusterSize, int x, int y, int z, BlockTypes type) {
            int r = clusterSize;

            Random random = new Random(_seed * x * y * z);
            for (int tx = -r; tx < r + 1; tx++) {
                for (int ty = -r; ty < r + 1; ty++) {
                    for (int tz = -r; tz < r + 1; tz++) {
                        if (Math.Sqrt(Math.Pow(tx, 2) + Math.Pow(ty, 2) + Math.Pow(tz, 2)) <= r - 2) {
                            if (random.Next(0, 30) >= 6) { }
                            else
                                try {
                                    _voxels[tx + r + x, ty + r + y, tz + r + z] = type;
                                }
                                catch {
                                }
                        }
                    }
                }
            }
        }



        public void SetVoxel(int x, int y, int z, BlockTypes type) {
            try {
                _voxels[x, y, z] = type;
            }
            catch (IndexOutOfRangeException ex) {
                Debug.WriteLine(ex.ToString());
            }
        }

        public BlockTypes GetVoxel(int x, int y, int z) {
            if (x >= _width || x < 0 || y >= _height || y < 0 || z >= _depth || z < 0) {
                // to optimize the mesh more we dont render all the chunk edges, just the once on the top( to avoid any wierd bugs )
                return BlockTypes.Air;
            }

            return _voxels[x, y, z];
        }

    }
}
