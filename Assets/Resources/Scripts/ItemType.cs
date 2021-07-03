using System;

public class ItemType
{
    public string name;
    public UnityEngine.Sprite icon;

    public Action<PlayerController> useAction; // Action for when the player uses the item.
    public Action<PlayerController> hitAction; // Action for when the item hits the ground or another player.

    public ItemType(string name, UnityEngine.Sprite icon, Action<PlayerController> useAction = null, Action<PlayerController> hitAction = null)
    {
        this.name = name;
        this.icon = icon;

        this.useAction = useAction;
        this.hitAction = hitAction;

        ItemTypes.allItemTypes.Add(this);
    }
}
