using NewEngine.Engine.components;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewEngine.Engine.Core
{
    /// <summary>
    /// Uses Filemanger to load a chuck file then transform it into a drawable chuck
    /// </summary>
    public class DrawableChunk
    {
        public static int ChunkSizeX = 64;
        public static int ChunkSizeY = 64;
        public bool Loaded;

        private FileSystem.ChunkFile _chunkFile;

        private List<GameObject> _gameObjects;
        private GameObject _chunkTerrain;
        private int _zoneId;
        private int _chunkX;
        private int _chunkY;

        public DrawableChunk(int zoneId, int x, int y)
        {
            _zoneId = zoneId;
            _chunkX = x;
            _chunkY = y;

            _chunkFile = FileSystem.FileManager.GetFile<FileSystem.ChunkFile>("./res/zone_" + zoneId + "/chunk_" + x + "_" + y);

            if (_chunkFile == null)
                return;

            _gameObjects = new List<GameObject>();

            var rootPos = new Vector3(
                     _chunkX * ChunkSizeX,
                        0,
                     _chunkY * ChunkSizeY);


            _chunkTerrain = new GameObject("chunk plane");

            _chunkTerrain.Transform.Position = new OpenTK.Vector3(
                (ChunkSizeX / 2) + rootPos.X,
                0,
                (ChunkSizeY / 2) + rootPos.Z);

            _chunkTerrain.AddComponent(new TerrainMesh(_chunkFile.TerrainHeightmap, ChunkSizeX, ChunkSizeY, _chunkTerrain.Transform.Position));
            //chunkTerrain.Transform.Rotation = new Quaternion(0, 180, 0);

            foreach (var chunkObj in _chunkFile.GameObjects)
            {

                var gObj = new GameObject(chunkObj.Name);
                gObj.Transform.Position = new OpenTK.Vector3(chunkObj.X + rootPos.X, chunkObj.Y, chunkObj.Z + rootPos.X);

                gObj.AddComponent(new MeshRenderer("cube.obj", "null"));

                _gameObjects.Add(gObj);
            }

            Loaded = true;
        }

        public void DrawOnTerrain(DrawBrush brush, float posX, float posY, int size, float strength)
        {
            _chunkTerrain.GetComponent<TerrainMesh>().DrawOnTerrain(brush, posX, posY, size, strength);
        }


        public void AddObject(GameObject obj)
        {

            _gameObjects.Add(obj);
        }

        public void Draw(ICoreEngine engine)
        {
            // transform the objects back to their positions

            _chunkTerrain.AddToEngine(engine);

            foreach (var gameObject in _gameObjects)
            {
                gameObject.AddToEngine(engine);
            }

        }

        public void Save()
        {
            _chunkFile.TerrainHeightmap = _chunkTerrain.GetComponent<TerrainMesh>().GetHeightmap();

            foreach (var child in _gameObjects)
            {
                FileSystem.GameObject gObj = new FileSystem.GameObject();

                gObj.Name = child.Name;
                gObj.X = child.Transform.Position.X % ChunkSizeX;
                gObj.Y = child.Transform.Position.Y;
                gObj.Z = child.Transform.Position.Z % ChunkSizeY;

                Console.WriteLine("Saving obj at " + child.Transform.Position);

                _chunkFile.GameObjects.Add(gObj);

            }
            FileSystem.FileManager.SaveFile<FileSystem.ChunkFile>("./res/zone_" + _zoneId + "/chunk_" + _chunkX + "_" + _chunkY, _chunkFile);
        }

        public static void CreateNew(int zoneId, int x, int y)
        {
            var chunkFile = new FileSystem.ChunkFile(x, y, ChunkSizeX, ChunkSizeY);

            FileSystem.FileManager.SaveFile<FileSystem.ChunkFile>("./res/zone_" + zoneId + "/chunk_" + x + "_" + y, chunkFile);
        }
    }
}
