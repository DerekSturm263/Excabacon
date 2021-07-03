public static class PlayerTypes
{
    private static UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/player_icons");

    public static System.Collections.Generic.List<PlayerType> allPlayerTypes = new System.Collections.Generic.List<PlayerType>();
    public static int Count
    {
        get => allPlayerTypes.Count;
    }

    public static PlayerType PorkChops = new PlayerType(name: "Pork Chops", icon: iconSpriteSheet[0], hp: 90, mana: 0, walkSpeed: 6, runSpeed: 10, undergroundSpeed: 8, jumpHeight: 15, defense: 3, damage: 1, knockback: 2, attackSpeed: 10);
    public static PlayerType TroughDweller = new PlayerType("Trough Dweller", iconSpriteSheet[1], 120, 0, 2, 6, 4, 2, 7, 1.5f, 4, 4);
    public static PlayerType Porkerer = new PlayerType("Porkerer", iconSpriteSheet[2], 75, 20, 4, 8, 6, 6, 0.75f, 0.75f, 0, 5);
    public static PlayerType RobinHog = new PlayerType("Robin Hog", iconSpriteSheet[3], 80, 0, 8, 12, 10, 4, 1, 1, 1, 0);

    public static PlayerType PlayerFromInt(int key)
    {
        return allPlayerTypes[key];
    }
}
