using Flamy2D;

namespace TestProject
{
    public class TestGame : GameEngine
    {

        public TestGame(GameConfiguration config)
            : base(config)
        {
            MainScene scene = new MainScene();

            Scenes.Add(scene);
            SwitchScene(MainScene.ID);
        }

    }
}
