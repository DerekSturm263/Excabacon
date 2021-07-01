public static class WeaponTypes
{
    private static UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/weapon_icons");

    public static WeaponType Drill = new WeaponType(name: "Drill", icon: iconSpriteSheet[0], damage: 4, knockback: 2, speed: 0, manaUse: 0);
    public static WeaponType Pickaxe = new WeaponType("Pickaxe", iconSpriteSheet[1], 8, 4, 2, 0, 
        new System.Action<PlayerController>((player) =>
        {
            player.SwingWeapon();
        })
    );
    public static WeaponType Spell = new WeaponType("Spell", iconSpriteSheet[2], 10, 0, 6, 4);

    public static WeaponType Bow = new WeaponType("Bow", iconSpriteSheet[3], 6, 2, 0, 0,
        new System.Action<PlayerController>((player) =>
        {
            player.BowStart(1, 1.5f, 1f);
        }),
            new System.Action<PlayerController>((player) =>
        {
            player.BowUpdate();
        }),
        new System.Action<PlayerController>((player) =>
        {
            player.BowEnd();
        })
    );
}
