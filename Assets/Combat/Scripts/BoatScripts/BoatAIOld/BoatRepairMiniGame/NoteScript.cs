using UnityEngine;
using UnityEngine.UI;

namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{

    public class NoteScript : MonoBehaviour
    {
        [Header("Settings")] [SerializeField]
        private float spawnTime = 1.5f; // The time the note exists on screen
        
        [Header("Judgment Timings (ms)")] [SerializeField]
        private float perfectTiming = 0.0395f; 
        
        [SerializeField] private float goodTiming = 0.0975f; 
        [SerializeField] private float mehTiming = 0.1425f; 

        [SerializeField] private Button noteButton; // Button associated with this note

        public delegate void NoteResult(Judgment judgment);

        public NoteResult onNoteCompleted;
        
        private float activationTime;

        
        
        private bool wasHit = false;

        private void Start()
        {
            activationTime = Time.time + spawnTime;
            
            if (noteButton != null){
                noteButton.onClick.AddListener(HitNote);
                noteButton.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            float currentTime = Time.time;


            if (currentTime >= activationTime - mehTiming)
            {
                noteButton.gameObject.SetActive(true);
            }

            if (currentTime >= activationTime + mehTiming && !wasHit)
            {
                RegisterNoteResult(Judgment.Miss);
            }

        }

        private void HitNote()
        {
            if (wasHit) return;
            wasHit = true;

            float hitTime = Time.time;
            float offset = Mathf.Abs(hitTime - activationTime);
            
            if (offset <= perfectTiming)
            {
                RegisterNoteResult(Judgment.Perfect);
            }
            else if (offset <= goodTiming)
            {
                RegisterNoteResult(Judgment.Good);
            }
            else if (offset <= mehTiming)
            {
                RegisterNoteResult(Judgment.Meh);
            }
            else
            {
                RegisterNoteResult(Judgment.Miss); 
            }
        }


        private void RegisterNoteResult(Judgment judgment)
        {
            if (noteButton != null){
                noteButton.gameObject.SetActive(false); 
            }

            onNoteCompleted?.Invoke(judgment); 
            Destroy(gameObject); 
        }
    }
}

