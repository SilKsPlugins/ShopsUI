using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopsUI.Configuration
{
    [Serializable]
    public class DefaultCategoryConfig
    {
        public DefaultSingleCategoryConfig Items { get; set; } = new();

        public DefaultSingleCategoryConfig Vehicles { get; set; } = new();
    }
}
