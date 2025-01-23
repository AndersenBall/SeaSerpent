using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGlobal : MonoBehaviour
{
    public static Boat playerBoat { get; set; }
    public static float money { get; set; }

    public static bool BuyItem(float amount) {

        if (money - amount >= 0) { 
            money -= amount;
            return true;
        }
        return false;
    }
    
}
