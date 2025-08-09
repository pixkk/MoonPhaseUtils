using BepInEx.Logging;
using HarmonyLib;

namespace MoonPhaseUtils.patches;

public class RunManagerPatchClass
{
    internal static ManualLogSource Logger = Plugin.Logger;

    [HarmonyPrefix]
    private static bool CalculateMoonLevelPatched(RunManager __instance, int _levelsCompleted, ref int __result)
    {
        __result = ((_levelsCompleted - 1) % 4) + 1;
        return false;
    }
    
    [HarmonyPrefix]
    private static bool UpdateMoonLevelPatched(RunManager __instance)
    {
        int moonLevel = (int)GetField("moonLevel", __instance);
        int moonLevelCalculated = __instance.CalculateMoonLevel((int)GetField("levelsCompleted", __instance));
        SetField("moonLevelPrev", moonLevel, __instance);
        SetField("moonLevel", moonLevelCalculated,  __instance);

        if (moonLevel == moonLevelCalculated)
        {
            return false; 
        }
        SetField("moonLevelChanged", true,  __instance);
        return false;
    }
    private static object GetField(string fieldName, object instance = null)
    {
        var field = AccessTools.Field(typeof(RunManager), fieldName);
        return field != null ? field.GetValue(instance) : null;
    }
    
    private static object ExecuteMethod(string methodName, object instance = null, object[] parameters = null)
    {
        var method = AccessTools.Method(typeof(RunManager), methodName);
        if (method == null) return null;
        return method.Invoke(instance, parameters);
    }

    private static void SetField(string fieldName, object value, object instance = null)
    {
        var field = AccessTools.Field(typeof(RunManager), fieldName);
        if (field != null)
        {
            field.SetValue(instance, value);
        }
    }
}