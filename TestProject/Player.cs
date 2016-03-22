using System;
using Flamy2D;
using Flamy2D.GameObjects;
using Flamy2D.Graphics;
using OpenTK.Graphics;
using OpenTK.Input;
using Flamy2D.Fonts;
using Flamy2D.Audio;

namespace TestProject
{
    public class Player : GameObject
    {
        private Texture2D[] textures;
        private Texture2D tex;
        private float x, y;
        private float drawX, drawY;
        private int width, height;
        private float speed = 100f;
        private float scale = 2f;
        private double c = 0;
        private float changer = 0.35f;
        private bool switcher = true;
        private Font font;
        private Text text;
        private Sound sound;

        public Player()
        {
            textures=new Texture2D[8];
        }

        public override void Load(GameEngine game)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = game.Content.Load<Texture2D>("spr_speler_" + i + ".png", TextureConfiguration.Nearest);
            }

            tex = textures[0];

            width = tex.Width;
            height = tex.Height;
            x = ((float)game.Configuration.Width / 2) - ((float)width / 2);
            y = ((float)game.Configuration.Height / 2) - ((float)height / 2);
            drawX = ((float)game.Configuration.Width / 2) - ((float)width / 2);
            drawY = ((float)game.Configuration.Height / 2) - ((float)height / 2);
             font = game.Content.Load<Font>("LVDCC.TTF", 50f);
            text = new Text("Het werkt gg ez!", font, x, y);
            sound = game.Content.Load<Sound>("3test.ogg");
        }

        public override void Update(GameEngine game)
        {
            base.Update(game);

            TestGame g = (TestGame) game;

            if (game.Keyboard.IsKeyDown(Key.S))
            {
                sound.Play();
            }

            if (game.Keyboard.IsAnyKeyDown(Key.Q, Key.Left))
            {
                x -= (float)(speed * game.Time.Elapsed);
                g.camera.x-= (float)(speed * game.Time.Elapsed);
                if (switcher)
                    tex = textures[4];
                else
                    tex = textures[5];
            }

            if (game.Keyboard.IsAnyKeyDown(Key.D, Key.Right))
            {
                x += (float)(speed * game.Time.Elapsed);
                g.camera.x += (float)(speed * game.Time.Elapsed);
                if (switcher)
                    tex = textures[0];
                else
                    tex = textures[1];
            }

            if (game.Keyboard.IsAnyKeyDown(Key.Z, Key.Up))
            {
                y -= (float)(speed * game.Time.Elapsed);
                g.camera.y -= (float)(speed * game.Time.Elapsed);
                if (switcher)
                    tex = textures[2];
                else
                    tex = textures[3];
            }

            if (game.Keyboard.IsAnyKeyDown(Key.S, Key.Down))
            {
                y += (float)(speed * game.Time.Elapsed);
                g.camera.y += (float)(speed * game.Time.Elapsed);
                if (switcher)
                    tex = textures[6];
                else
                    tex = textures[7];
            }

            c += game.Time.Elapsed;

            if (c > changer)
            {
                c = 0;
                switcher = !switcher;
            }

            text.Update(g);
        }

        

        public override void Render(GameEngine game, SpriteBatch batch)
        {
            batch.Draw(tex, drawX, drawY, Color4.White, scale);
            text.Render(game, batch);
        }
    }
}
