using System.Collections;
using Data.PerlinNoise;
using Microsoft.Xna.Framework;

namespace Data.Voxel.Map {
    public class Map {
        private byte[,,] _voxels;
        private int _width;
        private int _height;
        private int _depth;
        private float _scale = 1f / 8;

        // this will always be the zero position of the world
        private int _digDepth = 0;

        private Vector3 _chunkPosition;
        private int _seed;
        private float _heightFactor;

        public Map(int seed, Vector3 chunkPosition, int width, int height, int depth, float heightFactor, int digDepth, float noiseScale) {
            _chunkPosition = chunkPosition;
            _voxels = new byte[width, height, depth];
            _width = width;
            _height = height;
            _digDepth = digDepth;
            _depth = depth;
            _scale = noiseScale;
            _heightFactor = heightFactor;
            _seed = seed;
            for (var x = 0; x < _width; x++) {
                for (var y = 0; y < _height; y++) {
                    for (var z = 0; z < _depth; z++) {
                        // generate the chunk in layers to get more flexability
                        GenerateBaseTerrain(x, y, z);
                        GenerateLakes(x, y, z);
                        GenerateUnderground(x, y, z);
                    }
                }
            }
        }

        public void GenerateBaseTerrain(int x, int y, int z) {
            // generate base terrain

            _voxels[x, y, z] =0;

            float xCoord = (_chunkPosition.X + _seed + x / (float)_width) / _scale;
            float yCoord = (_chunkPosition.Y + y / (float)_height) / _scale;
            float zCoord = (_chunkPosition.Z + _seed + z / (float)_depth) / _scale;

            int curHeight = (int)(Noise.GetOctaveNoise(xCoord, yCoord, zCoord, 1) * _heightFactor) +_digDepth;


            int yPos = y;

            if (yPos == curHeight)
                _voxels[x, y, z] = 1;
            if (yPos > curHeight)
                _voxels[x, y, z] = 0;
            if (yPos < curHeight) {
                _voxels[x, y, z] = 2;
            }
        }

        public void GenerateLakes(int x, int y, int z) {
            if (y <= _digDepth+2 && y >= _digDepth && GetVoxel(x,y,z) == 0) {
                _voxels[x, y, z] = 3;
            }
        }

        public void GenerateUnderground(int x, int y, int z) {

        }

        public void SetVoxel(int x, int y, int z, byte value) {
            _voxels[x, y, z] = value;
        }

        public byte GetVoxel(int x, int y, int z) {
            if (x >= _width || x < 0 || y >= _height || y < 0 || z >= _depth || z < 0) {
                // to optimize the mesh more we dont render all the chunk edges, just the once on the top( to avoid any wierd bugs )
                if (y >= _digDepth - 5)
                    return 0;

                return 1;
            }

            return _voxels[x, y, z];
        }
    }
}
