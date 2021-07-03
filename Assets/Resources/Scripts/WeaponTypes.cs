public static class WeaponTypes
{
    private static UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/weapon_icons");

    public static System.Collections.Generic.List<WeaponType> allWeaponTypes = new System.Collections.Generic.List<WeaponType>();

    public static WeaponType Drill = new WeaponType(name: "Drill", icon: iconSpriteSheet[0], damage: 4, knockback: 2, speed: 0, manaUse: 0,
        null, new System.Action<PlayerController>((player) =>
        {
            player.Mine(2);
            player.weapon.CheckForImpact();
        })
    );
    public static WeaponType Pickaxe = new WeaponType("Pickaxe", iconSpriteSheet[1], 8, 4, 2, 0, 
        new System.Action<PlayerController>((player) =>
        {
            player.Mine(4);
            player.weapon.CheckForImpact();
        })
    );
    public static WeaponType Spell = new WeaponType("Spell", iconSpriteSheet[2], 10, 0, 6, 4,
        new System.Action<PlayerController>((player) =>
        {
            player.Spell();
        })
    );

    public static WeaponType Bow = new WeaponType("Bow", iconSpriteSheet[3], 6, 2, 0, 0,
        new System.Action<PlayerController>((player) =>
        {
            player.BowStart(1, 3f, 0.5f);
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

    public static WeaponType WeaponFromInt(int key)
    {
        return allWeaponTypes[key];
    }
}
