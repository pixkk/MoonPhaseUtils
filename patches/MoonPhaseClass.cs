using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace MoonPhaseUtils.patches;

[HarmonyPatch(typeof(GameManager), "SetGameMode")]
public class MoonPhaseClass
{
    internal static ManualLogSource Logger = Plugin.Logger;

    [HarmonyPostfix]
    public static void GenerateDonePatched(LevelGenerator __instance, PhotonMessageInfo _info)
    {
        // Logger.LogInfo($"Instance state - {__instance.State}");
        if (RunManager.instance != null)
        {
            bool isReady = RunManager.instance.levelCurrent != RunManager.instance.levelLobby
                           && RunManager.instance.levelCurrent != RunManager.instance.levelMainMenu
                           && RunManager.instance.levelCurrent != RunManager.instance.levelLobbyMenu
                           && RunManager.instance.levelCurrent != RunManager.instance.levelRecording
                           && !SemiFunc.IsLevelShop(RunManager.instance.levelCurrent)
                           && RunManager.instance.levelCurrent != RunManager.instance.levelTutorial;
            if (__instance.State == LevelGenerator.LevelState.Done && isReady)
            {
                var moonLevelField = AccessTools.Field(typeof(RunManager), "moonLevel");
                if (moonLevelField != null)
                {
                    var __moonLevel = (int)moonLevelField.GetValue(RunManager.instance);
                    if (Plugin.Instance != null)
                    {
                        Plugin.Instance.SetupLabel();
                        Plugin.Instance.SetupImage();
                        List<Moon.MoonAttribute> moonAttributes = RunManager.instance.MoonGetAttributes(__moonLevel);
                        // List<string> descrString = [];
                        // foreach (Moon.MoonAttribute attr in moonAttributes)
                        // {
                        //     descrString.Add(attr.text);
                        //     // Logger.LogInfo($"Found Moon Attr {attr.text.ToString()}");
                        // }

                        var text = string.Concat(
                            "<color=#",
                            ColorUtility.ToHtmlStringRGB(Color.white),
                            "><b>",
                            RunManager.instance.MoonGetName(__moonLevel).Trim(),
                            "</b>" + "\n" +
                            string.Join("\n", moonAttributes.Select(a => a.text)),
                            "</color>"
                        );

                        
                        bool showMoon = __moonLevel != 0;
                        Plugin.Instance.screenLabelText.SetText(text);
                        Plugin.Instance.screenLabelText.fontSizeMax = 10f;
                        Plugin.Instance.screenLabelText.fontSize = 12f;
                        Plugin.Instance.screenLabel.SetActive(showMoon);
                        Plugin.Instance.UpdateImagePosition();
                        Plugin.Instance.screenImageTexture.texture = RunManager.instance.MoonGetIcon(__moonLevel);
                        
                        Plugin.Instance.screenImageTexture.color = showMoon 
                            ? new Color(1f, 1f, 1f, 1f) 
                            : new Color(1f, 1f, 1f, 0f);
                        Plugin.Instance.screenImage.SetActive(true);
                    }
                }
                else
                {
                    // Logger.LogInfo($"MoonLevelField is null. Current level - {__instance.Level}");
                }
            }
        }
        else
        {
            Logger.LogWarning("RunManager.instance is null.");
            return;
        }
    }
}