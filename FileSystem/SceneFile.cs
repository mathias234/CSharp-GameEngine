using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileSystem
{
    [FileExtension(".cnk")]
    public class ChunkFile : ISerializableFile
    {
        public List<GameObject> GameObjects { get; set; }

        public ChunkFile()
        {
            GameObjects = new List<GameObject>();
        }

        public void Deserialize(BinaryReader reader)
        {
            var objectsCount = reader.ReadInt32();

            GameObjects = new List<GameObject>();

            for (int i = 0; i < objectsCount; i++)
            {
                var gameObj = new GameObject();
                gameObj.Deserialize(reader);
                GameObjects.Add(gameObj);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(GameObjects.Count);
            foreach (var gameObj in GameObjects)
            {
                gameObj.Serialize(writer);
            }
        }
    }

    public class GameObject
    {
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }


        public List<GameComponent> Components { get; set; }

        public GameObject()
        {
            Components = new List<GameComponent>();
        }

        public void Deserialize(BinaryReader reader)
        {
            Name = reader.ReadString();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();

            int componentCount = reader.ReadInt32();

            Components = new List<GameComponent>();

            for (int i = 0; i < componentCount; i++)
            {
                var component = new GameComponent();
                component.Deserialize(reader);
                Components.Add(component);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);

            writer.Write(Components.Count);
            foreach (var component in Components)
            {
                component.Serialize(writer);
            }
        }
    }

    public class GameComponent
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object[] Args { get; set; }

        public GameComponent()
        {
            Args = new object[0];
        }

        public void Deserialize(BinaryReader reader)
        {
            Name = reader.ReadString();
            Type = reader.ReadString();

            var argsCount = reader.ReadInt32();
            Args = new string[argsCount];
            for (int i = 0; i < argsCount; i++)
            {
                Args[i] = reader.ReadString();
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Type);

            writer.Write(Args.Length);
            foreach (var arg in Args)
            {
                writer.Write((string)arg);
            }
        }
    }
}
