using System.Collections.Generic;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoonPhaseUtils.patches;

public class MenuPageSavesPatchClass
{
    internal static ManualLogSource Logger = Plugin.Logger;

    [HarmonyPrefix]
    private static bool SaveFileSelectedPatched(MenuPageSaves __instance, string saveFolderName, List<string> saveFileBackups)
    {
        MenuElementAnimations component = __instance.saveFileInfo.GetComponent<MenuElementAnimations>();
        component.UIAniNudgeX(10f, 0.2f, 1f);
        component.UIAniRotate(2f, 0.2f, 1f);
        __instance.saveInfoDefault.SetActive(false);
        __instance.saveInfoSelected.SetActive(true);
        Image saveFileInfoPanel = (Image)GetField("saveFileInfoPanel", __instance);
        saveFileInfoPanel.color = new Color(0f, 0.1f, 0.25f, 1f);
        SetField("saveFileInfoPanel", saveFileInfoPanel, __instance);
        SetField("currentSaveFileName", saveFolderName, __instance);
        SetField("currentSaveFileBackups", saveFileBackups, __instance);
        
        string str1 = saveFolderName;
        int result1;
        int result2;
        int result3;
        if (!int.TryParse(StatsManager.instance.SaveFileGetRunLevel(saveFolderName), out result1) || 
            !int.TryParse(StatsManager.instance.SaveFileGetRunCurrency(saveFolderName), out result2) || 
            !int.TryParse(StatsManager.instance.SaveFileGetTotalHaul(saveFolderName), out result3))
        {
            SetField("currentSaveFileValid", false, __instance);
            if (saveFileBackups.Count > 0)
                str1 = saveFileBackups[0];
            if (saveFolderName == str1 || 
                !int.TryParse(StatsManager.instance.SaveFileGetRunLevel(saveFolderName, str1), out result1) || 
                !int.TryParse(StatsManager.instance.SaveFileGetRunCurrency(saveFolderName, str1), out result2) || 
                !int.TryParse(StatsManager.instance.SaveFileGetTotalHaul(saveFolderName, str1), out result3))
            {
                ((GameObject)GetField("saveFileInfoLoadButton", __instance)).SetActive(false);
                ((GameObject)GetField("saveFileInfoRestoreButton", __instance)).SetActive(false);

                __instance.saveFileHeader.text = "CORRUPTED SAVE FILE";
                __instance.saveFileHeader.color = new Color(1f, 0.0f, 0f);
                __instance.saveFileHeaderDate.text = ":(";
                __instance.saveFileInfoRow1.text = "Sorry!";
                __instance.saveFileInfoRow2.text = "";
                __instance.saveFileInfoMoonRect.gameObject.SetActive(false);
                __instance.saveFileInfoRow3.text = "Press \"Delete Save\" to delete \nthis save file.";
                return false;
            }
            ((GameObject)GetField("saveFileInfoLoadButton", __instance)).SetActive(false);
            ((GameObject)GetField("saveFileInfoRestoreButton", __instance)).SetActive(true);
        }
        else
        {
            SetField("currentSaveFileValid", true, __instance);
            ((GameObject)GetField("saveFileInfoLoadButton", __instance)).SetActive(true);
            ((GameObject)GetField("saveFileInfoRestoreButton", __instance)).SetActive(false);
        }
        __instance.saveFileHeader.text = StatsManager.instance.SaveFileGetTeamName(saveFolderName, str1);
        __instance.saveFileHeader.color = new Color(1f, 0.54f, 0f);
        __instance.saveFileHeaderDate.text = StatsManager.instance.SaveFileGetDateAndTime(saveFolderName);
        string str2 = "      ";
        __instance.saveFileInfoRow1.text = 
            $"<sprite name=truck>  " +
            $"<color=#{ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 10f, (float) result1 + 1f))}>" +
            $"<b>{(result1 + 1).ToString()}</b></color>";
        __instance.saveFileInfoRow1.text += str2; 
        float timePlayed = StatsManager.instance.SaveFileGetTimePlayed(saveFolderName, str1);
        TextMeshProUGUI saveFileInfoRow1_1 = __instance.saveFileInfoRow1;
        saveFileInfoRow1_1.text += 
            $"{saveFileInfoRow1_1.text}<sprite name=clock>  {SemiFunc.TimeToString(timePlayed, true, new Color(0.1f, 0.4f, 0.8f), new Color(0.05f, 0.3f, 0.6f))}";
        __instance.saveFileInfoRow1.text += str2;
        string htmlStringRgb = ColorUtility.ToHtmlStringRGB(new Color(0.2f, 0.5f, 0.3f));
        
        TextMeshProUGUI saveFileInfoRow1_2 = __instance.saveFileInfoRow1;
        saveFileInfoRow1_2.text = $"{saveFileInfoRow1_2.text}<sprite name=$$>  <b>{result2.ToString()}</b><color=#{htmlStringRgb}>k</color>";
        string str3 = SemiFunc.DollarGetString(result3);
        __instance.saveFileInfoRow2.text = $"<color=#{htmlStringRgb}><sprite name=$$$> TOTAL HAUL:      <b></b>$ </color><b>{str3}</b><color=#{htmlStringRgb}>k</color>";
        int _moonIndex = Mathf.Clamp(RunManager.instance.CalculateMoonLevel(result1), 0, RunManager.instance.moons.Count);
        if (_moonIndex > 0)
        {
            __instance.saveFileInfoMoonRect.gameObject.SetActive(true);
            __instance.saveFileInfoMoonImage.texture = RunManager.instance.MoonGetIcon(_moonIndex);
        }
        else
            __instance.saveFileInfoMoonRect.gameObject.SetActive(false);
        ExecuteMethod("InfoPlayerNames", __instance, new object[] { __instance.saveFileInfoRow3, saveFolderName, str1 });
        return false;
    }


    private static object GetField(string fieldName, object instance = null)
    {
        var field = AccessTools.Field(typeof(MenuPageSaves), fieldName);
        return field != null ? field.GetValue(instance) : null;
    }

    private static object ExecuteMethod(string methodName, object instance = null, object[] parameters = null)
    {
        var method = AccessTools.Method(typeof(MenuPageSaves), methodName);
        return method == null ? null : method.Invoke(instance, parameters);
    }

    private static void SetField(string fieldName, object value, object instance = null)
    {
        var field = AccessTools.Field(typeof(MenuPageSaves), fieldName);
        if (field != null)
        {
            field.SetValue(instance, value);
        }
    }
}