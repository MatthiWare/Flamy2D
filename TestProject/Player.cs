using Flamy2D;
using Flamy2D.Graphics;
using OpenTK.Graphics;
using OpenTK.Input;

namespace TestProject
{
    public class Player
    {
        private Texture2D texture;
        private float x, y;
        private int width, height;
        private bool initialized = false;
        public Player()
        {
            
        }

        public void Update(GameEngine game)
        {
            if (!initialized)
            {
                texture = Texture2D.LoadFromFile("./Content/sprite.png", TextureConfiguration.Linear);
                width = texture.Width;
                height = texture.Height;
                x = 50;
                y = 100;
                initialized = true;
            }

            if (game.Keyboard.IsAnyKeyDown(Key.Q, Key.Left))
            {
                x -= 0.5f;
            }
        }

        public void Render(GameEngine game, SpriteBatch batch)
        {
            batch.Draw(texture, x, y, Color4.Red);
        }
    }
}
