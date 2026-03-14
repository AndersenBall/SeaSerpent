using UnityEngine;

namespace MapMode.Scripts.PostBattle
{
    public static class PostCombatUIBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsurePostCombatUIExists()
        {
            var existingController = Object.FindObjectOfType<PostCombatUIController>();
            if (existingController != null)
            {
                return;
            }

            var uiObject = new GameObject("PostCombatUI");
            Object.DontDestroyOnLoad(uiObject);
            uiObject.AddComponent<PostCombatUIController>();
        }
    }
}
