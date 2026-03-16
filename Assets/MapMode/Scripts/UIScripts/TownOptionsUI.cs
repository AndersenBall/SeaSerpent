using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownOptionsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text townNameUI;
    [SerializeField] private TMP_Text townDescriptionUI;
    [SerializeField] private Image townImageUI;
    [SerializeField] private GameObject optionsContainer;

    public void DisplayOptionsMenu(Town town)
    {
        if (town == null)
        {
            return;
        }

        Time.timeScale = .001f;

        townNameUI.text = town.name;
        townDescriptionUI.text = town.townDescription;
        townImageUI.sprite = town.townIcon;

        optionsContainer.SetActive(true);
    }

    public void restartTime()
    {
        Time.timeScale = 1;
    }
}
