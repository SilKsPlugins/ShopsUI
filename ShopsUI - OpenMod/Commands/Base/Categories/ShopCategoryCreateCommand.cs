using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using System;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops;

namespace ShopsUI.Commands.Base.Categories
{
    public abstract class ShopCategoryCreateCommand<TCategoryEditor> : ShopCategoryCommandBase
        where TCategoryEditor : ICategoryEditor
    {
        private readonly TCategoryEditor _categoryEditor;

        protected ShopCategoryCreateCommand(IServiceProvider serviceProvider) : base(serviceProvider)
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

            var result = await _categoryEditor.CreateCategory(categoryName);

            if (!result)
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_category:category_already_exists",
                    new {CategoryName = categoryName}]);
            }

            await PrintAsync(StringLocalizer[GetSuccessLocalizationKey(),
                new {CategoryName = categoryName}]);
        }
    }
}
