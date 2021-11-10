using HarmonyLib;
using SDG.Unturned;

namespace ShopsUI.SellBox
{
    public static class SellBoxEvents
    {
        public delegate void CloseStorage(Player player);

        public static event CloseStorage? OnCloseStorage;

        [HarmonyPatch]
        private static class Patches
        {
            // ReSharper disable once InconsistentNaming
            [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.closeStorage))]
            [HarmonyPrefix]
            private static void PostCloseStorage(PlayerInventory __instance)
            {
                OnCloseStorage?.Invoke(__instance.player);
            }
        }
    }
}
