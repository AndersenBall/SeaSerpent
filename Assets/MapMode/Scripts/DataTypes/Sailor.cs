using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sailor
{
    public string name { get; private set; }
    public int hp { get; private set; }

    List<string> weapons;
    public int moveSpeed { get; private set; }
    public int cannonReloadSpeed { get; private set; }
    public int sailorStrength { get; private set; }
    public Sailor(string n)
    {
        name = n;
        hp = 10;
        weapons = new List<string>();
        cannonReloadSpeed = 5;
        moveSpeed = 5;

        sailorStrength = 5;
    }
    public Sailor(int h, List<string> weap, int move, int reload, int sail)
    {
        name = "base";
        hp = h;
        weapons = weap;
        cannonReloadSpeed = reload;
        moveSpeed = move;
        sailorStrength = sail;
    }
}
