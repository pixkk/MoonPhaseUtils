namespace MoonPhaseUtils.UI;

using UnityEngine;
using UnityEngine.UI;

public class TopUIBanner : MonoBehaviour
{
    public static void Show(string moonName, Texture moonIcon)
    {
        
                    GameObject bannerGO = new GameObject("ModUIBanner");
                    Canvas canvas = bannerGO.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    bannerGO.AddComponent<CanvasScaler>();
                    bannerGO.AddComponent<GraphicRaycaster>();

                    GameObject panelGO = new GameObject("TopPanel");
                    panelGO.transform.SetParent(canvas.transform);
                    RectTransform panelRT = panelGO.AddComponent<RectTransform>();
                    panelRT.anchorMin = new Vector2(0, 1);
                    panelRT.anchorMax = new Vector2(1, 1);
                    panelRT.pivot = new Vector2(0.5f, 1);
                    panelRT.anchoredPosition = new Vector2(0, 0);
                    panelRT.sizeDelta = new Vector2(0, 100);

                    Image panelImage = panelGO.AddComponent<Image>();
                    panelImage.color = new Color(0, 0, 0, 0.6f);

                    GameObject textGO = new GameObject("TopText");
                    textGO.transform.SetParent(panelGO.transform);
                    Text text = textGO.AddComponent<Text>();
                    text.text = $"Current moon: {moonName}";
                    text.alignment = TextAnchor.MiddleLeft;
                    text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    text.color = Color.white;
                    RectTransform textRT = text.GetComponent<RectTransform>();
                    textRT.anchorMin = new Vector2(0, 0);
                    textRT.anchorMax = new Vector2(0.7f, 1);
                    textRT.offsetMin = new Vector2(10, 0);
                    textRT.offsetMax = new Vector2(0, 0);

                    GameObject imageGO = new GameObject("TopImage");
                    imageGO.transform.SetParent(panelGO.transform);
                    Image img = imageGO.AddComponent<Image>();
                    Texture2D texture = moonIcon as Texture2D;
                    img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    RectTransform imgRT = img.GetComponent<RectTransform>();
                    imgRT.anchorMin = new Vector2(0.7f, 0);
                    imgRT.anchorMax = new Vector2(1f, 1);
                    imgRT.offsetMin = new Vector2(0, 10);
                    imgRT.offsetMax = new Vector2(-10, -10);
    }
}
