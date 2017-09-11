using Flamy2D;
using Flamy2D.Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace TestProject
{
    public class TestGame : Game
    {
        public Camera camera;

        public TestGame(GameConfiguration config)
            : base(config)
        {
            
        }

        protected override void Load()
        {
            MainScene scene = new MainScene();
            camera = new Camera(0, 0, Configuration.Width, Configuration.Height);
            Scenes.Add(scene);
            SwitchScene(MainScene.ID);
            Content.ContentRoot = "../../../Content/TestProject/";

            base.Load();
        }

        protected override void SetupOpenGL()
        {
            base.SetupOpenGL();
            GL.ClearColor(Color4.CornflowerBlue);
        }

        protected override void Render(SpriteBatch batch)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            base.Render(batch);
        }

    }
}
