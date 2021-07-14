using System;

public class ItemType
{
    public string name;
    public UnityEngine.Sprite icon;

    public Action<Item> pickupAction; // Action for when the player picks up the item.
    public Action<Item> useAction; // Action for when the player uses the item.
    public Action<Item> hitAction; // Action for when the item hits the ground or another player.

    public ItemType(string name, UnityEngine.Sprite icon, Action<Item> pickupAction = null, Action<Item> useAction = null, Action<Item> hitAction = null)
    {
        this.name = name;
        this.icon = icon;

        this.pickupAction = pickupAction;
        this.useAction = useAction;
        this.hitAction = hitAction;

        ItemTypes.allItemTypes.Add(this);
    }
}
