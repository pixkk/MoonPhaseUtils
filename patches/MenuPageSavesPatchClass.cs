using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace MoonPhaseUtils.patches;

public class MenuPageSavesPatchClass
{
    internal static ManualLogSource Logger = Plugin.Logger;

    [HarmonyPrefix]
    private static bool SaveFileSelectedPatched(MenuPageSaves __instance, string saveFolderName, List<string> saveFileBackups)
    {
        MenuElementAnimations component = __instance.saveFileInfo.GetComponent<MenuElementAnimations>();
        component.UIAniNudgeX();
        component.UIAniRotate();
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

                __instance.saveFileHeader.text = __instance.localizedCorruptedHeader?.GetLocalizedString() ?? "Corrupted Save File";
                __instance.saveFileHeader.color = new Color(1f, 0.0f, 0f);
                __instance.saveFileHeaderDate.text = ":(";
                __instance.saveFileInfoRow1.text = __instance.localizedCorruptedInfoRow1?.GetLocalizedString() ?? "Sorry!";
                __instance.saveFileInfoRow2.text = "";
                __instance.saveFileInfoMoonRect.gameObject.SetActive(false);
                __instance.saveFileInfoRow3.text = __instance.localizedCorruptedInfoRow3?.GetLocalizedString() ?? "Press \"Delete Save\" to delete \nentire saves folder.";
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
        
        GameObject gameObject = __instance.saveFileInfoLoadButton.activeSelf ? __instance.saveFileInfoLoadButton : (__instance.saveFileInfoRestoreButton.activeSelf ? __instance.saveFileInfoRestoreButton : (GameObject) null);
        MenuSelectableElement selectableElement1 = (bool) (Object) gameObject ? GetField("menuSelectableElement", gameObject.GetComponent<MenuButton>(), typeof(MenuButton)) as MenuSelectableElement : (MenuSelectableElement) null;
        if ((bool) (Object) selectableElement1 && (SemiFunc.InputMode() == InputModes.Gamepad || GetField("isKeyboardNavigating", MenuManager.instance, typeof(MenuManager)) is true))
            SetField("lastHoverMenuID", GetField("menuID", selectableElement1, typeof(MenuSelectableElement)), MenuManager.instance, typeof(MenuManager));
        MenuSelectableElement selectableElement2 = GetField("menuSelectableElement", __instance.saveFileRenameButton.GetComponent<MenuButton>(), typeof(MenuButton)) as MenuSelectableElement;
        if ((bool) (Object) selectableElement2)
        {
            Navigation navigation = selectableElement2.navigation with
            {
                selectOnDown = (Selectable) selectableElement1
            };
            selectableElement2.navigation = navigation;
        }
        MenuSelectableElement component2 = ((List<MenuElementSaveFile>)GetField("saveFiles", __instance)).FirstOrDefault<MenuElementSaveFile>((Func<MenuElementSaveFile, bool>) (s => ((string)GetField("saveFileName", s)) == saveFolderName))?.GetComponent<MenuSelectableElement>();
        ExecuteMethod("SetRightNavigation", __instance, new object[] { selectableElement2, selectableElement1, component2 });
        if (StatsManager.instance.SaveFileGetRenamed(saveFolderName, str1))
            __instance.saveFileHeader.text = StatsManager.instance.SaveFileGetTeamName(saveFolderName, str1);
        else
            __instance.saveFileHeader.text = __instance.localizedClickToRename?.GetLocalizedString() ?? "(Click to Rename)";
        __instance.saveFileHeader.color = new Color(1f, 0.54f, 0f);
        __instance.saveFileRenameButton.GetComponent<MenuButton>().ResizeToText();
        __instance.saveFileHeaderDate.text = StatsManager.instance.SaveFileGetDateAndTime(saveFolderName,str1);
        __instance.saveFileInfoRow1.text = 
            $"<sprite name=truck>  " +
            $"<color=#{ColorUtility.ToHtmlStringRGB(SemiFunc.ColorDifficultyGet(1f, 20f, (float) result1 + 1f))}>" +
            $"<b>{(result1 + 1).ToString()}</b></color>";
        __instance.saveFileInfoRow1.text += "      "; 
        float timePlayed = StatsManager.instance.SaveFileGetTimePlayed(saveFolderName, str1);
        TextMeshProUGUI saveFileInfoRow1_1 = __instance.saveFileInfoRow1;
        saveFileInfoRow1_1.text += 
            $"{saveFileInfoRow1_1.text}<sprite name=clock>  {SemiFunc.TimeToString(timePlayed, __instance.localizedDurationShort, true, new Color(0.1f, 0.4f, 0.8f), new Color(0.05f, 0.3f, 0.6f))}";
        __instance.saveFileInfoRow1.text += "      ";
        TextMeshProUGUI saveFileInfoRow1_2 = __instance.saveFileInfoRow1;
        saveFileInfoRow1_2.text = $"{saveFileInfoRow1_2.text}<sprite name=$$>  <b>{result2.ToString()}</b><color=#{__instance.hexColorDollarGreen}>k</color>";
        string str2 = SemiFunc.DollarGetString(result3);
        // string htmlStringRgb = ColorUtility.ToHtmlStringRGB(new Color(0.2f, 0.5f, 0.3f));
        __instance.saveFileInfoRow2.text = $"<color=#{__instance.hexColorDollarGreen}><sprite name=$$$> {__instance.localizedTotalHaul?.GetLocalizedString() ?? "Total Haul:"}      $ <color=white><b>{str2}</b></color>k</color>";
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


    private static object GetField(string fieldName, object instance = null, Type type = null)
    {
        type ??= typeof(MenuPageSaves);
        var field = AccessTools.Field(type, fieldName);
        if (field != null)
            return field.GetValue(instance);
        return instance == null ? null : AccessTools.Property(type, fieldName)?.GetValue(instance);
    }

    private static object ExecuteMethod(string methodName, object instance = null, object[] parameters = null)
    {
        var method = AccessTools.Method(typeof(MenuPageSaves), methodName);
        return method == null ? null : method.Invoke(instance, parameters);
    }

    private static void SetField(string fieldName, object value, object instance = null, Type type = null)
    {
        var field = AccessTools.Field(type ?? typeof(MenuPageSaves), fieldName);
        if (field != null)
        {
            field.SetValue(instance, value);
        }
    }
}