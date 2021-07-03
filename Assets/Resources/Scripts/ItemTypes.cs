public static class ItemTypes
{
    private static UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/item_icons");

    public static System.Collections.Generic.List<ItemType> allItemTypes = new System.Collections.Generic.List<ItemType>();

    public static ItemType Bomb = new ItemType("Bomb", iconSpriteSheet[0]);

    public static ItemType ItemFromInt(int key)
    {
        return allItemTypes[key];
    }
}
