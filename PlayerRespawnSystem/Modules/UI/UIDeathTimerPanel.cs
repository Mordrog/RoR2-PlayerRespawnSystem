using UnityEngine;
using UnityEngine.UI;

namespace PlayerRespawnSystem
{
    class UIDeathTimerPanel : MonoBehaviour
    {
        public RectTransform rectTransform;
        public RectTransform colorRectTransform;
        public RoR2.UI.HGTextMeshProUGUI textContext1;
        public RoR2.UI.HGTextMeshProUGUI textContext2;

        public bool show = false;
        private float fontSize = 30;
        private float showSpeed = 0.05f;
        private float hideSpeed = 0.1f;

        public void Awake()
        {
            rectTransform = base.gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.one / 2;
            rectTransform.anchorMax = Vector2.one / 2;
            rectTransform.pivot = Vector2.one / 2;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.position = Vector3.zero;
            rectTransform.sizeDelta = new Vector2(300, 60);
            rectTransform.localScale = Vector3.one;

            var color = new GameObject();
            color.transform.SetParent(transform);
            colorRectTransform = color.AddComponent<RectTransform>();
            var image = color.AddComponent<Image>();
            image.rectTransform.localPosition = Vector3.zero;
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.pivot = Vector2.one / 2;
            image.color = new Color(0, 0, 0, 0.5f);

            var text1 = new GameObject();
            text1.transform.SetParent(transform);
            textContext1 = text1.AddComponent<RoR2.UI.HGTextMeshProUGUI>();
            textContext1.rectTransform.localPosition = Vector3.zero;
            textContext1.rectTransform.anchorMin = Vector2.zero;
            textContext1.rectTransform.anchorMax = Vector2.one;
            textContext1.rectTransform.pivot = Vector2.one / 2;
            textContext1.enableAutoSizing = false;
            textContext1.fontSizeMax = fontSize;
            textContext1.color = Color.white;
            textContext1.alignment = TMPro.TextAlignmentOptions.Top;
            textContext1.text = "You will respawn";

            var text2 = new GameObject();
            text2.transform.SetParent(transform);
            textContext2 = text2.AddComponent<RoR2.UI.HGTextMeshProUGUI>();
            textContext2.rectTransform.localPosition = Vector3.zero;
            textContext2.rectTransform.anchorMin = Vector2.zero;
            textContext2.rectTransform.anchorMax = Vector2.one;
            textContext2.rectTransform.pivot = Vector2.one / 2;
            textContext2.enableAutoSizing = false;
            textContext2.fontSizeMax = fontSize;
            textContext2.color = Color.white;
            textContext2.alignment = TMPro.TextAlignmentOptions.Midline;
            textContext2.text = "in x seconds";
        }

        public void OnEnable()
        {
            rectTransform.anchoredPosition = new Vector2(0, 100);
        }

        public void FixedUpdate()
        {
            if (show && textContext1.fontSize != fontSize && textContext2.fontSize != fontSize && colorRectTransform.localScale != Vector3.one)
            {
                textContext1.fontSize = Mathf.Lerp(textContext1.fontSize, fontSize, showSpeed);
                textContext2.fontSize = Mathf.Lerp(textContext2.fontSize, fontSize, showSpeed);
                colorRectTransform.localScale = Vector2.Lerp(colorRectTransform.localScale, Vector3.one, showSpeed);
            }
            else if (!show && textContext1.fontSize != 0 && textContext2.fontSize != 0 && colorRectTransform.localScale != Vector3.up)
            {
                textContext1.fontSize = Mathf.Lerp(textContext1.fontSize, 0, hideSpeed);
                textContext2.fontSize = Mathf.Lerp(textContext2.fontSize, 0, hideSpeed);
                colorRectTransform.localScale = Vector2.Lerp(colorRectTransform.localScale, Vector3.up, hideSpeed);
            }
        }
    }
}
