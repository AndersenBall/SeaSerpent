using UnityEngine;

namespace GerneralScripts.BattleManagement
{
    public sealed class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }
        public BattleSession CurrentSession { get; private set; }
        public BattleResult PendingResult { get; private set; }

        [SerializeField] private string combatSceneName = "CombatScene";

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartBattle(BattleSession session)
        {
            CurrentSession = session;
            PendingResult = null;

            GameEvents.SaveGame();

            if (session.Mode == ResolutionMode.Auto)
            {
                PendingResult = AutoResolver.Resolve(session);
                ReturnAndApply();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(combatSceneName);
            }
        }

        public void SubmitCombatResult(BattleResult result)
        {
            PendingResult = result;
            ReturnAndApply();
        }

        private void ReturnAndApply()
        {
            var returnScene = CurrentSession.ReturnSceneName;

            // If we're already in the return scene, don't reload it—just apply immediately.
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene.name == returnScene)
            {
                FinalizeBattle();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnReturnLoaded;
            UnityEngine.SceneManagement.SceneManager.LoadScene(returnScene);
        }

        private void OnReturnLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            if (scene.name != CurrentSession.ReturnSceneName) return;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnReturnLoaded;

            FinalizeBattle();
        }

        private void FinalizeBattle()
        {
            BattleResultApplier.Apply(CurrentSession, PendingResult);

            CurrentSession = null;
            PendingResult = null;
            GameEvents.ClearEvents();
        }
    }

}