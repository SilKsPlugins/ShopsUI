using Cysharp.Threading.Tasks;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MoreLinq;
using OpenMod.API.Permissions;
using OpenMod.Core.Permissions;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace ShopsUI.SellBox
{
    public class SellBoxInstance : IAsyncDisposable
    {
        private static readonly FieldInfo StorageItemsField = AccessTools.Field(typeof(InteractableStorage), "_items");

        public delegate void SellBoxClosed(UnturnedUser user, SellBoxInstance sellBox);

        public static event SellBoxClosed? OnSellBoxClosed;

        public UnturnedUser User { get; }

        public PlayerInventory PlayerInventory => User.Player.Player.inventory; 

        public InteractableStorage? Storage { get; private set; }

        private readonly ILogger<SellBoxInstance> _logger;
        private readonly IPermissionRoleStore _roleStore;
        private readonly IPermissionRolesDataStore _rolesDataStore;
        private readonly IConfiguration _configuration;
        
        public SellBoxInstance(UnturnedUser user,
            ILogger<SellBoxInstance> logger,
            IPermissionRoleStore roleStore,
            IPermissionRolesDataStore rolesDataStore,
            IConfiguration configuration)
        {
            User = user;
            _logger = logger;
            _roleStore = roleStore;
            _rolesDataStore = rolesDataStore;
            _configuration = configuration;
        }

        private async Task<SellBoxDimensions?> GetSellBoxDimensions()
        {
            var roles = await _roleStore.GetRolesAsync(User);

            var sellBoxDims = _configuration.GetSection("sellbox").Get<SellBoxDimensions>();

            foreach (var role in roles.OrderBy(x => x.Priority))
            {
                var currentSellBoxDims = await _rolesDataStore.GetRoleDataAsync<SellBoxDimensions>(role.Id, "sellbox");

                if (currentSellBoxDims != null)
                {
                    sellBoxDims = currentSellBoxDims;

                    break;
                }
            }

            return sellBoxDims;
        }

        private async Task<InteractableStorage?> CreateStorage()
        {
            var sellBoxDims = await GetSellBoxDimensions();

            if (sellBoxDims == null || sellBoxDims.Height == 0 || sellBoxDims.Width == 0)
            {
                return null;
            }

            var sellBoxObject = new GameObject();

            var storage = sellBoxObject.AddComponent<InteractableStorage>();

            StorageItemsField.SetValue(storage, new Items(PlayerInventory.STORAGE));

            storage.items.resize(sellBoxDims.Width, sellBoxDims.Height);

            return storage;
        }

        public async UniTask<bool> OpenAsync()
        {
            await UniTask.SwitchToMainThread();

            SellBoxEvents.OnCloseStorage += OnCloseStorage;

            Storage = await CreateStorage();

            if (Storage == null)
            {
                return false;
            }

            PlayerInventory.openStorage(Storage);

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await UniTask.SwitchToMainThread();

                SellBoxEvents.OnCloseStorage -= OnCloseStorage;

                if (Storage != null && PlayerInventory.storage == Storage)
                {
                    PlayerInventory.closeStorage();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred when disposing of sell box instance");
            }
        }

        private void OnCloseStorage(Player player)
        {
            if (player == User.Player.Player && player.inventory.storage == Storage)
            {
                OnSellBoxClosed?.Invoke(User, this);
            }
        }
    }
}
