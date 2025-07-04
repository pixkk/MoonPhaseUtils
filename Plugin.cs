using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoonPhaseUtils.patches;

namespace MoonPhaseUtils;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    static Harmony harmony;

    private void Awake()
    {
        // Plugin startup logic 
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        
        MethodInfo original = AccessTools.Method(typeof(LevelGenerator), "GenerateDone");
        MethodInfo patch = AccessTools.Method(typeof(MoonPhaseClass), "GenerateDonePatched");
        harmony.Patch(original, new HarmonyMethod(patch));
        
        PatchMoonUpdate();
    }

    private void PatchMoonUpdate()
    {
        // MethodInfo original = AccessTools.Method(typeof(RunManager), "UpdateMoonLevel");
        // MethodInfo patch = AccessTools.Method(typeof(RunManagerPatchClass), "UpdateMoonLevelPatched");
        // harmony.Patch(original, new HarmonyMethod(patch));
        
        MethodInfo original = AccessTools.Method(typeof(RunManager), "CalculateMoonLevel");
        MethodInfo patch = AccessTools.Method(typeof(RunManagerPatchClass), "CalculateMoonLevelPatched");
        harmony.Patch(original, new HarmonyMethod(patch));
        
        MethodInfo originalTwo = AccessTools.Method(typeof(MenuPageSaves), "SaveFileSelected");
        MethodInfo patchTwo = AccessTools.Method(typeof(MenuPageSavesPatchClass), "SaveFileSelectedPatched");
        harmony.Patch(originalTwo, new HarmonyMethod(patchTwo));
        
    }
}   