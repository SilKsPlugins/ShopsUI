using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops;
using ShopsUI.Ranges;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopsUI.Commands.Base.Categories
{
    public abstract class ShopCategoryRemoveCommand<TCategoryEditor> : ShopCategoryCommandBase
        where TCategoryEditor : ICategoryEditor
    {
        private readonly TCategoryEditor _categoryEditor;

        protected ShopCategoryRemoveCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _categoryEditor = serviceProvider.GetRequiredService<TCategoryEditor>();
        }

        protected abstract string GetSuccessLocalizationKey();

        protected override async UniTask OnExecuteAsync()
        {
            if (Context.Parameters.Count == 0)
            {
                throw new CommandWrongUsageException(Context);
            }

            var categoryName = await Context.Parameters.GetAsync<string>(0);
            var idRangeStr = await Context.Parameters.GetAsync<string>(1);

            List<ushort> shopIds;

            try
            {
                var idRange = RangeHelper.ParseMulti(idRangeStr);

                shopIds = new List<ushort>(idRange.Ranges.Sum(x => x.End - x.Start + 1));

                foreach (var subrange in idRange.Ranges)
                {
                    for (var i = subrange.Start; i <= subrange.End; i++)
                    {
                        shopIds.Add(i);
                    }
                }
            }
            catch (RangeParseException)
            {
                if (ushort.TryParse(idRangeStr, out var id))
                {
                    shopIds = new List<ushort> { id };
                }
                else
                {
                    throw new UserFriendlyException(StringLocalizer["commands:errors:invalid_range"]);
                }
            }

            var result = await _categoryEditor.RemoveShopsFromCategory(categoryName, shopIds) ??
                         throw new UserFriendlyException(StringLocalizer["commands:errors:shop_category:no_category",
                             new { CategoryName = categoryName }]);

            await PrintAsync(StringLocalizer[GetSuccessLocalizationKey(),
                new { ShopsRemoved = result, CategoryName = categoryName }]);
        }
    }
}