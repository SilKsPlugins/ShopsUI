using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopsUI.API.Shops
{
    public interface ICategoryEditor
    {
        Task<bool> CreateCategory(string displayName);
        
        Task<bool> DeleteCategory(string displayName);

        Task<int?> AddShopsToCategory(string displayName, IEnumerable<ushort> shopIds);

        Task<int?> RemoveShopsFromCategory(string displayName, IEnumerable<ushort> shopIds);
    }
}
