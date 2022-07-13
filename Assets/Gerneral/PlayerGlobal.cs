using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerGlobal : MonoBehaviour
{
    #region variables
    [System.Serializable]
    public struct PlayerGlobalData
    {
        public Boat playerBoat;
        public int money;
        //public Dictionary<string, Mission> activeMission;
    }

    public static Boat playerBoat { get; set; }
    public static int money { get; set; }
    /*
    static Dictionary<string, Mission> activeMissions;

    public static void AddMission(Mission m){
        activeMissions.Add(m.missionName, m);
    }
    public static Mission[] GetMissions(){
        return activeMissions.Values.ToArray();
    }
    #endregion

    #region Methods

    private void Start()
    {
        activeMissions = new Dictionary<string, Mission>();
        GameEvents.SaveInitiated += SaveGlobal;// add events to happen when save and load is called
        GameEvents.LoadInitiated += LoadGlobal;
    }
    public void SaveGlobal() {
        PlayerGlobalData savePlayer = new PlayerGlobalData();
        savePlayer.playerBoat = playerBoat;
        savePlayer.money = money;
        savePlayer.activeMission = activeMissions;
        SaveLoad.Save(savePlayer, "PlayerGlobal");
    }
    public void LoadGlobal(){
        if (SaveLoad.SaveExists("PlayerGlobal"))
        {
            PlayerGlobalData playerData = SaveLoad.Load<PlayerGlobalData>("PlayerGlobal");
            playerBoat = playerData.playerBoat;
            money = playerData.money;
            activeMissions = playerData.activeMission;
        }
    }
    */
    #endregion
}
