using Rocket.API.Collections;
using Rocket.Core.Plugins;
using ShopsUI.Configuration;

namespace ShopsUI
{
    public class ShopsUIPlugin : RocketPlugin<ShopsUIConfiguration>
    {
        public static ShopsUIPlugin Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;
        }

        protected override void Unload()
        {
            Instance = null;
        }

        public override TranslationList DefaultTranslations { get; } = new TranslationList()
        {
			
        };
    }
}
