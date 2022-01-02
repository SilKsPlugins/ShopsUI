using System;

namespace ShopsUI.Configuration
{
    [Serializable]
    public class ShopsUIConfig
    {
        public ShopsConfig Shops { get; set; } = new();

        public SellBoxConfig SellBox { get; set; } = new();

        public UIConfig UI { get; set; } = new();
    }
}
