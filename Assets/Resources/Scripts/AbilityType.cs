using System;

public class AbilityType
{
    public string name;
    public UnityEngine.Sprite icon;

    public float rechargeTime;
    public float manaUse;

    public Action<PlayerController> actionStart; // Action that runs when the player begins using the ability.
    public Action<PlayerController> actionUpdate; // Action that runs until the player finishes using the ability.
    public Action<PlayerController> actionEnd; // Action that runs once the player finishes using the ability.

    public AbilityType(string name, UnityEngine.Sprite icon, float damage, float rechargeTime, float manaUse, Action<PlayerController> actionStart = null, Action<PlayerController> actionUpdate = null, Action<PlayerController> actionEnd = null)
    {
        this.name = name;
        this.icon = icon;

        this.rechargeTime = rechargeTime;
        this.manaUse = manaUse;

        this.actionStart = actionStart;
        this.actionUpdate = actionUpdate;
        this.actionEnd = actionEnd;

        AbilityTypes.allAbilityTypes.Add(this);
    }
}
