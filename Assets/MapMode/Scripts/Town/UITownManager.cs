using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor.SearchService;
using SlimUI.ModernMenu;
using UnityEditor.ShaderGraph.Internal;
using static PlayerFleetMapController;

public class UITownManager : MonoBehaviour {
    private static readonly int Animate = Animator.StringToHash("Animate");

    #region Variables
    private Animator CameraObject;
    private Vector3 loadedPosition;

    public string[] pirateAdjectives = {
        "Black", "Bloody", "Crimson", "Golden", "Iron", "Salty", "Stormy", "Vengeful", "Wicked", "Ghostly"
    };

    public string[] pirateShipNouns = {
        "Galleon", "Raven", "Corsair", "Buccaneer", "Leviathan", "Voyager", "Cutlass", "Siren", "Wraith", "Tempest"
    };

    [Header("LoadScreen")]
    [Tooltip("Button prefab for loads")]
    public GameObject buttonPrefab;
    [Tooltip("Folder For Saves")] 
    public string saveFolderPath = "/saves/";

    [Header("MENUS")]
    [Tooltip("The Menu for when the MAIN menu buttons")]
    public GameObject mainMenu;

    public enum Theme {custom1, custom2, custom3};

    [Header("Dock")] 
    public GameObject dockShipSelect;
    
    [Header("Ship Select")]
    [Tooltip("The list of ships to select from")]
    public GameObject shipSelect;
    [SerializeField] private TMP_Text moneyField;
    [SerializeField] private TMP_Text selectedShipText;
    [SerializeField] public TMP_InputField inputField;
    [Tooltip("The list of player ships to select from")] 
    public GameObject playerBoats;
    [SerializeField] private BoatType selectedBoatType = BoatType.Frigate;
    private Boat selectedBoat;
    [SerializeField] private TMP_Text selectedShipField;

    [Header("Sailor Select")] 
    [SerializeField] private Sailor selectedSailor;
    [SerializeField] private SailorType selectedSailorType = SailorType.Gunner;
    [SerializeField] private Boat selectedPlayerShip { get; set; }
    [Tooltip("The list of sailors to select from")]
    public GameObject sailorSelect;
    [Tooltip("The list of player ships to select from")]
    public GameObject sailorBoatInsertSelect;
    [SerializeField] private TMP_Text moneyFieldSailor;
    [SerializeField] private TMP_Text selectedSailorField;
    [SerializeField] private TMP_Text selectedSailorDescription;

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
    #endregion

    #region Monobehaviours
    public void Awake() {
        GameEvents.SaveInitiated += SavePlayerFleet;
        GameEvents.LoadInitiated += LoadPlayerFleet;
    }

    void Start() {
        CameraObject = transform.GetComponent<Animator>();
        mainMenu.SetActive(true);
        SetThemeColors();
        GameEvents.LoadGame();
        RefreshUi();
        LoadBoats();
        LoadSailors();
    }

    private void OnDestroy() {
        GameEvents.SaveInitiated -= SavePlayerFleet;
        GameEvents.LoadInitiated -= LoadPlayerFleet;
    }
    #endregion

    #region Load/Save Methods
    private void LoadPlayerFleet() {
        if (SaveLoad.SaveExists("Player")) {
            PlayerFleetData playerData = SaveLoad.Load<PlayerFleetData>("Player");
            SceneTransfer.playerFleet = playerData.fleet;
            loadedPosition = new Vector3(playerData.pos[0], playerData.pos[1], playerData.pos[2]);
            Debug.Log("Player fleet loaded successfully.");
        }
        else {
            Debug.LogWarning("No saved player fleet data found.");
        }
    }

    private void SavePlayerFleet() {
        PlayerFleetData updatedFleetData = new PlayerFleetData {
            fleet = SceneTransfer.playerFleet,
            pos = new float[] { loadedPosition.x, loadedPosition.y, loadedPosition.z }
        };

        SaveLoad.Save(updatedFleetData, "Player");
        Debug.Log("Player fleet saved successfully.");
    }
    #endregion

    #region Boat Management Methods
    public void BuyBoat() {
        Boat newBoat = new Boat(inputField.text, selectedBoatType);

        Debug.Log($"Attempting to buy: {PlayerGlobal.money} : {newBoat.baseStats.boatCost}");

        if (SceneTransfer.playerFleet.HasBoatWithName(inputField.text) || inputField.text.Length > inputField.characterLimit || inputField.text.Length < 3 || inputField.text.Contains("does not work")) {
            Debug.Log($"A boat named '{inputField.text}' already exists in your fleet.");
            inputField.text = $"{inputField.text} does not work.";
            return;
        }

        if (PlayerGlobal.BuyItem(newBoat.baseStats.boatCost)) {
            SceneTransfer.playerFleet.AddBoat(newBoat);
            Debug.Log("Boat purchased successfully.");
            GameEvents.SaveGame();
            RefreshUi();
        }
        else {
            Debug.Log("Not enough money to buy the boat.");
        }
    }

    public void SellBoat() {
        if (selectedPlayerShip != null) {
            float totalRefund = 0;


            for (int i = selectedPlayerShip.GetSailors().Count - 1; i >= 0; i--) {
                Sailor sailor = selectedPlayerShip.GetSailors()[i];
                
                selectedPlayerShip.RemoveSailor(sailor);
                SellSailor(sailor);
            }

            // Add boat sell value
            totalRefund += selectedPlayerShip.baseStats.boatCost * 0.7f;

            // Remove boat and add money
            SceneTransfer.playerFleet.RemoveBoat(selectedPlayerShip);
            PlayerGlobal.AddMoney(totalRefund);

            Debug.Log($"Sold {selectedPlayerShip.boatName} and all sailors for {totalRefund} gold");
            GameEvents.SaveGame();
            RefreshUi();
            selectedPlayerShip = null;
        }
        else {
            Debug.LogWarning("No boat selected to sell.");
        }
    }

    public void LoadBoats() {
        Transform verticalLayoutParent = shipSelect.transform.Find("VerticalLayout");
        foreach (Transform child in verticalLayoutParent) {
            Destroy(child.gameObject);
        }
        OnBoatTypeButtonClicked(BoatType.Frigate);

        foreach (BoatType boatType in System.Enum.GetValues(typeof(BoatType))) {
            GameObject newButton = Instantiate(buttonPrefab, verticalLayoutParent);
            newButton.name = "Btn_" + boatType;

            TMP_Text buttonText = newButton.transform.Find("Text").GetComponent<TMP_Text>();
            if (buttonText != null) {
                buttonText.text = boatType.ToString();
            }

            Button button = newButton.GetComponent<Button>();
            if (button != null) {
                button.onClick.AddListener(() => OnBoatTypeButtonClicked(boatType));
            }
        }
    }

    private void OnBoatTypeButtonClicked(BoatType boatType) {
        Debug.Log($"Selected boat type: {boatType}");
        selectedBoatType = boatType;
        selectedShipField.text = boatType.ToString().ToUpper();
        selectedBoat = new("cool", selectedBoatType);
        selectedShipText.text = selectedBoat.ToString();
    }
    #endregion

    #region UI Methods
    public void RefreshUi() {
        LoadBoatsSailor();
        UpdateMoney();
        LoadBoatsPlayer();
        LoadBoatsDock();
    }

    public void UpdateMoney() {
        moneyField.text = "Gold: " + PlayerGlobal.money;
        moneyFieldSailor.text = "Gold: " + PlayerGlobal.money; 
    }

    public void RandomBoatName() {
        if (pirateAdjectives.Length > 0 && pirateShipNouns.Length > 0 && inputField != null) {
            string randomAdjective = pirateAdjectives[Random.Range(0, pirateAdjectives.Length)];
            string randomNoun = pirateShipNouns[Random.Range(0, pirateShipNouns.Length)];
            string pirateShipName = $"{randomAdjective} {randomNoun}";

            inputField.text = pirateShipName;
            Debug.Log("Assigned pirate-themed ship name: " + pirateShipName);
        }
    }

    void SetThemeColors() {
        switch (theme) {
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
    #endregion

    #region Sailor Management Methods
    public void LoadSailors() {
        Transform verticalLayoutParent = sailorSelect.transform.Find("VerticalLayout");

        foreach (Transform child in verticalLayoutParent) {
            Destroy(child.gameObject);
        }

        foreach (SailorType sailorType in System.Enum.GetValues(typeof(SailorType))) {
            GameObject newButton = Instantiate(buttonPrefab, verticalLayoutParent);
            newButton.name = "Btn_" + sailorType;

            TMP_Text buttonText = newButton.transform.Find("Text").GetComponent<TMP_Text>();
            if (buttonText != null) {
                buttonText.text = sailorType.ToString();
            }

            Button button = newButton.GetComponent<Button>();
            if (button != null) {
                button.onClick.AddListener(() => OnSailorTypeButtonClicked(sailorType));
            }
        }
    }

    private void OnSailorTypeButtonClicked(SailorType sailorType) {
        Debug.Log($"Selected sailor type: {sailorType}");
        selectedSailorType = sailorType;
        selectedSailorField.text = sailorType.ToString().ToUpper();

        if (SailorStatsDatabase.BaseStats.TryGetValue(sailorType, out SailorStats stats)) {
            selectedSailor = new Sailor("DefaultName", sailorType);
            Debug.Log(selectedSailor.ToString());
            selectedSailorDescription.text = selectedSailor.ToString();
        }
        else {
            Debug.LogError($"No stats found for SailorType {sailorType}");
        }
    }

    public void AddSailorToSelectedShip() {
        if (selectedSailor != null && selectedPlayerShip != null) {
            int sailorCost = selectedSailor.SailorStats.baseCost;

            if (PlayerGlobal.BuyItem(sailorCost) && selectedPlayerShip.sailors.Count < selectedPlayerShip.maxSailorCount) {
                selectedPlayerShip.AddSailor(selectedSailor);
                Debug.Log($"Added {selectedSailor.Name} to {selectedPlayerShip.boatName}. Cost: {sailorCost}");
                GameEvents.SaveGame();
                RefreshUi();
            }
            else {
                Debug.LogWarning("Not enough money to buy the sailor.");
            }
        }
        else {
            Debug.LogWarning("No sailor or ship selected to add the sailor to.");
        }
    }

    public void SellSailor() {
        if (selectedSailor != null && selectedPlayerShip != null) {
            int sailorRefund = (int)(selectedSailor.SailorStats.baseCost * 0.7f);
            selectedPlayerShip.RemoveSailor(selectedSailor);
            PlayerGlobal.AddMoney(sailorRefund);
            Debug.Log($"Sold {selectedSailor.Name} for {sailorRefund} gold");
            GameEvents.SaveGame();
            RefreshUi();
        }
        else {
            Debug.LogWarning("No sailor or ship selected to sell.");
        }
    }

    private void SellSailor(Sailor sailor) {
        int sailorRefund = (int)(sailor.SailorStats.baseCost * 0.7f);
        PlayerGlobal.AddMoney(sailorRefund);
    }

    public void LoadPlayerBoats(Transform verticalLayoutParent) {
        foreach (Transform child in verticalLayoutParent) {
            Destroy(child.gameObject);
        }
        
        foreach (Boat boat in SceneTransfer.playerFleet.GetBoats()) {
            GameObject newButton = Instantiate(buttonPrefab, verticalLayoutParent);
            newButton.name = "Btn_" + boat.boatName;

            TMP_Text buttonText = newButton.transform.Find("Text").GetComponent<TMP_Text>();
            buttonText.text = $"{boat.boatName} ({boat.GetSailors().Count}/{boat.baseStats.maxSailorCount} sailors)";

            Button button = newButton.GetComponent<Button>();
            if (button != null) {
                button.onClick.AddListener(() => OnBoatButtonClickedSailor(boat));
            }
        }
    }

    private void OnBoatButtonClickedSailor(Boat boat) {
        Debug.Log($"Selected boat: {boat.boatName}");
        selectedPlayerShip = boat;
        PositionDock();
    }
    
    public void LoadBoatsSailor() {
        Transform verticalLayoutParent = sailorBoatInsertSelect.transform.Find("VerticalLayout");
        foreach (Transform child in verticalLayoutParent) {
            Destroy(child.gameObject);
        }
        
        foreach (Boat boat in SceneTransfer.playerFleet.GetBoats()) {
            GameObject newButton = Instantiate(buttonPrefab, verticalLayoutParent);
            newButton.name = "Btn_" + boat.boatName;

            TMP_Text buttonText = newButton.transform.Find("Text").GetComponent<TMP_Text>();
            buttonText.text = $"{boat.boatName} ({boat.GetSailors().Count}/{boat.baseStats.maxSailorCount} sailors)";

            Button button = newButton.GetComponent<Button>();
            if (button != null) {
                button.onClick.AddListener(() => {
                    Debug.Log($"Selected boat: {boat.boatName}");
                    selectedPlayerShip = boat;
                });
            }
        }
    }
    
    public void LoadBoatsPlayer() {
        Transform verticalLayoutParent = playerBoats.transform.Find("VerticalLayout"); 
        LoadPlayerBoats(verticalLayoutParent);
    }

    public void LoadBoatsDock()
    {
        Transform verticalLayoutParent = dockShipSelect.transform.Find("VerticalLayout");  
        LoadPlayerBoats(verticalLayoutParent);
    }

    #endregion

    #region Camera Methods
    public void PositionDock() {
        CameraObject.SetFloat(Animate, 2);
    }
    
    public void TavernCamView() {
        CameraObject.SetFloat(Animate, 1);
    }

    public void ShipyardCamView() {
        CameraObject.SetFloat(Animate, 0);
    }
    #endregion

    #region Audio Methods
    public void PlayHover() {
        hoverSound.Play();
    }

    public void PlaySFXHover() {
        sliderSound.Play();
    }

    public void PlaySwoosh() {
        swooshSound.Play();
    }
    #endregion

    #region Scene Management Methods
    public void PlayCampaignMobile() {
        mainMenu.SetActive(false);
    }

    public void LoadScene(string scene) {
        if (scene == "IslandView") {
            SceneTransfer.TransferToMap();
        }
        else if (scene != "") {
            StartCoroutine(LoadAsynchronously(scene));
        }
    }

    IEnumerator LoadAsynchronously(string sceneName) {
        Debug.Log("Loading scene: " + sceneName);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log($"Loading progress: {progress * 100}%");

            if (operation.progress >= 0.9f) {
                Debug.Log("Scene loading complete. Activating...");
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log($"Scene {sceneName} loaded and activated.");
    }

    public void QuitGame() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    #endregion
}