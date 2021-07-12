public static class PigTypes
{
    private static readonly UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/player_icons");

    public static System.Collections.Generic.List<PigType> allPlayerTypes = new System.Collections.Generic.List<PigType>();
    public static int Count
    {
        get => allPlayerTypes.Count;
    }

    public static PigType Random = new PigType("Random", iconSpriteSheet[4], 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    public static PigType PorkChops = new PigType("Pork Chops", iconSpriteSheet[0], hp: 100, mana: 0, walkSpeed: 6, runSpeed: 12, undergroundSpeed: 8, jumpHeight: 20, defense: 4, damage: 2, knockback: 2, attackSpeed: 10);
    public static PigType TroughDweller = new PigType("Trough Dweller", iconSpriteSheet[1], hp: 150, mana: 0, walkSpeed: 4, runSpeed: 8, undergroundSpeed: 6, jumpHeight: 12, defense: 8, damage: 6, knockback: 4, attackSpeed: 4);
    public static PigType Porkerer = new PigType("Porkerer", iconSpriteSheet[2], hp: 80, mana: 20, walkSpeed: 6, runSpeed: 12, undergroundSpeed: 6, jumpHeight: 16, defense: 2, damage: 4, knockback: 2, attackSpeed: 8);
    public static PigType RobinHog = new PigType("Robin Hog", iconSpriteSheet[3], hp: 100, mana: 0, walkSpeed: 8, runSpeed: 16, undergroundSpeed: 10, jumpHeight: 22, defense: 2, damage: 4, knockback: 2, attackSpeed: 12);

    public static PigType PigFromInt(int key)
    {
        return allPlayerTypes[key];
    }
}
