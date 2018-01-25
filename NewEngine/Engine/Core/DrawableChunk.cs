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
        private GameObject _chunkRoot;
        private int _zoneId;
        private int _chunkX;
        private int _chunkY;

        public DrawableChunk(int zoneId, int x, int y)
        {
            _zoneId = zoneId;
            _chunkX = x;
            _chunkY = y;

            var chunk = FileSystem.FileManager.GetFile<FileSystem.ChunkFile>("./res/zone_" + zoneId + "/chunk_" + x + "_" + y);
            foreach (var chunkObj in chunk.GameObjects)
            {
                var gObj = new GameObject(chunkObj.Name);
                gObj.Transform.Position = new OpenTK.Vector3(chunkObj.X, chunkObj.Y, chunkObj.Z);
                _chunkRoot.AddChild(gObj);
            }
        }

        public void AddObject(GameObject obj) {
            _chunkRoot.AddChild(obj);
        }

        public void Draw(ICoreEngine engine)
        {
            _chunkRoot.AddToEngine(engine);
        }

        public void Save() {
            var chunkFile = new FileSystem.ChunkFile();

            foreach (var child in _chunkRoot.GetChildren())
            {
                FileSystem.GameObject gObj = new FileSystem.GameObject();

                gObj.Name = child.Name;
                gObj.X = child.Transform.Position.X;
                gObj.Y = child.Transform.Position.Y;
                gObj.Z = child.Transform.Position.Z;

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
