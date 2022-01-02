using System;

namespace ShopsUI.Configuration
{
    [Serializable]
    public class DefaultSingleCategoryConfig
    {
        public bool Enabled { get; set; } = true;

        public string Name { get; set; } = "All";
    }
}
