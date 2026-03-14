using UnityEngine;
using UnityEngine.UI;

namespace MapMode.Scripts.PostBattle
{
    public sealed class PostCombatLootRowUI : MonoBehaviour
    {
        public string ItemId { get; private set; }

        private int maxAmount;
        private InputField amountInput;
        private Text itemLabel;
        private Text placeholderLabel;

        public static PostCombatLootRowUI Create(Transform parent, Font font)
        {
            var rowObject = PostCombatUIController.CreateUIObject("LootRow", parent);
            rowObject.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.05f);

            var rowLayout = rowObject.AddComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 8f;
            rowLayout.padding = new RectOffset(8, 8, 6, 6);
            rowLayout.childControlWidth = false;
            rowLayout.childForceExpandWidth = false;

            rowObject.AddComponent<LayoutElement>().preferredHeight = 40f;

            var row = rowObject.AddComponent<PostCombatLootRowUI>();
            row.Build(font);
            return row;
        }

        public void Bind(string itemId, int availableAmount, int defaultAmount)
        {
            ItemId = itemId;
            maxAmount = Mathf.Max(0, availableAmount);

            amountInput.text = Mathf.Clamp(defaultAmount, 0, maxAmount).ToString();
            placeholderLabel.text = $"0-{maxAmount}";
            itemLabel.text = $"{itemId} (max {maxAmount})";
        }

        public int GetSelectedAmount()
        {
            if (!int.TryParse(amountInput.text, out var selectedAmount))
            {
                selectedAmount = 0;
            }

            return Mathf.Clamp(selectedAmount, 0, maxAmount);
        }

        private void Build(Font font)
        {
            var nameLabelObject = PostCombatUIController.CreateUIObject("ItemLabel", transform);
            itemLabel = nameLabelObject.AddComponent<Text>();
            itemLabel.font = font;
            itemLabel.color = Color.white;
            itemLabel.alignment = TextAnchor.MiddleLeft;

            var labelLayout = nameLabelObject.AddComponent<LayoutElement>();
            labelLayout.preferredWidth = 330f;
            labelLayout.preferredHeight = 28f;

            var inputObject = PostCombatUIController.CreateUIObject("AmountInput", transform);
            inputObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.35f);
            amountInput = inputObject.AddComponent<InputField>();

            var inputLayout = inputObject.AddComponent<LayoutElement>();
            inputLayout.preferredWidth = 120f;
            inputLayout.preferredHeight = 30f;

            var textObject = PostCombatUIController.CreateUIObject("Text", inputObject.transform);
            var text = textObject.AddComponent<Text>();
            text.font = font;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;

            StretchToParent(textObject.GetComponent<RectTransform>(), 8f);

            var placeholderObject = PostCombatUIController.CreateUIObject("Placeholder", inputObject.transform);
            placeholderLabel = placeholderObject.AddComponent<Text>();
            placeholderLabel.font = font;
            placeholderLabel.color = new Color(1f, 1f, 1f, 0.45f);
            placeholderLabel.alignment = TextAnchor.MiddleLeft;

            StretchToParent(placeholderObject.GetComponent<RectTransform>(), 8f);

            amountInput.textComponent = text;
            amountInput.placeholder = placeholderLabel;
            amountInput.contentType = InputField.ContentType.IntegerNumber;
            amountInput.lineType = InputField.LineType.SingleLine;
            amountInput.characterLimit = 8;
            amountInput.onValueChanged.AddListener(OnAmountChanged);
        }

        private void OnAmountChanged(string input)
        {
            if (!int.TryParse(input, out var selectedAmount))
            {
                return;
            }

            var clamped = Mathf.Clamp(selectedAmount, 0, maxAmount);
            if (clamped.ToString() != input)
            {
                amountInput.SetTextWithoutNotify(clamped.ToString());
            }
        }

        private static void StretchToParent(RectTransform rectTransform, float horizontalPadding)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(horizontalPadding, 0f);
            rectTransform.offsetMax = new Vector2(-horizontalPadding, 0f);
        }
    }
}
