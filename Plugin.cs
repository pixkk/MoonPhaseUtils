using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoonPhaseUtils.patches;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoonPhaseUtils;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    static Harmony harmony;
    public static Plugin Instance { get; private set; }
    private static readonly object _updateLock = new();
    
    public GameObject? screenLabel;
    public GameObject? screenImage;
    public TextMeshProUGUI? screenLabelText;
    public RawImage? screenImageTexture;
    private void Awake()
    {
        // Plugin startup logic 
        Logger = base.Logger;
        
        Instance = this;
        
        // Prevent the plugin from being deleted
        gameObject.transform.parent = null;
        gameObject.hideFlags = HideFlags.HideAndDontSave;
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
    public void SetupLabel()
    {
        lock (_updateLock)
        {
            if (screenLabel == null) {
                
                
                
                Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}: Creating label...");

                GameObject hud = GameObject.Find("Game Hud");
                GameObject haul = GameObject.Find("Tax Haul");

                if (hud == null || haul == null)
                {
                    Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}: Error getting HUD or Haul - not setting up label :(");
                    return;
                }
				
                TMP_FontAsset font = haul.GetComponent<TMP_Text>().font;
                screenLabel = new GameObject();
                screenLabel.SetActive(false);
                screenLabel.name = "ListDeadPlayers";
                screenLabel.AddComponent<TextMeshProUGUI>();

                screenLabelText = screenLabel.GetComponent<TextMeshProUGUI>();
                screenLabelText.font = font;
                // screenLabelText.color = Color.red;
                screenLabelText.fontSize = 24f;
                screenLabelText.enableWordWrapping = true;
                screenLabelText.overflowMode = TextOverflowModes.Overflow;
                screenLabelText.alignment = TextAlignmentOptions.BottomRight;
                screenLabelText.horizontalAlignment = HorizontalAlignmentOptions.Right;
                screenLabelText.verticalAlignment = VerticalAlignmentOptions.Bottom;
                screenLabelText.SetText("");
                screenLabel.transform.SetParent(hud.transform, false);

                RectTransform component = screenLabel.GetComponent<RectTransform>();

                component.anchorMax = new Vector2(1f, 0f);
                component.anchorMin = new Vector2(1f, 0f);
                component.anchoredPosition = new Vector2(-50f, 50f);
                component.pivot = new Vector2(1f, 0f);
                component.sizeDelta = new Vector2(300f, 400f);
            }
        }
    }
    public void SetupImage()
    {
        lock (_updateLock)
        {
            if (screenImage == null)
            {
                Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}: Creating image...");

                GameObject hud = GameObject.Find("Game Hud");
                GameObject haul = GameObject.Find("Tax Haul");

                if (hud == null || haul == null)
                {
                    Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID}: Error getting HUD or Haul - not setting up image :(");
                    return;
                }


                screenImage = new GameObject("MoonImage");
                screenImage.SetActive(false);

                // Добавляем компонент RawImage
                screenImageTexture = screenImage.AddComponent<RawImage>();
                // screenImageTexture.texture = texture;
                screenImageTexture.color = Color.white;

                // Добавляем в иерархию
                screenImage.transform.SetParent(hud.transform, false);

                // Настройка позиции и размеров
                RectTransform rectTransform = screenImage.GetComponent<RectTransform>();

// Привязка к верхнему правому углу
                rectTransform.anchorMin = new Vector2(1f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.pivot = new Vector2(1f, 1f);

// Задаём отступы: 60px от правого и 20px от верхнего
                rectTransform.anchoredPosition = new Vector2(-70f, -10f);

// Размер сохраняется
                rectTransform.sizeDelta = new Vector2(30f, 30f);

                // screenImage.SetActive(true); // Активируем после настройки
            }
        }
    }

}   