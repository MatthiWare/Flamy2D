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
        private float speed = 30f;
        private float scale = 2f;
        public Player()
        {
            
        }

        public void Update(GameEngine game)
        {
            if (!initialized)
            {
                texture = Texture2D.LoadFromFile("./Content/sprite.png", TextureConfiguration.Nearest);
                width = texture.Width;
                height = texture.Height;
                x = 50;
                y = 100;
                initialized = true;
            }


            if (game.Keyboard.IsAnyKeyDown(Key.Q, Key.Left))
            {
                x -= (float)(speed * game.Time.Elapsed);
            }

            if (game.Keyboard.IsAnyKeyDown(Key.D, Key.Right))
            {
                x += (float)(speed * game.Time.Elapsed);
            }

            if (game.Keyboard.IsAnyKeyDown(Key.Z, Key.Up))
            {
                y -= (float)(speed * game.Time.Elapsed);
            }

            if (game.Keyboard.IsAnyKeyDown(Key.S, Key.Down))
            {
                y += (float)(speed * game.Time.Elapsed);
            }


        }

        public void Render(GameEngine game, SpriteBatch batch)
        {
            batch.Draw(texture, x, y, new Color4(255,255,255,255), scale);
        }
    }
}
