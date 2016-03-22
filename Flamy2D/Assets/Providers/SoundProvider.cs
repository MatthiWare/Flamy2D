using Flamy2D.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Assets.Providers
{
    public class SoundProvider : AssetHandler<Sound>
    {
        public SoundProvider(ContentManager mgr)
            : base(mgr, "sounds")
        { }

        public override Sound Load(string assetName, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
