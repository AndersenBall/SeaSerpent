using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MapMode.Scripts.PostBattle
{
    public sealed class PostCombatUIController : MonoBehaviour
    {
        private const string GoldLootKey = "gold";

        private Font uiFont;
        private GameObject panelRoot;
        private Text goldRewardLabel;
        private Transform lootContainer;
        private Transform boatsContainer;
        private Button confirmButton;

        private readonly List<PostCombatLootRowUI> lootRows = new List<PostCombatLootRowUI>();
        private readonly List<PostCombatBoatRowUI> boatRows = new List<PostCombatBoatRowUI>();

        private void Awake()
        {
            BuildUIIfNeeded();
            HidePanel();
        }

        private void OnEnable()
        {
            PostCombatFlowService.PostCombatReady += OnPostCombatReady;

            if (PostCombatFlowService.IsPostCombatActive && PostCombatFlowService.CurrentPostCombatData != null)
            {
                OnPostCombatReady(PostCombatFlowService.CurrentPostCombatData);
            }
        }

        private void OnDisable()
        {
            PostCombatFlowService.PostCombatReady -= OnPostCombatReady;
        }

        private void OnPostCombatReady(PostCombatData data)
        {
            if (data == null)
            {
                return;
            }

            BuildUIIfNeeded();
            Populate(data);
            panelRoot.SetActive(true);
        }

        private void Populate(PostCombatData data)
        {
            goldRewardLabel.text = $"Gold Reward: {data.GoldReward}";

            ClearRows();

            foreach (var lootKvp in data.AvailableLoot)
            {
                var row = PostCombatLootRowUI.Create(lootContainer, uiFont);
                var defaultAmount = string.Equals(lootKvp.Key, GoldLootKey, StringComparison.OrdinalIgnoreCase)
                    ? lootKvp.Value
                    : 0;

                row.Bind(lootKvp.Key, lootKvp.Value, defaultAmount);
                lootRows.Add(row);
            }

            foreach (var boat in data.CapturableBoats)
            {
                if (boat == null)
                {
                    continue;
                }

                var row = PostCombatBoatRowUI.Create(boatsContainer, uiFont);
                row.Bind(boat.boatName, false);
                boatRows.Add(row);
            }
        }

        private void OnConfirmPressed()
        {
            var selectedLoot = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in lootRows)
            {
                if (row == null)
                {
                    continue;
                }

                selectedLoot[row.ItemId] = row.GetSelectedAmount();
            }

            var selectedBoats = new List<string>();
            foreach (var row in boatRows)
            {
                if (row != null && row.IsSelected)
                {
                    selectedBoats.Add(row.BoatName);
                }
            }

            PostCombatFlowService.ResolvePostCombat(new PostCombatSelection(selectedLoot, selectedBoats));

            ClearRows();
            HidePanel();
        }

        private void BuildUIIfNeeded()
        {
            if (panelRoot != null)
            {
                return;
            }

            uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            var canvas = EnsureCanvas();
            EnsureEventSystem();

            panelRoot = CreateUIObject("PostCombatPanel", canvas.transform);
            var panelImage = panelRoot.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.85f);

            var panelRect = panelRoot.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.15f, 0.1f);
            panelRect.anchorMax = new Vector2(0.85f, 0.9f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            var panelLayout = panelRoot.AddComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(20, 20, 20, 20);
            panelLayout.spacing = 12f;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = false;
            panelLayout.childForceExpandHeight = false;

            panelRoot.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            CreateLabel("Post Combat", panelRoot.transform, 32, TextAnchor.MiddleCenter);
            goldRewardLabel = CreateLabel("Gold Reward: 0", panelRoot.transform, 22, TextAnchor.MiddleLeft);

            CreateLabel("Available Loot", panelRoot.transform, 24, TextAnchor.MiddleLeft);
            lootContainer = CreateListContainer("LootContainer", panelRoot.transform);

            CreateLabel("Capturable Boats", panelRoot.transform, 24, TextAnchor.MiddleLeft);
            boatsContainer = CreateListContainer("BoatsContainer", panelRoot.transform);

            var buttonObj = CreateUIObject("ConfirmButton", panelRoot.transform);
            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.18f, 0.4f, 0.18f, 1f);

            confirmButton = buttonObj.AddComponent<Button>();
            confirmButton.onClick.AddListener(OnConfirmPressed);

            var buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0f, 48f);
            buttonObj.AddComponent<LayoutElement>().preferredHeight = 48f;

            CreateLabel("Confirm", buttonObj.transform, 20, TextAnchor.MiddleCenter);
        }

        private void HidePanel()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(false);
            }
        }

        private void ClearRows()
        {
            foreach (var row in lootRows)
            {
                if (row != null)
                {
                    Destroy(row.gameObject);
                }
            }

            foreach (var row in boatRows)
            {
                if (row != null)
                {
                    Destroy(row.gameObject);
                }
            }

            lootRows.Clear();
            boatRows.Clear();
        }

        private Canvas EnsureCanvas()
        {
            var existingCanvas = FindObjectOfType<Canvas>();
            if (existingCanvas != null)
            {
                return existingCanvas;
            }

            var canvasObject = new GameObject("PostCombatCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            DontDestroyOnLoad(eventSystemObject);
        }

        private Transform CreateListContainer(string objectName, Transform parent)
        {
            var container = CreateUIObject(objectName, parent);

            var layout = container.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 6f;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;

            var contentSize = container.AddComponent<ContentSizeFitter>();
            contentSize.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            container.AddComponent<LayoutElement>().preferredHeight = 120f;
            return container.transform;
        }

        private Text CreateLabel(string text, Transform parent, int fontSize, TextAnchor alignment)
        {
            var labelObject = CreateUIObject($"Label_{text}", parent);
            var label = labelObject.AddComponent<Text>();
            label.font = uiFont;
            label.text = text;
            label.fontSize = fontSize;
            label.color = Color.white;
            label.alignment = alignment;

            var layoutElement = labelObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = fontSize + 12f;

            return label;
        }

        internal static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }
    }
}
