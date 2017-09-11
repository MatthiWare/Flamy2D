using Flamy2D.Audio;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flamy2D.Assets.Providers
{
    public class SoundProvider : AssetHandler<Sound>
    {
        private ConcurrentDictionary<string, Sound> soundCache = new ConcurrentDictionary<string, Sound>();

        public SoundProvider(ContentManager mgr)
            : base(mgr, "sounds")
        { }

        public override Sound Load(string assetName, params object[] args)
        {
            return soundCache.GetOrAdd(assetName, new Sound(assetName));
        }

        public override void Save(Sound asset, string path)
        {
            throw new NotImplementedException();
        }
    }
}
