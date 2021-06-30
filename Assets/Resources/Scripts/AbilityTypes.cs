public static class AbilityTypes
{
    private static UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/ability_icons");

    public static AbilityType DirtDash = new AbilityType(name: "Dirt Dash", icon: iconSpriteSheet[0], damage: 8, rechargeTime: 0, manaUse: 0, new System.Action<PlayerController>((player) =>
    {
        player.Dash();
    }));

    public static AbilityType MudMash = new AbilityType("Mud Mash", iconSpriteSheet[1], 14, 6, 0, new System.Action<PlayerController>((player) =>
    {
        
    }));

    public static AbilityType PorkPush = new AbilityType("Pork Push", iconSpriteSheet[2], 4, 6, 0, new System.Action<PlayerController>((player) =>
    {
        
    }));

    public static AbilityType SuperShot = new AbilityType("Super Shot", iconSpriteSheet[3], 4, 6, 0, new System.Action<PlayerController>((player) =>
    {
        
    }));
}
