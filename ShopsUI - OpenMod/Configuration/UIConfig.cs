using System;

namespace ShopsUI.Configuration
{
    [Serializable]
    public class UIConfig
    {
        public string? LogoUrl { get; set; } = "https://i.imgur.com/t6HbFTN.png";

        public ushort ShopsEffect { get; set; } = 29150;

        public ushort SellBoxEffect { get; set; } = 29200;

        public bool BackgroundsEnabled { get; set; } = true;

        public float ButtonDelay { get; set; } = 1;

        public float CategoryButtonDelay { get; set; } = 0.2f;
        
        public bool ShowEmptyCategories { get; set; } = false;

        public bool ShowSellOnlyShops { get; set; } = true;
    }
}
