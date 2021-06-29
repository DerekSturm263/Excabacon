using System;

public class ItemType
{
    public string name;

    public Action useAction; // Action for when the player uses the item.
    public Action hitAction; // Action for when the item hits the ground or another player.

    public ItemType(string name, Action useAction, Action hitAction)
    {
        this.name = name;

        this.useAction = useAction;
        this.hitAction = hitAction;
    }
}
