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
    private static bool SaveFileSelectedPatched(MenuPageSaves __instance, string saveFileName)
    {
        MenuElementAnimations component = __instance.saveFileInfo.GetComponent<MenuElementAnimations>();
        component.UIAniNudgeX(10f, 0.2f, 1f);
        component.UIAniRotate(2f, 0.2f, 1f);
        __instance.saveInfoDefault.SetActive(false);
        __instance.saveInfoSelected.SetActive(true);
        Image saveFileInfoPanel = (Image)GetField("saveFileInfoPanel", __instance);
        saveFileInfoPanel.color = new Color(0f, 0.1f, 0.25f, 1f);
        SetField("saveFileInfoPanel", saveFileInfoPanel, __instance);
        string text = StatsManager.instance.SaveFileGetTeamName(saveFileName);
        string text2 = StatsManager.instance.SaveFileGetDateAndTime(saveFileName);
        __instance.saveFileHeader.text = text;
        __instance.saveFileHeader.color = new Color(1f, 0.54f, 0f);
        __instance.saveFileHeaderDate.text = text2;
        SetField("currentSaveFileName", saveFileName, __instance);
        string str = "      ";
        float time = StatsManager.instance.SaveFileGetTimePlayed(saveFileName);
        int num = int.Parse(StatsManager.instance.SaveFileGetRunLevel(saveFileName)) + 1;
        string text3 = ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 10f, (float)num));
        string text4 = StatsManager.instance.SaveFileGetRunCurrency(saveFileName);
        __instance.saveFileInfoRow1.text = string.Concat(new string[]
        {
            "<sprite name=truck>  <color=#",
            text3,
            "><b>",
            num.ToString(),
            "</b></color>"
        });
        TextMeshProUGUI textMeshProUGUI = __instance.saveFileInfoRow1;
        textMeshProUGUI.text += str;
        TextMeshProUGUI textMeshProUGUI2 = __instance.saveFileInfoRow1;
        textMeshProUGUI2.text = textMeshProUGUI2.text + "<sprite name=clock>  " +
                                SemiFunc.TimeToString(time, true, new Color(0.1f, 0.4f, 0.8f),
                                    new Color(0.05f, 0.3f, 0.6f));
        TextMeshProUGUI textMeshProUGUI3 = __instance.saveFileInfoRow1;
        textMeshProUGUI3.text += str;
        string text5 = ColorUtility.ToHtmlStringRGB(new Color(0.2f, 0.5f, 0.3f));
        TextMeshProUGUI textMeshProUGUI4 = __instance.saveFileInfoRow1;
        textMeshProUGUI4.text = string.Concat(new string[]
        {
            textMeshProUGUI4.text,
            "<sprite name=$$>  <b>",
            text4,
            "</b><color=#",
            text5,
            ">k</color>"
        });
        string text6 = SemiFunc.DollarGetString(int.Parse(StatsManager.instance.SaveFileGetTotalHaul(saveFileName)));
        __instance.saveFileInfoRow2.text = string.Concat(new string[]
        {
            "<color=#",
            text5,
            "><sprite name=$$$> TOTAL HAUL:      <b></b>$ </color><b>",
            text6,
            "</b><color=#",
            text5,
            ">k</color>"
        });
        int num2 = RunManager.instance.CalculateMoonLevel(num - 1);
        Logger.LogInfo("MoonLevel - " + num2 + " (num-1=" + num + ")");
        num2 = Mathf.Clamp(num2, 0, RunManager.instance.moons.Count);
        if (num2 > 0)
        {
            __instance.saveFileInfoMoonRect.gameObject.SetActive(true);
            __instance.saveFileInfoMoonImage.texture = RunManager.instance.MoonGetIcon(num2);

            Logger.LogInfo("MoonGetIcon - " + num2 + " (texture=" + __instance.saveFileInfoMoonImage.texture + ")");
        }
        else
        {
            __instance.saveFileInfoMoonRect.gameObject.SetActive(false);
        }

        ExecuteMethod("InfoPlayerNames", __instance, new object[] { __instance.saveFileInfoRow3, saveFileName });
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
        if (method == null) return null;
        return method.Invoke(instance, parameters);
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