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

        int updates =0 , renders = 0;
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
                Console.WriteLine(String.Format("UPS {0}  -- FPS {1}", updates, renders));
                updates = 0;
                renders = 0;
                time = 0;
            }
        }

        protected override void Render(SpriteBatch batch)
        {
            renders++;

            if (CurrentScene != null)
            {
                batch.Begin();
                CurrentScene.Render(this, batch);
                batch.End();
            }

            base.Render(batch);
        }
    }
}
