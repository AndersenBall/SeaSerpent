using System;
using System.Collections.Generic;
using System.Linq;
using MapMode.Scripts;
using MapMode.Scripts.NavalInteractionContracts;
using UnityEngine;

[Serializable]
public class NationalityOpinionData
{
    public Dictionary<Nation, int> NationalityOpinions;
}

public class NationalityOpinionSystem : MonoBehaviour
{
    

    #region variables
    private static NationalityOpinionSystem _instance;
    public static NationalityOpinionSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("NationalityOpinionSystem").AddComponent<NationalityOpinionSystem>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private Dictionary<Nation, int> NationalityOpinions = new Dictionary<Nation, int>
    {
        { Nation.Britain, 0 },
        { Nation.Spain, 0 },
        { Nation.Netherlands, 0 },
        { Nation.France, 0 },
        { Nation.RepublicOfPirates, 0 },
        { Nation.BarbaryCorsairs, 0 },
        { Nation.BrethrenOfTheCoast, 0 }
    };
    #endregion
    
    # region monohaviors 
    
    private void Awake()
    {
        GameEvents.SaveInitiated += Save;
        GameEvents.LoadInitiated += Load;
        NavalInteractionEvent.AttackedFleet += HandleAttackEvent;
    }
    private void Start()
    {
        // Ensure this instance is the singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;

        DisplayOpinions();
        InvokeRepeating(nameof(UpdateOpinionsOverTime), 60f, 60f); // Decay opinions every 30 seconds
    }

    private void OnDestroy()
    {
        GameEvents.SaveInitiated -= Save;
        GameEvents.LoadInitiated -= Load;
        NavalInteractionEvent.AttackedFleet -= HandleAttackEvent;
    }
    
    #endregion

    #region  handle events

    public void HandleAttackEvent(AttackFleetEvent attackEvent)
    {
        if (attackEvent.PlayerStartedAttack){
            ModifyOpinion(attackEvent.EnemyFleet.Nationality,-10);
        }
    }

    #endregion
    
    #region modify opinions 
    public void ModifyOpinion(Nation nationality, int amount)
    {
        if (NationalityOpinions.ContainsKey(nationality))
        {
            NationalityOpinions[nationality] = Mathf.Clamp(NationalityOpinions[nationality] + amount, -100, 100);
            Debug.Log($"{nationality}'s opinion updated by {amount}. New opinion: {NationalityOpinions[nationality]}.");
        }
        else
        {
            Debug.LogError($"Nationality '{nationality}' does not exist in the system.");
        }
    }

    public void UpdateOpinionsOverTime()
    {
        foreach (var key in NationalityOpinions.Keys.ToList())
        {
            if (NationalityOpinions[key] > 0)
            {
                NationalityOpinions[key]--;
            }
            else if (NationalityOpinions[key] < 0)
            {
                NationalityOpinions[key]++;
            }
        }
        Debug.Log("Opinions updated over time.");
    }

    public int CheckInteractions(Nation nationality)
    {
        if (NationalityOpinions.ContainsKey(nationality))
        {
            return NationalityOpinions[nationality];
        }
        else
        {
            Debug.LogError($"Nationality '{nationality}' does not exist in the system.");
            return 0;
        }
    }

    public void DisplayOpinions()
    {
        Debug.Log("Current Nationality Opinions:");
        foreach (var pair in NationalityOpinions)
        {
            Debug.Log($"{pair.Key}: {pair.Value}");
        }
    }
    #endregion
    
    #region  save load

    public void Save()
    {
        NationalityOpinionData data = new()
        {
            NationalityOpinions = new Dictionary<Nation, int>(NationalityOpinions)
        };
        SaveLoad.Save(data, "NationalityOpinions");
    }

    public void Load()
    {
        NationalityOpinionData data = SaveLoad.Load<NationalityOpinionData>("NationalityOpinions");
        NationalityOpinions = new Dictionary<Nation, int>(data.NationalityOpinions);

    }

    #endregion
    
}
