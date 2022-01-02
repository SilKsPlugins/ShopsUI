#if DEBUG
using HarmonyLib;
using SDG.Unturned;
using System.Collections.Generic;
using System.Reflection;

namespace ShopsUI
{
    [HarmonyPatch]
    public static class UnturnedValidationPatches
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            var classType = typeof(Player).Assembly.GetType("SDG.Unturned.MasterBundleValidation");

            yield return classType.GetMethod("serverHandleResponse", BindingFlags.Public | BindingFlags.Static)!;
        }

        private static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
#endif