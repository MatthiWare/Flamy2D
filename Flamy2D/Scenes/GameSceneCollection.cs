using System;
using System.Collections.ObjectModel;

namespace Flamy2D.Scenes
{
    public class GameSceneCollection : KeyedCollection<String, GameScene>
    {
        protected override string GetKeyForItem(GameScene item)
        {
            return item.Name;
        }
    }
}
