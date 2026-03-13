using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("PlayerGlobal is deprecated. Use PlayerStateService instead.")]
public class PlayerGlobal : MonoBehaviour
{
    public static Boat playerBoat
    {
        get => PlayerStateService.ActiveBoat;
        set => PlayerStateService.ActiveBoat = value;
    }

    public static float money
    {
        get => PlayerStateService.Money;
        set => PlayerStateService.Money = value;
    }

    public static bool BuyItem(float amount) {

        return PlayerStateService.TrySpendMoney(amount);
    }

    public static void AddMoney(float amount) {
        PlayerStateService.AddMoney(amount);
    }
}
