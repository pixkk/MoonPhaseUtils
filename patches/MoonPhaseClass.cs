using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;

namespace MoonPhaseUtils.patches;

[HarmonyPatch(typeof(GameManager), "SetGameMode")]
public class MoonPhaseClass
{
    internal static ManualLogSource Logger = Plugin.Logger;
    [HarmonyPostfix]
        public static void GenerateDonePatched(LevelGenerator __instance, PhotonMessageInfo _info)
        {
            int __moonLevel = -1;
            Logger.LogInfo($"Instance state - {__instance.State}");
            if (__instance.State == LevelGenerator.LevelState.Done)
            {
                if (RunManager.instance != null)
                {
                    var moonLevelField = AccessTools.Field(typeof(RunManager),"moonLevel");
                    if (moonLevelField != null) 
                    {
                    
                        __moonLevel = (int)moonLevelField.GetValue(RunManager.instance);
                        if (__moonLevel > 4)
                        {
                            Logger.LogInfo("MoonLevel more that 4 (currently is "+ __moonLevel + "). Setting to 1");
                            moonLevelField.SetValue(RunManager.instance, 1);
                        }
                        List<Moon> moons = (List<Moon>)AccessTools.Field(typeof(RunManager),"moons").GetValue(RunManager.instance);
                        Logger.LogInfo("Available Moons: ");
                        foreach (Moon moon in moons)
                        {
                            Logger.LogInfo("- " + moon.moonName + " (" + string.Join("||", moon.moonAttributes) + ")" + "(" + moon.moonIcon.ToString() + ")");
                        }
                    }
                    else
                    {
                    
                        // тут можешь делать что угодно с этим значением
                        Logger.LogInfo($"MoonLevelField is null. Current level - {__instance.Level}");
                    }
                }
                else
                {
                    Logger.LogWarning("RunManager.instance is null.");
                    return;
                }
            }
            else
            {
                
                // Logger.LogInfo($"Instance NOT generated - {__instance.Level}");
            }
            
        }
}