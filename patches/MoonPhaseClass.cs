using System.Collections.Generic;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using MenuLib;
using MoonPhaseUtils.UI;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace MoonPhaseUtils.patches;

[HarmonyPatch(typeof(GameManager), "SetGameMode")]
public class MoonPhaseClass
{
    internal static ManualLogSource Logger = Plugin.Logger;

    [HarmonyPostfix]
    public static void GenerateDonePatched(LevelGenerator __instance, PhotonMessageInfo _info)
    {
        Logger.LogInfo($"Instance state - {__instance.State}");
        if (RunManager.instance != null)
        {
            bool isReady = RunManager.instance.levelCurrent != RunManager.instance.levelLobby
                           && RunManager.instance.levelCurrent != RunManager.instance.levelMainMenu
                           && RunManager.instance.levelCurrent != RunManager.instance.levelLobbyMenu
                           && RunManager.instance.levelCurrent != RunManager.instance.levelRecording
                           && RunManager.instance.levelCurrent != RunManager.instance.levelShop
                           && RunManager.instance.levelCurrent != RunManager.instance.levelTutorial;
            if (__instance.State == LevelGenerator.LevelState.Done && isReady)
            {
                var moonLevelField = AccessTools.Field(typeof(RunManager), "moonLevel");
                if (moonLevelField != null)
                {
                    var __moonLevel = (int)moonLevelField.GetValue(RunManager.instance);
                    List<Moon> moons = (List<Moon>)AccessTools.Field(typeof(RunManager), "moons")
                        .GetValue(RunManager.instance);
                    Logger.LogInfo("Available Moons: ");
                    foreach (Moon moon in moons)
                    {
                        Logger.LogInfo("- " + moon.moonName + " (" + string.Join("||", moon.moonAttributes) + ")" +
                                       "(" + moon.moonIcon.ToString() + ")");
                    }
                    //    TopUIBanner banner  here
                    // TopUIBanner.Show("TEST", RunManager.instance.MoonGetIcon(__moonLevel));

                    if (Plugin.Instance != null)
                    {
                        // GameObject[] activeGameObjects = Object.FindObjectsOfType<GameObject>();
                        // foreach (GameObject go in activeGameObjects)
                        // {
                        //     Logger.LogInfo($"Found game object {go.name}");
                        // }
                        Plugin.Instance.SetupLabel();
                        Plugin.Instance.SetupImage();
                        // string text = $"Dead players: test/{GameDirector.instance.PlayerList.Count}\n" 
                        //               + string.Join("\n", "test2");

                        string text = string.Concat(new string[]
                        {
                            "<color=#",
                            ColorUtility.ToHtmlStringRGB(Color.white),
                            "><b>",
                            RunManager.instance.MoonGetName(__moonLevel).Trim(),
                            "</b>" + "\n" +
                            string.Join("\n", RunManager.instance.MoonGetAttributes(__moonLevel)),
                            "</color>"
                        });
                        Plugin.Instance.screenLabelText.SetText(text);
                        // Plugin.Instance.screenLabelText.color = Color.white;
                        Plugin.Instance.screenLabelText.fontSizeMax = 10f;
                        Plugin.Instance.screenLabelText.fontSize = 12f;
                        Plugin.Instance.screenLabel.SetActive(__moonLevel != 0);
                        Plugin.Instance.screenImageTexture.texture = RunManager.instance.MoonGetIcon(__moonLevel);
                        Plugin.Instance.screenImage.SetActive(true);
                    }
                }
                else
                {
                    // тут можешь делать что угодно с этим значением
                    Logger.LogInfo($"MoonLevelField is null. Current level - {__instance.Level}");
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