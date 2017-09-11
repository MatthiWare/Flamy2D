using Flamy2D.Graphics;

namespace Flamy2D.GameObjects
{
    public abstract class GameObject
    {
        public bool Loaded { get; private set; }

        protected GameObject()
        {
            Loaded = false;
        }
        public abstract void Load(Game game);

        public virtual void Update(Game game)
        {
            if (!Loaded)
            {
                Load(game);
                Loaded = true;
            }
        }

        public abstract void Render(Game game, SpriteBatch batch);

        public void Reload()
        {
            Loaded = false;
        }
    }


}
