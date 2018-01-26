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
        private List<GameObject> _gameObjects;
        private int _zoneId;
        private int _chunkX;
        private int _chunkY;
        public bool Loaded;

        public DrawableChunk(int zoneId, int x, int y)
        {
            _zoneId = zoneId;
            _chunkX = x;
            _chunkY = y;

            var chunk = FileSystem.FileManager.GetFile<FileSystem.ChunkFile>("./res/zone_" + zoneId + "/chunk_" + x + "_" + y);

            if (chunk == null)
                return;

            _gameObjects = new List<GameObject>();

            var rootPos = new Vector3(
                     _chunkX * DrawableZone.ChunkSizeX,
                        0,
                     _chunkY * DrawableZone.ChunkSizeY);


            var chunkPlane = new GameObject("chunk plane");

            chunkPlane.Transform.Position = new OpenTK.Vector3(
                (DrawableZone.ChunkSizeX / 2) + rootPos.X, 
                0, 
                (DrawableZone.ChunkSizeY / 2) + rootPos.Z);

            chunkPlane.AddComponent(new MeshRenderer("cube.obj", "null"));

            chunkPlane.Transform.Scale = new Vector3((DrawableZone.ChunkSizeX / 2), 0.5f, (DrawableZone.ChunkSizeY / 2));
            _gameObjects.Add(chunkPlane);

            foreach (var chunkObj in chunk.GameObjects)
            {
   
                var gObj = new GameObject(chunkObj.Name);
                gObj.Transform.Position = new OpenTK.Vector3(chunkObj.X + rootPos.X, chunkObj.Y, chunkObj.Z + rootPos.X);

                gObj.AddComponent(new MeshRenderer("cube.obj", "null"));

                _gameObjects.Add(gObj);
            }

            Loaded = true;
        }

        public void AddObject(GameObject obj) {

            _gameObjects.Add(obj);
        }

        public void Draw(ICoreEngine engine)
        {
            // transform the objects back to their positions


            foreach (var gameObject in _gameObjects)
            {
                gameObject.AddToEngine(engine);
            }

        }

        public void Save() {
            var chunkFile = new FileSystem.ChunkFile();

            foreach (var child in _gameObjects)
            {
                FileSystem.GameObject gObj = new FileSystem.GameObject();

                gObj.Name = child.Name;
                gObj.X = child.Transform.Position.X % DrawableZone.ChunkSizeX;
                gObj.Y = child.Transform.Position.Y;
                gObj.Z = child.Transform.Position.Z % DrawableZone.ChunkSizeY;

                Console.WriteLine("Saving obj at " + child.Transform.Position);

                chunkFile.GameObjects.Add(gObj);

            }
            FileSystem.FileManager.SaveFile<FileSystem.ChunkFile>("./res/zone_" + _zoneId + "/chunk_" + _chunkX + "_" + _chunkY, chunkFile);
        }

        public static void CreateNew(int zoneId, int x, int y)
        {
            var chunkFile = new FileSystem.ChunkFile();

            FileSystem.FileManager.SaveFile<FileSystem.ChunkFile>("./res/zone_" + zoneId + "/chunk_" + x + "_" + y, chunkFile);
        }
    }
}
