public static class WeaponTypes
{
    public static WeaponType Drill = new WeaponType("Drill", damage: 4, knockback: 2, speed: 6, manaUse: 0, canMine: true);
    public static WeaponType Pickaxe = new WeaponType("Pickaxe", 8, 4, 2, 0, true);
    public static WeaponType Spell = new WeaponType("Mining Spell", 10, 0, 6, 4, true);
    public static WeaponType Shovel = new WeaponType("Shovel", 6, 3, 4, 0, true);
}
