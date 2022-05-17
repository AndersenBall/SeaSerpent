using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sailor
{
    public string name;
    int hp;
    List<string> weapons;
    int moveSpeed;
    int reloadSpeed;
    int commandStrength;
    int sailorStrength;
    public Sailor(string n)
    {
        name = n;
        hp = 10;
        weapons = new List<string>();
        reloadSpeed = 5;
        moveSpeed = 5;
        commandStrength = 3;
        sailorStrength = 5;
    }
    public Sailor(int h, List<string> weap, int move, int reload, int command, int sail)
    {
        name = "base";
        hp = h;
        weapons = weap;
        reloadSpeed = reload;
        moveSpeed = move;
        commandStrength = command;
        sailorStrength = sail;
    }
}
