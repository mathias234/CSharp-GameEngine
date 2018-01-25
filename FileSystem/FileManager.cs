using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileSystem
{
    public class FileManager
    {
        private static Dictionary<string, Type> extTypeCache;

        private static void Init()
        {
            extTypeCache = new Dictionary<string, Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    var attribs = type.GetCustomAttributes(typeof(FileExtensionAttribute), false);
                    if (attribs != null && attribs.Length > 0)
                    {
                        foreach (var attrib in attribs)
                        {
                            var ext = ((FileExtensionAttribute)attrib).Extension;
                            extTypeCache.Add(ext, type);
                        }
                    }
                }
            }
        }

        public static T GetFile<T>(string filename) where T : ISerializableFile
        {
            if (extTypeCache == null)
                Init();

            var ext = extTypeCache.FirstOrDefault((val) => val.Value == typeof(T));

            filename += ext;

            if (!File.Exists(filename))
                return default(T);

            using (BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                ISerializableFile file = (T)Activator.CreateInstance(typeof(T));

                file.Deserialize(reader);

                return (T)file;
            }
        }


        public static void SaveFile<T>(string filename, T file) where T : ISerializableFile
        {
            if (extTypeCache == null)
                Init();

            if (!Directory.Exists(Path.GetDirectoryName( filename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            }

            var ext = extTypeCache.FirstOrDefault((val) => val.Value == typeof(T)).Key;

            filename += ext;

            Console.WriteLine("Trying to save file: " + filename);

            if (File.Exists(filename))
                File.Delete(filename);

            using (BinaryWriter writer = new BinaryWriter(new FileStream(filename, FileMode.Create)))
            {
                file.Serialize(writer);
            }
        }
    }
}
