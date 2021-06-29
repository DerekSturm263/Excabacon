using System;

public class AbilityType
{
    public string name;

    public float damage;
    public float rechargeTime;
    public float manaUse;

    public Action<PlayerController> useAction;

    public AbilityType(string name, float damage, float rechargeTime, float manaUse, Action<PlayerController> useAction)
    {
        this.name = name;

        this.damage = damage;
        this.rechargeTime = rechargeTime;
        this.manaUse = manaUse;

        this.useAction = useAction;
    }
}
