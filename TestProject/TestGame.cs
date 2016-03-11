using Flamy2D;

namespace TestProject
{
    public class TestGame : GameEngine
    {
        public Camera camera;

        public TestGame(GameConfiguration config)
            : base(config)
        {
            MainScene scene = new MainScene();
            camera=new Camera(0,0,config.Width, config.Height);
            Scenes.Add(scene);
            SwitchScene(MainScene.ID);
        }

    }
}
