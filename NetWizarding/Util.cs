using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NetWizarding
{
    public class Util
    {
        // Token: 0x04004B67 RID: 19303
        public static GameObject inputUI;

        // Token: 0x04004B68 RID: 19304
        private static InputField ipInputField;

        public static string ConsumeIPInput()
        {
            if (!ipInputField)
            {
                return null;
            }
            var ret = ipInputField.text.Trim();

            UnityEngine.Object.Destroy(inputUI);
            inputUI = null;
            return ret;
        }

        // Token: 0x06004304 RID: 17156 RVA: 0x0023C248 File Offset: 0x0023A448
        public static void ToggleIPInput()
        {
            Debug.Log("NetBoot: ToggleIPInput called; inputUI is " + ((inputUI == null) ? "null" : "exists"));
            if (inputUI != null)
            {
                UnityEngine.Object.Destroy(inputUI);
                inputUI = null;
                return;
            }
            inputUI = new GameObject("IPInputUI");
            UnityEngine.Object.DontDestroyOnLoad(inputUI);
            inputUI.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            inputUI.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            inputUI.AddComponent<GraphicRaycaster>();
            if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject gameObject = new GameObject("EventSystem");
                gameObject.AddComponent<EventSystem>();
                gameObject.AddComponent<StandaloneInputModule>();
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
            GameObject gameObject2 = new GameObject("Panel");
            gameObject2.transform.SetParent(inputUI.transform, false);
            gameObject2.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);
            RectTransform component = gameObject2.GetComponent<RectTransform>();
            component.sizeDelta = new Vector2(500f, 100f);
            Vector2 vector = new Vector2(0.5f, 0.5f);
            component.anchorMax = vector;
            component.anchorMin = vector;
            component.pivot = new Vector2(0.5f, 0.5f);
            component.anchoredPosition = Vector2.zero;
            GameObject gameObject3 = new GameObject("IPInputField");
            gameObject3.transform.SetParent(gameObject2.transform, false);
            RectTransform rectTransform = gameObject3.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(450f, 60f);
            vector = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = vector;
            rectTransform.anchorMin = vector;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            ipInputField = gameObject3.AddComponent<InputField>();
            ipInputField.textComponent = CreateText(gameObject3.transform, "", TextAnchor.MiddleLeft, Color.white, 24);
            ipInputField.placeholder = CreateText(gameObject3.transform, "Enter IP to connect...", TextAnchor.MiddleLeft, new Color(0.6f, 0.6f, 0.6f), 24);
            ipInputField.ActivateInputField();
            ipInputField.Select();
        }

        // Token: 0x0600430B RID: 17163 RVA: 0x0023C904 File Offset: 0x0023AB04
        private static Text CreateText(Transform parent, string content, TextAnchor align, Color color, int size)
        {
            GameObject gameObject = new GameObject("Text");
            gameObject.transform.SetParent(parent, false);
            Text text = gameObject.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = align;
            text.color = color;
            RectTransform component = gameObject.GetComponent<RectTransform>();
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.offsetMin = (component.offsetMax = Vector2.zero);
            return text;
        }
    }
}
