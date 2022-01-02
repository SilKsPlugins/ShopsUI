using System;

namespace ShopsUI.Configuration
{
    [Serializable]
    public class SellBoxConfig
    {
        public byte Width { get; set; } = 8;

        public byte Height { get; set; } = 6;
    }
}
