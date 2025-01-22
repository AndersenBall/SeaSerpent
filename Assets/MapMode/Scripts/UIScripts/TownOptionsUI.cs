using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownOptionsUI : MonoBehaviour
{
    private Town town;
    private TMP_Text townNameUI;
    private TMP_Text townDescriptionUI;
    private Image townImageUI;
    private Transform optionsUI;
    private void Start()
    {
        townNameUI = transform.Find("OptionsContainer/TownNameOP").GetComponent<TMP_Text>();
        townDescriptionUI = transform.Find("OptionsContainer/TownDescriptionOP").GetComponent<TMP_Text>();
        townImageUI = transform.Find("OptionsContainer/ImageContainer/TownImageOP").GetComponent<Image>();
        optionsUI = transform.Find("OptionsContainer");
    }
    public void DisplayOptionsMenu(Town t)
    {
        Time.timeScale = .001f;

        town = t;
        string townName = t.name;
        string townDescription = t.townDescription;
        Sprite townPic = t.townIcon;

        townNameUI.text = townName;
        townDescriptionUI.text = townDescription;
        townImageUI.sprite = townPic;

        optionsUI.gameObject.SetActive(true);

    }

    public void restartTime()
    {
        Time.timeScale = 1;
    }
}
