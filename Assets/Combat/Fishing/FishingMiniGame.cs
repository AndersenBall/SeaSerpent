using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Combat.Fishing
{
    /*
     * FishingMiniGame
     * - 1D lane from 0..1
     * - Player controls HOOK (left/right); FISH moves with noisy bursts
     * - Overlap window awards catch progress; miss builds escape/tension
     *
     * Hook physics (feels better than raw position):
     *   acceleration from input, max speed, friction
     *
     * Provide simple UI via RectTransforms:
     *   laneRect  : the bar background
     *   fishRect  : small marker for fish
     *   hookRect  : small marker for your hook
     *   catchFill : Image fill (0..1) showing catch progress
     *   tensionFill: Image fill (0..1) showing tension/escape risk
     */
    
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [DisallowMultipleComponent]
    public class FishingMiniGame : MonoBehaviour
    {
        [Header("UI (Worldspace or Screen-space)")]
        [SerializeField] private RectTransform laneRect;
        [SerializeField] private RectTransform fishRect;
        [SerializeField] private RectTransform hookRect;
        [SerializeField] private Image catchFill;
        [SerializeField] private Image tensionFill;

        [Header("Session")]
        [SerializeField] private float sessionTime = 20f;
        [SerializeField] private bool useSessionTimer = true;

        [Header("Win/Lose")]
        [SerializeField] private float catchToWin = 1.0f;
        [SerializeField] private float missToLose = 1.0f;

        [Header("Overlap Rules (Base)")]
        [SerializeField] private float overlapRadius = 0.06f;
        [SerializeField] private float catchRate = 0.30f;
        [SerializeField] private float tensionRelaxRate = 0.50f;
        [SerializeField] private float tensionBuildRate = 0.25f;

        [Header("Hook Control")]
        [SerializeField] private KeyCode leftKey = KeyCode.A;
        [SerializeField] private KeyCode rightKey = KeyCode.D;
        [SerializeField] private float hookAccel = 3.0f;
        [SerializeField] private float hookFriction = 6.0f;
        [SerializeField] private float hookMaxSpeed = 1.5f;

        [Header("Fish Motion (Base)")]
        [SerializeField] private float fishBaseSpeed = 0.35f;
        [SerializeField] private float fishWobble = 1.8f;
        [SerializeField] private float surgeChance = 0.35f;
        [SerializeField] private float surgeSpeed = 1.25f;
        [SerializeField] private float surgeTimeMin = 0.25f;
        [SerializeField] private float surgeTimeMax = 0.8f;

        [Header("Start Range")]
        [SerializeField] private Vector2 fishStartRange = new Vector2(0.25f, 0.75f);
        [SerializeField] private Vector2 hookStartRange = new Vector2(0.25f, 0.75f);

        [Header("Events")]
        public UnityEvent OnCatch;
        public UnityEvent OnEscape;
        public UnityEvent OnBegin;
        public UnityEvent OnEnd;

        // Runtime state
        private bool _active;
        private float _time;
        private float _catchMeter;
        private float _tension;
        private float _fish;
        private float _fishDir;
        private float _fishSpeed;
        private float _surgeTimer;
        private float _hook;
        private float _hookVel;

        // Active fish reference
        private FishType _currentFish;

        // Effective values (after modifiers applied)
        private float effOverlapRadius;
        private float effFishSpeed;
        private float effWobble;
        private float effSurgeChance;

        // Public API --------------------------------------------------

        public bool IsActive => _active;
        public float CatchProgress01 => Mathf.Clamp01(_catchMeter / catchToWin);
        public float Tension01 => Mathf.Clamp01(_tension / missToLose);
        public float FishPos01 => _fish;
        public float HookPos01 => _hook;
        public FishType CurrentFish => _currentFish;

        /// <summary>
        /// Begin a new mini-game session with a given FishType.
        /// </summary>
        public void Begin(FishType fish)
        {
            _currentFish = fish;
            ApplyFishModifiers(fish);

            _active = true;
            _time = 0;
            _catchMeter = 0;
            _tension = 0;

            _fish = Random.Range(fishStartRange.x, fishStartRange.y);
            _hook = Random.Range(hookStartRange.x, hookStartRange.y);
            _hookVel = 0;

            _fishDir = Random.value < 0.5f ? -1f : 1f;
            _fishSpeed = effFishSpeed;

            _surgeTimer = 0;

            SyncUI();
            OnBegin?.Invoke();
        }

        public void End()
        {
            _active = false;
            OnEnd?.Invoke();
        }

        // Internal helpers --------------------------------------------

        private void ApplyFishModifiers(FishType fish)
        {
            if (fish == null)
            {
                effOverlapRadius = overlapRadius;
                effFishSpeed = fishBaseSpeed;
                effWobble = fishWobble;
                effSurgeChance = surgeChance;
                return;
            }

            effOverlapRadius = overlapRadius * fish.overlapRadiusMultiplier;
            effFishSpeed = fishBaseSpeed * fish.speedMultiplier;
            effWobble = fishWobble * fish.wobbleMultiplier;
            effSurgeChance = surgeChance * fish.surgeChanceMultiplier;
        }

        private void Update()
        {
            if (!_active) return;

            float dt = Time.deltaTime;
            _time += dt;

            if (useSessionTimer && _time >= sessionTime)
            {
                if (CatchProgress01 >= 0.5f) ResolveWin();
                else ResolveLose();
                return;
            }

            UpdateHook(dt);
            UpdateFish(dt);
            UpdateMeters(dt);
            SyncUI();
        }

        private void UpdateHook(float dt)
        {
            float input = 0f;
            if (Input.GetKey(leftKey)) input -= 1f;
            if (Input.GetKey(rightKey)) input += 1f;

            _hookVel += input * hookAccel * dt;
            _hookVel = Mathf.Clamp(_hookVel, -hookMaxSpeed, hookMaxSpeed);

            float fr = Mathf.Exp(-hookFriction * dt);
            _hookVel *= fr;

            _hook += _hookVel * dt;
            _hook = Mathf.Clamp01(_hook);
            if ((_hook <= 0f && _hookVel < 0f) || (_hook >= 1f && _hookVel > 0f))
                _hookVel = 0f;
        }

        private void UpdateFish(float dt)
        {
            _fishDir += Random.Range(-effWobble, effWobble) * dt;
            _fishDir = Mathf.Clamp(_fishDir, -1f, 1f);

            if (_surgeTimer > 0f)
            {
                _surgeTimer -= dt;
                _fishSpeed = Mathf.Lerp(_fishSpeed, surgeSpeed, 0.6f);
            }
            else
            {
                _fishSpeed = Mathf.Lerp(_fishSpeed, effFishSpeed, 0.5f);
                if (Random.value < effSurgeChance * dt)
                    _surgeTimer = Random.Range(surgeTimeMin, surgeTimeMax);
            }

            _fish += _fishDir * _fishSpeed * dt;

            if (_fish < 0f) { _fish = 0f; _fishDir = Mathf.Abs(_fishDir); }
            if (_fish > 1f) { _fish = 1f; _fishDir = -Mathf.Abs(_fishDir); }
        }

        private void UpdateMeters(float dt)
        {
            bool overlapping = Mathf.Abs(_hook - _fish) <= effOverlapRadius;

            if (overlapping)
            {
                _catchMeter += catchRate * dt;
                _tension -= tensionRelaxRate * dt;
            }
            else
            {
                _tension += tensionBuildRate * dt;
            }

            _catchMeter = Mathf.Clamp(_catchMeter, 0f, catchToWin);
            _tension = Mathf.Clamp(_tension, 0f, missToLose);

            if (_catchMeter >= catchToWin) ResolveWin();
            else if (_tension >= missToLose) ResolveLose();
        }

        private void ResolveWin()
        {
            SyncUI();
            OnCatch?.Invoke();
            End();
        }

        private void ResolveLose()
        {
            SyncUI();
            OnEscape?.Invoke();
            End();
        }

        private void SyncUI()
        {
            if (laneRect == null) return;

            if (fishRect) SetMarkerAlongLane(fishRect, _fish);
            if (hookRect) SetMarkerAlongLane(hookRect, _hook);

            if (catchFill) catchFill.fillAmount = CatchProgress01;
            if (tensionFill) tensionFill.fillAmount = Tension01;
        }

        private void SetMarkerAlongLane(RectTransform marker, float t01)
        {
            var laneWidth = laneRect.rect.width;
            var local = marker.anchoredPosition;
            local.x = Mathf.Lerp(-laneWidth * 0.5f, laneWidth * 0.5f, t01);
            marker.anchoredPosition = local;
        }
    }
}


