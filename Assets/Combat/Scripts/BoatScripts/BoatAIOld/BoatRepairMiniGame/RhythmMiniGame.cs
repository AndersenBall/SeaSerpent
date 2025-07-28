namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmMiniGame : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject buttonPrefab; // Prefab for the rhythm buttons
    public Transform spawnParent;  // Parent object for spawned notes
    public Slider progressSlider;  // Slider to show mini-game progress

    [Header("Game Settings")]
    public float gameDuration = 5.0f;  // Duration of the mini-game
    public int totalNotes = 10;       // Number of notes in this session

    private float timeBetweenNotes;
    private List<GameObject> activeNotes = new List<GameObject>();
    private int currentNoteIndex = 0;
    private float elapsedTime;

    private bool gameActive = false;

    public delegate void MiniGameResult(bool success);
    public MiniGameResult onMiniGameCompleted;

    private void Start()
    {
        progressSlider.maxValue = gameDuration;
    }

    public void StartRhythmGame()
    {
        gameActive = true;
        elapsedTime = 0f;
        timeBetweenNotes = gameDuration / totalNotes;
        StartCoroutine(SpawnNotes());
    }

    private void Update()
    {
        if (!gameActive) return;

        elapsedTime += Time.deltaTime;
        progressSlider.value = elapsedTime;

        if (elapsedTime >= gameDuration)
        {
            EndMiniGame(currentNoteIndex >= totalNotes); // Success if all notes were hit
        }

        // Handle player input
        if (Input.GetKeyDown(KeyCode.Space)) // Replace with dynamic key detection
        {
            CheckPlayerInput();
        }
    }

    private IEnumerator SpawnNotes()
    {
        for (int i = 0; i < totalNotes; i++)
        {
            GameObject note = Instantiate(buttonPrefab, spawnParent);
            activeNotes.Add(note);
            yield return new WaitForSeconds(timeBetweenNotes);
        }
    }

    private void CheckPlayerInput()
    {
        if (currentNoteIndex < activeNotes.Count)
        {
            // Check if player hit the correct note or missed
            GameObject note = activeNotes[currentNoteIndex];
            currentNoteIndex++;
            Destroy(note);

            Debug.Log("Note Hit!");
        }
        else
        {
            Debug.Log("Missed or Extra Input!");
        }
    }

    private void EndMiniGame(bool success)
    {
        gameActive = false;
        StopAllCoroutines(); // Stop spawning notes if still active
        foreach (var note in activeNotes)
        {
            Destroy(note);
        }
        activeNotes.Clear();

        onMiniGameCompleted?.Invoke(success); // Notify listeners
        Debug.Log("Mini-Game Completed: " + success);
    }
}
}