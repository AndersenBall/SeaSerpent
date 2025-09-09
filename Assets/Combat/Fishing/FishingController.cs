using UnityEngine;
namespace Combat.Fishing
{


    [DisallowMultipleComponent]
    public class FishingController : MonoBehaviour
    {
        [Header("Refs")]
        public Camera cam;
        public CastGesture gesture;
        public Transform rodTip;
        public GameObject bobberPrefab;

        [Tooltip("Reference to the Stardew-style mini-game UI+logic.")]
        public FishingMiniGame miniGame;   // <-- NEW

        [Header("Cast")]
        public float castUpward = 2.2f;         // add arc
        public float castAimMaxDistance = 35f;
        public LayerMask aimMask = ~0;
        [SerializeField] private float settleDelay = 1.1f; // time after splash before mini-game starts

        private GameObject _bobberObj;
        private Rigidbody  _bobberRb;
        private bool _readyToCast = true;
        private bool _miniGameActive;
        
        [Header("Fish Catalog")]
        [SerializeField] private FishType[] fishCatalog;  
        [SerializeField, Tooltip("Higher = more bias toward common fish. 1 = linear.")]
        private float rarityBias = 1.5f;     

        void Update()
        {
            if (_readyToCast)
            {
                if (Input.GetMouseButtonDown(0))
                    gesture.Begin();

                if (Input.GetMouseButtonUp(0))
                {
                    if (gesture.TryComputeCast(out float power))
                    {
                        Cast(power);
                    }
                    gesture.End();
                }
            }
        }

        void Cast(float power)
        {
            _readyToCast = false;

            if (_bobberObj != null) Destroy(_bobberObj);
            _bobberObj = Instantiate(bobberPrefab, rodTip.position, Quaternion.identity);
            _bobberRb = _bobberObj.GetComponent<Rigidbody>();
            if (_bobberRb == null) _bobberRb = _bobberObj.AddComponent<Rigidbody>();
            _bobberRb.mass = 0.3f;
            _bobberRb.drag = 0.1f;

            // aim from camera center ray to ground/water hit; fallback = camera forward
            Vector3 dir = cam.transform.forward;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out var hit, castAimMaxDistance, aimMask))
                dir = (hit.point - rodTip.position).normalized;

            Vector3 vel = dir * power + Vector3.up * castUpward;
            _bobberRb.velocity = vel;

            // after a short “splash settle”, start mini-game
            Invoke(nameof(BeginMiniGame), settleDelay);
        }

        // --- Mini-game wiring ------------------------------------------------

        void BeginMiniGame()
        {
            if (miniGame == null)
            {
                Debug.LogWarning("FishingController: No FishingMiniGame assigned.");
                Cleanup(false);
                return;
            }

            // Make sure any previous run is clean
            miniGame.OnCatch.RemoveListener(OnMiniCatch);
            miniGame.OnEscape.RemoveListener(OnMiniEscape);
            miniGame.OnEnd.RemoveListener(OnMiniEnd);

            miniGame.OnCatch.AddListener(OnMiniCatch);
            miniGame.OnEscape.AddListener(OnMiniEscape);
            miniGame.OnEnd.AddListener(OnMiniEnd);

            if (!miniGame.gameObject.activeSelf)
                miniGame.gameObject.SetActive(true);

            _miniGameActive = true;

            // NEW: pick a fish using rarity weighting and begin the session
            var picked = PickRandomFishByRarity();
            if (picked == null)
            {
                Debug.LogWarning("FishingController: No FishType available in fishCatalog; starting with default difficulty.");
                miniGame.Begin(null); 
            }
            else
            {
                miniGame.Begin(picked);
                Debug.Log($"Hooked: {picked.displayName} (rarity {picked.rarity:0.00}, len {picked.length}cm, wt {picked.weight}kg, value {picked.cost})");
            }
        }


        void OnMiniCatch()
        {
            if (!_miniGameActive) return;
            // miniGame will call End() internally after invoking OnCatch
            // We just clean up success path.
            Cleanup(true);
        }

        void OnMiniEscape()
        {
            if (!_miniGameActive) return;
            // miniGame will call End() internally after invoking OnEscape
            Cleanup(false);
        }

        void OnMiniEnd()
        {
            // Hide/disable UI if you want the widget to go away automatically
            if (miniGame != null && miniGame.gameObject.activeSelf)
                miniGame.gameObject.SetActive(false);

            // Ensure listeners don’t stack between sessions
            if (miniGame != null)
            {
                miniGame.OnCatch.RemoveListener(OnMiniCatch);
                miniGame.OnEscape.RemoveListener(OnMiniEscape);
                miniGame.OnEnd.RemoveListener(OnMiniEnd);
            }

            _miniGameActive = false;
        }
        
        private FishType PickRandomFishByRarity()
        {
            if (fishCatalog == null || fishCatalog.Length == 0) return null;

            // weight = (1 - rarity)^bias  (rarity in [0..1])
            float total = 0f;
            var weights = new float[fishCatalog.Length];

            for (int i = 0; i < fishCatalog.Length; i++)
            {
                var f = fishCatalog[i];
                float r = Mathf.Clamp01(f != null ? f.rarity : 0.5f);
                float w = Mathf.Pow(1f - r, Mathf.Max(0.01f, rarityBias)); // more common → bigger weight
                if (w < 0f || float.IsNaN(w)) w = 0f;
                weights[i] = w;
                total += w;
            }

            if (total <= 0f)
            {
                // fallback: uniform if everything is zero/invalid
                return fishCatalog[Random.Range(0, fishCatalog.Length)];
            }

            float pick = Random.value * total;
            float acc = 0f;
            for (int i = 0; i < fishCatalog.Length; i++)
            {
                acc += weights[i];
                if (pick <= acc)
                    return fishCatalog[i];
            }
            return fishCatalog[fishCatalog.Length - 1]; // safety
        }


        // --- Cleanup ---------------------------------------------------------

        void Cleanup(bool success)
        {
            if (_bobberObj) Destroy(_bobberObj);

            // Note: OnMiniEnd will also run (miniGame.End() happens inside the mini-game on resolve)
            // so guard resets live there. Here we just re-arm casting and let loot logic hook in.
            _readyToCast = true;

            // TODO: award loot if success, decrement bait if not, etc.
            if (success)
                Debug.Log("Caught! (mini-game)");
            else
                Debug.Log("Fish escaped. (mini-game)");
        }
    }
}