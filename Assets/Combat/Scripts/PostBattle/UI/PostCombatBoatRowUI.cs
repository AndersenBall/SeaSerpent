using UnityEngine;
using UnityEngine.UI;

namespace MapMode.Scripts.PostBattle
{
    public sealed class PostCombatBoatRowUI : MonoBehaviour
    {
        public string BoatName { get; private set; }
        public bool IsSelected => selectionToggle != null && selectionToggle.isOn;

        private Toggle selectionToggle;
        private Text label;

        public static PostCombatBoatRowUI Create(Transform parent, Font font)
        {
            var rowObject = PostCombatUIController.CreateUIObject("BoatRow", parent);
            rowObject.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.05f);

            var layout = rowObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(8, 8, 6, 6);
            layout.spacing = 10f;
            layout.childControlWidth = false;
            layout.childForceExpandWidth = false;

            rowObject.AddComponent<LayoutElement>().preferredHeight = 40f;

            var row = rowObject.AddComponent<PostCombatBoatRowUI>();
            row.Build(font);
            return row;
        }

        public void Bind(string boatName, bool defaultSelected)
        {
            BoatName = boatName;
            label.text = boatName;
            selectionToggle.isOn = defaultSelected;
        }

        private void Build(Font font)
        {
            var toggleObj = PostCombatUIController.CreateUIObject("Toggle", transform);
            toggleObj.AddComponent<LayoutElement>().preferredWidth = 24f;

            var bg = PostCombatUIController.CreateUIObject("Background", toggleObj.transform);
            var bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.45f);

            var checkmark = PostCombatUIController.CreateUIObject("Checkmark", bg.transform);
            var checkmarkImage = checkmark.AddComponent<Image>();
            checkmarkImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);

            FitRect(bg.GetComponent<RectTransform>());
            FitRect(checkmark.GetComponent<RectTransform>(), 4f);

            selectionToggle = toggleObj.AddComponent<Toggle>();
            selectionToggle.targetGraphic = bgImage;
            selectionToggle.graphic = checkmarkImage;

            var textObj = PostCombatUIController.CreateUIObject("BoatLabel", transform);
            label = textObj.AddComponent<Text>();
            label.font = font;
            label.color = Color.white;
            label.alignment = TextAnchor.MiddleLeft;

            textObj.AddComponent<LayoutElement>().preferredWidth = 350f;
        }

        private static void FitRect(RectTransform rectTransform, float padding = 0f)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(padding, padding);
            rectTransform.offsetMax = new Vector2(-padding, -padding);
        }
    }
}
