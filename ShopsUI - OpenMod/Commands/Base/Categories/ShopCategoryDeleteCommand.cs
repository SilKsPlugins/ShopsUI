using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Commands.Base.Categories
{
    public abstract class ShopCategoryDeleteCommand<TCategoryEditor, TShopData> : ShopCategoryCommandBase
        where TCategoryEditor : ICategoryEditor
        where TShopData : IShopData
    {
        private readonly DeleteConfirmer _deleteConfirmer;
        private readonly TCategoryEditor _categoryEditor;

        protected ShopCategoryDeleteCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _deleteConfirmer = serviceProvider.GetRequiredService<DeleteConfirmer>();
            _categoryEditor = serviceProvider.GetRequiredService<TCategoryEditor>();
        }

        protected abstract Task<IShopCategory<TShopData>?> GetCategory(string name);

        protected abstract string GetSuccessLocalizationKey();

        protected abstract string GetConfirmationLocalizationKey();

        protected override async UniTask OnExecuteAsync()
        {
            if (Context.Parameters.Count == 0)
            {
                throw new CommandWrongUsageException(Context);
            }

            var parameters = Context.Parameters.ToList();

            var force = parameters.RemoveAll(x =>
                x.Equals("-f", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("--force", StringComparison.OrdinalIgnoreCase)) > 0;

            var categoryName = parameters.FirstOrDefault() ?? throw new CommandWrongUsageException(Context);

            var checkCategoryName = GetType().Name + ":" + categoryName;

            var category = await GetCategory(categoryName);

            if (category == null)
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_category:no_category",
                    new { CategoryName = categoryName }]);
            }

            if (!force && !_deleteConfirmer.ShouldContinue(Context.Actor.Type, Context.Actor.Id, checkCategoryName))
            {
                await PrintAsync(StringLocalizer[GetConfirmationLocalizationKey(), new { Category = category }]);
                return;
            }

            var result = await _categoryEditor.DeleteCategory(categoryName);

            if (!result)
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_category:no_category",
                    new {CategoryName = categoryName}]);
            }

            await PrintAsync(StringLocalizer[GetSuccessLocalizationKey(),
                new {CategoryName = categoryName}]);
        }
    }

    public class DeleteConfirmer
    {
        private readonly Dictionary<(string ActorType, string ActorId, string CategoryName), DateTime>
            _deletionAttempts = new();

        public bool ShouldContinue(string actorType, string actorId, string categoryName)
        {
            var key = (actorType, actorId, categoryName.ToLower());

            if (!_deletionAttempts.TryGetValue(key, out var lastRunTime) || (DateTime.Now - lastRunTime).TotalSeconds > 10)
            {
                _deletionAttempts[key] = DateTime.Now;

                return false;
            }

            _deletionAttempts.Remove(key);

            return true;
        }
    }
}
