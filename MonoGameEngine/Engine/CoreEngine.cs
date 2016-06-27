using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameEngine.Engine;
using MonoGameEngine.Engine.Components;
using MonoGameEngine.Engine.UI;

namespace MonoGameEngine {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class CoreEngine : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BaseGameCode _baseGameCode;
        private UISystem _uiSystem;
        private PhysicsEngine _physicsEngine;

        public List<GameObject> GameObjects = new List<GameObject>();

        public static CoreEngine instance;

        public CoreEngine(string title, BaseGameCode baseCode) {
            instance = this;
            Content.RootDirectory = "Content";
            _graphics = new GraphicsDeviceManager(this);

            Window.Title = title;

            _baseGameCode = baseCode;
        }

        public PhysicsEngine GetPhysicsEngine => _physicsEngine;

        public SpriteBatch GetSpriteBatch => _spriteBatch;

        public UISystem GetUISystem => _uiSystem;

        protected override void Initialize() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _uiSystem = new UISystem();
            _physicsEngine = new PhysicsEngine();
            _graphics.ApplyChanges();

            _baseGameCode.Initialize();
        }

        protected override void LoadContent() {
        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            _uiSystem.Update();
            _physicsEngine.Update(gameTime);
            _baseGameCode.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            for (int i = 0; i < GameObjects.Count; i++) {

                GameObjects[i].Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public GameObject GetClosestGameObject(Vector3 position) {
            GameObject closest = GameObjects[0];
            foreach (var gameObject in GameObjects) {
                if (Vector3.Distance(gameObject.Transform.Position, position) <= Vector3.Distance(closest.Transform.Position, position))
                    closest = gameObject;
            }

            return closest;
        }

        public GameObject AddGameObject(GameObject gameObject) {
            GameObjects.Add(gameObject);
            return GameObjects[GameObjects.Count - 1];
        }

        public void DestoryGameObject(GameObject obj) {
            GameObjects.Remove(obj);
        }


        protected override void Draw(GameTime gameTime) {

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // later on this will be sorted with a draw order list
            var components = new List<Component>();

            for (int i = 0; i < GameObjects.Count; i++) {
                for (int j = 0; j < GameObjects[i]._components.Count; j++) {
                    components.Add(GameObjects[i]._components[j]);
                }
            }

            foreach (var component in components) {
                component.Draw(GraphicsDevice);
            }

            base.Draw(gameTime);
        }



        public static void Quit() {
            instance.Exit();
        }
    }
}
