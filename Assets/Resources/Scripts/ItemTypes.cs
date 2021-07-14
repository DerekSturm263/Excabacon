public static class ItemTypes
{
    private static readonly UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/item_icons");

    public static System.Collections.Generic.List<ItemType> allItemTypes = new System.Collections.Generic.List<ItemType>();

    public static ItemType Bomb = new ItemType("Bomb", iconSpriteSheet[0],
        null,
        useAction: new System.Action<Item>((item) =>
        {
            item.Throw();
        }),
        hitAction: new System.Action<Item>((item) =>
        {
            item.Explode();
        })
    );

    public static ItemType HealthPotion = new ItemType("Health Potion", iconSpriteSheet[1],
        pickupAction: new System.Action<Item>((item) =>
        {
            item.carrier.Heal(25f);
            item.Destroy();
        }));

    public static ItemType ManaPotion = new ItemType("Mana Potion", iconSpriteSheet[2],
        pickupAction: new System.Action<Item>((item) =>
        {
            item.carrier.RestoreMana(25f);
            item.Destroy();
        }));

    public static ItemType Relic = new ItemType("Ancient Relic", iconSpriteSheet[3],
        pickupAction: new System.Action<Item>((item) =>
        {
            item.carrier.statMultiplier = 1.5f;
        }),
        useAction: new System.Action<Item>((item) =>
        {
            item.carrier.statMultiplier = 1f;
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
