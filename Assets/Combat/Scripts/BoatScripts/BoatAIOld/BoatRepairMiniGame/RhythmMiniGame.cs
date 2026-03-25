using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    public class RhythmMiniGame : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private Slider progressSlider;

        [Header("Game Settings")]
        [SerializeField] private float gameDuration = 5.0f;
        [SerializeField] private int totalNotes = 10;

        private float timeBetweenNotes;
        private float elapsedTime;
        private bool gameActive;
        private bool gameFinished;

        public delegate void MiniGameResult(float score);
        public MiniGameResult onMiniGameCompleted;

        private int numberNotesPlayed;
        private float totalScore;

        public void StartRhythmGame()
        {
            if (buttonPrefab == null || totalNotes <= 0)
            {
                Debug.LogWarning("RhythmMiniGame requires a button prefab and at least one note.");
                onMiniGameCompleted?.Invoke(0f);
                return;
            }

            gameActive = true;
            gameFinished = false;
            elapsedTime = 0f;
            totalScore = 0f;
            numberNotesPlayed = 0;
            timeBetweenNotes = totalNotes >= 20 ? 0.25f : 0.5f;
            gameDuration = timeBetweenNotes * totalNotes;

            if (progressSlider != null)
            {
                progressSlider.maxValue = gameDuration;
                progressSlider.value = 0f;
            }

            StartCoroutine(SpawnNotes());
        }

        private void Update()
        {
            if (!gameActive)
            {
                return;
            }

            elapsedTime += Time.deltaTime;
            if (progressSlider != null)
            {
                progressSlider.value = Mathf.Min(elapsedTime, gameDuration);
            }

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
                    0f
                );

                GameObject note = Instantiate(buttonPrefab, transform);
                note.transform.localPosition = randomPosition;

                NoteScript noteScript = note.GetComponent<NoteScript>();
                if (noteScript != null)
                {
                    noteScript.onNoteCompleted += OnNoteCompleted;
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
                    break;
            }

            numberNotesPlayed++;
            if (numberNotesPlayed >= totalNotes)
            {
                EndMiniGame();
            }
        }

        private void EndMiniGame()
        {
            if (gameFinished)
            {
                return;
            }

            gameFinished = true;
            gameActive = false;
            StopAllCoroutines();

            float maxPossibleScore = totalNotes * 100f;
            float finalModifier = maxPossibleScore <= 0f ? 0f : totalScore / maxPossibleScore;

            onMiniGameCompleted?.Invoke(finalModifier);
            Debug.Log($"Mini-Game Completed! Final Score: {finalModifier:F3}");
        }
    }
}
