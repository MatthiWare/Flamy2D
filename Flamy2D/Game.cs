using System;
using Flamy2D.Base;
using Flamy2D.Scenes;
using Flamy2D.Graphics;

namespace Flamy2D
{
    public class Game : BaseGame
    {
        public GameSceneCollection Scenes { get; private set; }
        public GameScene CurrentScene { get; private set; }

        public Game(GameConfiguration config)
            : base(config)
        {
            Scenes = new GameSceneCollection();
        }

        public void SwitchScene(string sceneName)
        {
            if (!Scenes.Contains(sceneName))
            {
                this.Log($"'{sceneName}' scene doesn't exist");
                return;
            }

            if (CurrentScene == null)
            {
                CurrentScene = Scenes[sceneName];
                CurrentScene.SceneEntered();
                return;
            }

            if (CurrentScene.Name == sceneName)
            {
                this.Log($"'{sceneName}' scene is already showing");
                return;
            }

            CurrentScene.SceneExited();
            CurrentScene = Scenes[sceneName];
            CurrentScene.SceneEntered();
        }

        int updates = 0, renders = 0;
        double time = 0;
        protected override void Update()
        {
            updates++;
            if (CurrentScene != null)
            {
                CurrentScene.Update(this);
            }

            base.Update();

            time += Time.Elapsed;
            if (time > 1f)
            {
                Console.WriteLine($"UPS: {updates} - FPS: {renders}");
                updates = 0;
                renders = 0;
                time = 0;
            }
        }

        protected override void Render(SpriteBatch batch)
        {
            renders++;

            batch.Begin();

            CurrentScene?.Render(this, batch);

            batch.End();

            base.Render(batch);
        }
    }
}
