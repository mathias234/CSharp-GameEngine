using System;
using System.Collections;
using System.Diagnostics;
using Data.SimplexNoise;
using Microsoft.Xna.Framework;

namespace Data.Voxel.Map {
    public enum BlockTypes {
        Air,
        Grass,
        Stone,
        Water,
        Lava,
        Sand,
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
        }

        public void GenerateBaseTerrain(int x, int y, int z) {
            // generate base terrain

            _voxels[x, y, z] = 0;

            float xCoord = ((_chunkPosition.X + _seed + x / (float)_width) / _scale) - _width;
            float yCoord = ((_chunkPosition.Y + _seed + y / (float)_height) / _scale) - _height;
            float zCoord = ((_chunkPosition.Z + _seed + z / (float)_depth) / _scale) - _depth;

            int curHeight = (int)(Noise.GetNoise(xCoord, yCoord, zCoord) * _heightFactor) + _digDepth;

            int yPos = y;

            if (yPos == curHeight)
                _voxels[x, y, z] = BlockTypes.Grass;
            if (yPos > curHeight)
                _voxels[x, y, z] = BlockTypes.Air;
            if (yPos < curHeight) {
                _voxels[x, y, z] = BlockTypes.Stone;
            }

        }

        public void GenerateLakes(int x, int y, int z) {
            if (y <= _digDepth + 2 && y >= _digDepth && GetVoxel(x, y, z) == 0) {
                _voxels[x, y, z] = BlockTypes.Water;
            }
        }

        public void GenerateBeaches(int x, int y, int z) {
            if (y <= _digDepth + 2 && y >= _digDepth &&
                (GetVoxel(x, y, z) != BlockTypes.Air && GetVoxel(x,y,z) != BlockTypes.Water)
                && (GetVoxel(x, y + 1, z) == BlockTypes.Air || GetVoxel(x,y+1,z) == BlockTypes.Water)) {
                _voxels[x, y, z] = BlockTypes.Sand;
            }
        }


        public void SetVoxel(int x, int y, int z, BlockTypes type) {
            _voxels[x, y, z] = type;
        }

        public BlockTypes GetVoxel(int x, int y, int z) {
            if (x >= _width || x < 0 || y >= _height || y < 0 || z >= _depth || z < 0) {
                // to optimize the mesh more we dont render all the chunk edges, just the once on the top( to avoid any wierd bugs )
                if (y >= _digDepth - 5)
                    return 0;

                return BlockTypes.Stone;
            }

            return _voxels[x, y, z];
        }

    }
}
