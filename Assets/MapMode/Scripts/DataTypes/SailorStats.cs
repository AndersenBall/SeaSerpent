
using System.Collections.Generic;

[System.Serializable]
public class SailorStats
{
    public int hp;
    public List<string> weapons;
    public int moveSpeed;
    public int cannonReloadSpeed;
    public int sailingStrength;
    public int meleeWeaponAttackSpeed;
    public int meleeWeaponDamage;
    public int navigation;
    public int repairStrength;
    public int baseCost;

    public SailorStats(
        int hp,
        List<string> weapons,
        int moveSpeed,
        int cannonReloadSpeed,
        int sailingStrength,
        int meleeWeaponAttackSpeed,
        int meleeWeaponDamage,
        int navigation,
        int repairStrength,
        int baseCost)
    {
        this.hp = hp;
        this.weapons = weapons;
        this.moveSpeed = moveSpeed;
        this.cannonReloadSpeed = cannonReloadSpeed;
        this.sailingStrength = sailingStrength;
        this.meleeWeaponAttackSpeed = meleeWeaponAttackSpeed;
        this.meleeWeaponDamage = meleeWeaponDamage;
        this.navigation = navigation;
        this.repairStrength = repairStrength;
        this.baseCost = baseCost;
    }

    public override string ToString()
    {
        return $"HP: {hp}, Weapons: {string.Join(", ", weapons)}, Move Speed: {moveSpeed}, " +
               $"Cannon Reload Speed: {cannonReloadSpeed}, Sailing Strength: {sailingStrength}, " +
               $"Melee Attack Speed: {meleeWeaponAttackSpeed}, Melee Damage: {meleeWeaponDamage}, " +
               $"Navigation: {navigation}, Repair Strength: {repairStrength}, Base Cost: {baseCost}";
    }
}