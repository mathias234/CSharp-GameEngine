using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcManagedFBX;
using ArcManagedFBX.IO;
using NewEngine.Engine.Core;

namespace NewEngine.Engine.Rendering.MeshLoading.FBX {
    public class FbxModel {

        private int _revision;
        private int _minor;
        private int _major;

        public FbxModel(string filename) {
            FBXManager manager = FBXManager.Create();
            FBXIOSettings settings = FBXIOSettings.Create(manager, "IOSRoot");

            manager.SetIOSettings(settings);
            manager.LoadPluginsDirectory(Environment.CurrentDirectory, "");

            FBXScene scene = FBXScene.Create(manager, filename);
            FBXImporter importer = FBXImporter.Create(manager, "");

            bool initializeResult = importer.Initialize(filename, -1, manager.GetIOSettings());

            FBXManager.GetFileFormatVersion(ref _major, ref _minor, ref _revision);

            bool importResult = importer.Import(scene);
            if(importResult == false)
                LogManager.Debug("ERROR!");
        }
    }
}
