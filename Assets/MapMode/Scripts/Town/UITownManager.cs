﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor.SearchService;
using SlimUI.ModernMenu;

public class UITownManager : MonoBehaviour {
	private Animator CameraObject;

	[Header("LoadScreen")]
	[Tooltip("Button prefab for loads")]
    public GameObject buttonPrefab;
	[Tooltip("Folder For Saves")]
    public string saveFolderPath = "/saves/";

    // campaign button sub menu
    [Header("MENUS")]
    [Tooltip("The Menu for when the MAIN menu buttons")]
    public GameObject mainMenu;
    [Tooltip("THe first list of buttons")]
    public GameObject firstMenu;
    [Tooltip("The Menu for when the PLAY button is clicked")]

    public GameObject shipSelect;

    public enum Theme {custom1, custom2, custom3};
    [Header("THEME SETTINGS")]
    public Theme theme;
    private int themeIndex;
    public ThemedUIData themeController;


	[Header("SFX")]
    [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
    public AudioSource hoverSound;
    [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
    public AudioSource sliderSound;
    [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
    public AudioSource swooshSound;

	void Start(){
		CameraObject = transform.GetComponent<Animator>();

		firstMenu.SetActive(true);
		mainMenu.SetActive(true);
        LoadBoats();

		SetThemeColors();
	}

	void SetThemeColors()
	{
		switch (theme)
		{
			case Theme.custom1:
				themeController.currentColor = themeController.custom1.graphic1;
				themeController.textColor = themeController.custom1.text1;
				themeIndex = 0;
				break;
			case Theme.custom2:
				themeController.currentColor = themeController.custom2.graphic2;
				themeController.textColor = themeController.custom2.text2;
				themeIndex = 1;
				break;
			case Theme.custom3:
				themeController.currentColor = themeController.custom3.graphic3;
				themeController.textColor = themeController.custom3.text3;
				themeIndex = 2;
				break;
			default:
				Debug.Log("Invalid theme selected.");
				break;
		}
	}

    public void BuyBoat() {
        float cost = 1;
        Boat b = new("cool", BoatType.Frigate);
        if (PlayerGlobal.BuyItem(cost)){
            SceneTransfer.playerFleet.AddBoat(b);
        }
        return;
    }

    public void LoadBoats()
    {
        Transform verticalLayoutParent = shipSelect.transform.Find("VerticalLayout");

        

        // Iterate through each value in the BoatType enum
        foreach (BoatType boatType in System.Enum.GetValues(typeof(BoatType)))
        {
            // Create a new button
            GameObject newButton = Instantiate(buttonPrefab, verticalLayoutParent);
            newButton.name = "Btn_" + boatType;

            // Set the button text to the boat type name
            TMP_Text buttonText = newButton.transform.Find("Text").GetComponent<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = boatType.ToString();
            }

            // Optionally, add a click listener to the button
            Button button = newButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnBoatTypeButtonClicked(boatType));
            }
        }
    }

    private void OnBoatTypeButtonClicked(BoatType boatType)
{
    Debug.Log($"Selected boat type: {boatType}");
    // Perform additional logic here, such as spawning a boat or loading details for the selected boat type
}

    private void OnLoadGameButtonClicked(string folderName)
    {
        Debug.Log("Selected save folder: " + folderName);

        // Set the save folder
        SceneTransferMainMenu.SetSaveFolder(folderName);

        // Load the IslandView scene
        StartCoroutine(LoadAsynchronously("IslandView"));
    }

    public void PlayCampaignMobile(){

		mainMenu.SetActive(false);
	}


	public void LoadScene(string scene){
		if(scene != ""){
			StartCoroutine(LoadAsynchronously(scene));
		}
	}




	public void Position1(){
		CameraObject.SetFloat("Animate",0);
	}




	public void PlayHover(){
		hoverSound.Play();
	}

	public void PlaySFXHover(){
		sliderSound.Play();
	}

	public void PlaySwoosh(){
		swooshSound.Play();
	}



	public void QuitGame(){
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}

    // Load Bar synching animation
    IEnumerator LoadAsynchronously(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevent automatic activation initially

        // Adjust UI to show loading screen
        //mainCanvas.SetActive(false); // Hide the main canvas
        //loadingMenu.SetActive(true); // Show the loading menu

        // Loop until the scene is loaded
        while (!operation.isDone)
        {
            // Calculate loading progress (progress ranges from 0 to 0.9 before activation)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //loadingBar.value = progress;

            Debug.Log($"Loading progress: {progress * 100}%");

            // If loading is complete (operation.progress >= 0.9f), activate the scene
            if (operation.progress >= 0.9f)
            {
                Debug.Log("Scene loading complete. Activating...");
                operation.allowSceneActivation = true; // Allow scene activation
            }

            yield return null; // Wait for the next frame
        }

        Debug.Log($"Scene {sceneName} loaded and activated.");
    }

}
