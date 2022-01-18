using System;

namespace ShopsUI.Configuration
{
    [Serializable]
    public class UIConfig
    {
        public string? LogoUrl { get; set; } = "https://i.imgur.com/t6HbFTN.png";

        public ushort ShopsEffect { get; set; } = 29300;

        public ushort SellBoxEffect { get; set; } = 29400;

        public bool BackgroundsEnabled { get; set; } = true;

        public float ButtonDelay { get; set; } = 1;

        public float CategoryButtonDelay { get; set; } = 0.2f;
        
        public bool ShowEmptyCategories { get; set; } = false;

        public bool ShowSellOnlyShops { get; set; } = true;

        public float DisplayDelay { get; set; } = 0.5f;

        public int DisplayAmount { get; set; } = 10;
    }
}
