public static class WeaponTypes
{
    public static WeaponType Drill = new WeaponType("Drill", damage: 4, knockback: 2, speed: 0, manaUse: 0, canMine: true);
    public static WeaponType Pickaxe = new WeaponType("Pickaxe", 8, 4, 2, 0, true);
    public static WeaponType Spell = new WeaponType("Mining Spell", 10, 0, 6, 4, false,
    new System.Action<PlayerController>((player) =>
    {
        player.ShootSpell();
    }));

    public static WeaponType Bow = new WeaponType("Bow", 6, 2, 0, 0, false,
        new System.Action<PlayerController>((player) =>
    {
        player.PullBow();
    }
    ),
        new System.Action<PlayerController>((player) =>
    {
        player.ReleaseBow();
    }));
}
