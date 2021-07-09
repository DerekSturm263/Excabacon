public static class AbilityTypes
{
    private static UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/ability_icons");

    public static System.Collections.Generic.List<AbilityType> allAbilityTypes = new System.Collections.Generic.List<AbilityType>();
    public static int Count
    {
        get => allAbilityTypes.Count;
    }

    public static AbilityType Random = new AbilityType("Random", null, 0, 0, 0);
    public static AbilityType DirtDash = new AbilityType(name: "Dirt Dash", icon: iconSpriteSheet[0], damage: 8, rechargeTime: 0, manaUse: 0, new System.Action<PlayerController>((player) =>
    {
        player.Dash(27.5f);
    }));

    public static AbilityType MudMash = new AbilityType("Mud Mash", iconSpriteSheet[1], 14, 6, 0, new System.Action<PlayerController>((player) =>
    {
        
    }));

    public static AbilityType PorkPush = new AbilityType("Pork Push", iconSpriteSheet[2], 4, 6, 0, new System.Action<PlayerController>((player) =>
    {
        
    }));

    public static AbilityType SuperShot = new AbilityType("Super Shot", iconSpriteSheet[3], 4, 6, 0,
        new System.Action<PlayerController>((player) =>
        {
            player.BowStart(3, 2f, 1f);
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

    public static AbilityType AbilityFromInt(int key)
    {
        return allAbilityTypes[key];
    }
}
