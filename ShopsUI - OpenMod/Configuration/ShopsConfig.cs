using System;

namespace ShopsUI.Configuration
{
    [Serializable]
    public class ShopsConfig
    {
        public bool BlacklistEnabled { get; set; } = false;

        public bool WhitelistEnabled { get; set; } = false;

        public DefaultCategoryConfig DefaultCategory { get; set; } = new();
    }
}
