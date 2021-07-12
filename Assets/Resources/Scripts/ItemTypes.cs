public static class ItemTypes
{
    private static readonly UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/item_icons");

    public static System.Collections.Generic.List<ItemType> allItemTypes = new System.Collections.Generic.List<ItemType>();

    public static ItemType Bomb = new ItemType("Bomb", iconSpriteSheet[0],
        new System.Action<Item>((item) =>
        {
            item.Throw();
        }),
        new System.Action<Item>((item) =>
        {
            item.Explode();
        })
    );

    public static ItemType Relic = new ItemType("Ancient Relic", iconSpriteSheet[0],
        new System.Action<Item>((item) =>
        {
            item.Throw();
        })
    );

    public static ItemType ItemFromInt(int key)
    {
        if (key == -1)
        {
            return allItemTypes[UnityEngine.Random.Range(0, allItemTypes.Count)];
        }

        return allItemTypes[key];
    }
}
