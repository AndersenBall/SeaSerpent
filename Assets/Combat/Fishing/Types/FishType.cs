using UnityEngine;

namespace Combat.Fishing
{/// <summary>
 /// Defines one type of fish with its stats and difficulty modifiers.
 /// Create instances via: Right-click in Project → Create → Fishing → Fish Type
 /// </summary>
 [CreateAssetMenu(fileName = "FishType", menuName = "Fishing/Fish Type", order = 0)]
 public class FishType : ScriptableObject
 {
     [Header("Identity")]
     public string displayName = "Unnamed Fish";
     [TextArea] public string description;
 
     [Header("Stats")]
     [Tooltip("Average length in cm (or in-game unit).")]
     public float length = 30f;
 
     [Tooltip("Average weight in kg.")]
     public float weight = 2f;
 
     [Tooltip("Sell price or cost value.")]
     public int cost = 10;
 
     [Range(0f, 1f), Tooltip("0 = common, 1 = legendary.")]
     public float rarity = 0.1f;
 
     [Header("Mini-game Modifiers")]
     [Tooltip("Multiplier on fish base speed.")]
     public float speedMultiplier = 1.0f;
 
     [Tooltip("Multiplier on surge chance.")]
     public float surgeChanceMultiplier = 1.0f;
 
     [Tooltip("Multiplier on wobble (erraticness).")]
     public float wobbleMultiplier = 1.0f;
 
     [Tooltip("Size of overlap window (higher = easier).")]
     public float overlapRadiusMultiplier = 1.0f;
 }

    
}