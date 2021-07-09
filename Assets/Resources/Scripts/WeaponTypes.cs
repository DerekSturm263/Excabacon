public static class WeaponTypes
{
    private static UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/weapon_icons");

    public static System.Collections.Generic.List<WeaponType> allWeaponTypes = new System.Collections.Generic.List<WeaponType>();
    public static int Count
    {
        get => allWeaponTypes.Count;
    }

    public static WeaponType Random = new WeaponType("Random", null, 0, 0, 0, 0, 0);
    public static WeaponType Drill = new WeaponType("Drill", iconSpriteSheet[0], damage: 2, knockback: 2, speed: 0, manaUse: 0, mineRadius: 2,
        null, new System.Action<PlayerController>((player) =>
        {
            player.Mine();
            player.weapon.CheckForImpact();
        })
    );
    public static WeaponType Pickaxe = new WeaponType("Pickaxe", iconSpriteSheet[1], damage: 8, knockback: 6, speed: 2, manaUse: 0, mineRadius: 4,
        new System.Action<PlayerController>((player) =>
        {
            player.Mine();
            player.weapon.CheckForImpact();
        })
    );
    public static WeaponType Spell = new WeaponType("Spell", iconSpriteSheet[2], damage: 6, knockback: 2, speed: 0, manaUse: 2, mineRadius: 2,
        new System.Action<PlayerController>((player) =>
        {
            player.Spell();
        })
    );

    public static WeaponType Bow = new WeaponType("Bow", iconSpriteSheet[3], damage: 4, knockback: 2, speed: 0, manaUse: 0, mineRadius: 2,
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
