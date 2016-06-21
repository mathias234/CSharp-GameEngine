using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using SwiftEngine.Components;
using Microsoft.Xna.Framework;
using SwiftEngine.Engine.Components;

namespace SwiftEngine.Engine {
    public static class SceneManager {
        public static void LoadScene(string name) {
            XmlSerializer serializer = new XmlSerializer(typeof(List<GameObject>));
            FileStream fs = null;
            try {
                fs = new FileStream(@"Scenes\" + name, FileMode.OpenOrCreate);
                CoreEngine.instance.GameObjects = (List<GameObject>) serializer.Deserialize(fs);
                fs.Dispose();
            }
            catch (DirectoryNotFoundException) {
                Directory.CreateDirectory("Scenes");
                SaveScene(name);
            }
            catch (System.InvalidOperationException) {
                fs?.Close();
                CreateNewScene(name);
            }
            catch (Exception) {
                throw;
            }
            foreach (var gameObject in CoreEngine.instance.GameObjects) {
                gameObject.Initialize();
            }
        }

        public static void SaveScene(string name) {
            XmlSerializer serializer = new XmlSerializer(typeof(List<GameObject>));
            FileStream fs = null;
            try {
                fs = new FileStream(@"Scenes\" + name, FileMode.OpenOrCreate);
                serializer.Serialize(fs, CoreEngine.instance.GameObjects);
                fs.Close();
            }
            catch (DirectoryNotFoundException) {
                Directory.CreateDirectory("Scenes");
                SaveScene(name);
            }
            catch (Exception) {
                throw;
            }
        }

        public static void CreateNewScene(string name) {
            CoreEngine.instance.GameObjects = new List<GameObject>();

            var camera = new GameObject(new Vector3(0, 20, -100));
            camera.AddComponent<Camera>();
            camera.name = "Camera";
            camera.Instantiate();

            var sampleCube = new GameObject(new Vector3(0, 30, 0));
            sampleCube.AddComponent<MeshRenderer>();
            sampleCube.GetComponent<MeshRenderer>().Mesh = Primitives.CreateCube();
            sampleCube.GetComponent<MeshRenderer>().Color = Color.LightGray;
            sampleCube.AddComponent<SphereCollider>();
            sampleCube.GetComponent<SphereCollider>().Radius = 2;
            sampleCube.GetComponent<SphereCollider>().Mass = 10;
            sampleCube.GetComponent<SphereCollider>().IsStatic = false;
            sampleCube.name = "cube";
            sampleCube.Instantiate();

            var ground = new GameObject(new Vector3(0, 0, 0));
            ground.AddComponent<MeshRenderer>();
            ground.GetComponent<MeshRenderer>().Mesh = Primitives.CreateCube();
            ground.GetComponent<MeshRenderer>().Color = Color.LightGray;
            ground.Transform.Scale = new Vector3(200, 1, 200);
            ground.AddComponent<BoxCollider>();
            ground.GetComponent<BoxCollider>().Height = 2f;
            ground.GetComponent<BoxCollider>().Width = 200 * 2f;
            ground.GetComponent<BoxCollider>().Length = 200 * 2f;
            ground.GetComponent<BoxCollider>().Mass = 0;
            ground.GetComponent<BoxCollider>().IsStatic = true;
            ground.name = "ground";
            ground.Instantiate();
        }
    }
}
