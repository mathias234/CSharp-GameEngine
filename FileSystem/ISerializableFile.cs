using System.IO;

namespace FileSystem
{
    public interface ISerializableFile
    {
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}