namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class RhythmMiniGame : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject buttonPrefab; 
        public Transform spawnParent;  
        public Slider progressSlider;  

        [Header("Game Settings")]
        public float gameDuration = 5.0f;  
        public int totalNotes = 10;        

        private float timeBetweenNotes;

        private float elapsedTime;

        private bool gameActive = false;

        public delegate void MiniGameResult(float score); // Percentage score: 0–100
        public MiniGameResult onMiniGameCompleted;

        public int numberNotesPlayed = 0;
        private float totalScore = 0f; 

        private void Start()
        {
            progressSlider.maxValue = gameDuration;
        }

        public void StartRhythmGame()
        {
            gameActive = true;
            elapsedTime = 0f;
            totalScore = 0f;
            timeBetweenNotes = gameDuration / totalNotes;
            StartCoroutine(SpawnNotes());
        }

        private void Update()
        {
            if (!gameActive) return;

            elapsedTime += Time.deltaTime;
            progressSlider.value = elapsedTime;

            if (elapsedTime >= gameDuration + 3f)
            {
                EndMiniGame();
            }
        }

        private IEnumerator SpawnNotes()
        {
            for (int i = 0; i < totalNotes; i++)
            {
                Vector3 randomPosition = new Vector3(
                    Random.Range(-100f, 100f),  
                    Random.Range(-50f, 50f),   
                    0                          
                );
            
                GameObject note = Instantiate(buttonPrefab, spawnParent);
                note.transform.localPosition = randomPosition;

                // Hook into the note's judgment system to record scores
                NoteScript noteScript = note.GetComponent<NoteScript>();
                if (noteScript != null)
                {
                    noteScript.onNoteCompleted = OnNoteCompleted;
                }
                

                yield return new WaitForSeconds(timeBetweenNotes);
            }
        }

        private void OnNoteCompleted(Judgment judgment)
        {
            switch (judgment)
            {
                case Judgment.Perfect:
                    totalScore += 100f;
                    break;
                case Judgment.Good:
                    totalScore += 65f;
                    break;
                case Judgment.Meh:
                    totalScore += 35f;
                    break;
                case Judgment.Miss:
                    totalScore += 0f; 
                    break;
            }

            numberNotesPlayed++;
            if (numberNotesPlayed> totalNotes)
            {
                EndMiniGame();
            }
        }

        private void EndMiniGame()
        {
            gameActive = false;
            StopAllCoroutines(); 

            // Calculate final normalized score
            float maxPossibleScore = totalNotes * 100f; 
            float finalModifier = (totalScore / maxPossibleScore); 

            onMiniGameCompleted?.Invoke(finalModifier); 
            Debug.Log($"Mini-Game Completed! Final Score: {finalModifier:F3}%");
        }
    }
}
}