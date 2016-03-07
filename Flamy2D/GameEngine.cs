using System;
using Flamy2D.Base;
using Flamy2D.Scenes;
using Flamy2D.Graphics;

namespace Flamy2D
{
    public class GameEngine : Game
    {
        public GameSceneCollection Scenes { get; private set; }
        public GameScene CurrentScene { get; private set; }

        public GameEngine(GameConfiguration config)
            : base(config)
        {
            Scenes = new GameSceneCollection();
        }

        public void SwitchScene(String sceneName)
        {
            if (Scenes.Contains(sceneName))
            {
                if (CurrentScene != null)
                {
                    if (CurrentScene.Name == sceneName)
                    {
                        this.Log("'{0}' scene is already showing", sceneName);
                    }
                    else
                    {
                        CurrentScene.SceneExited();
                        CurrentScene = Scenes[sceneName];
                        CurrentScene.SceneEntered();
                    }
                }
                else
                {
                    CurrentScene = Scenes[sceneName];
                    CurrentScene.SceneEntered();
                }
            }
            else 
            {
                this.Log("'{0}' scene doesn't exist", sceneName);
            }
            
        }

        protected override void Update()
        {
            base.Update();

            if (CurrentScene != null)
            {
                CurrentScene.Update(this);
            }
        }

        protected override void Render(SpriteBatch batch)
        {
            base.Render(batch);

            if (CurrentScene != null)
            {
                batch.Begin();
                CurrentScene.Render(this, batch);
                batch.End();
            }

            
        }
    }
}
