using System;

public class WeaponType
{
    public string name;

    public float damage;
    public float knockback;
    public float speed;
    public float manaUse;

    public bool canMine;

    public Action<PlayerController> startUseAction;
    public Action<PlayerController> endUseAction;

    public WeaponType(string name, float damage, float knockback, float speed, float manaUse, bool canMine, Action<PlayerController> startUseAction = null, Action<PlayerController> endUseAction = null)
    {
        this.name = name;

        this.damage = damage;
        this.knockback = knockback;
        this.speed = speed;
        this.manaUse = manaUse;

        this.canMine = canMine;

        this.startUseAction = startUseAction;
        this.endUseAction = endUseAction;
    }
}
