using System;

public class WeaponType
{
    public string name;
    public UnityEngine.Sprite icon;

    public float damage;
    public float knockback;
    public float speed;
    public float manaUse;

    public Action<PlayerController> actionStart ; // Action that runs when the player begins using the ability.
    public Action<PlayerController> actionUpdate; // Action that runs until the player finishes using the ability.
    public Action<PlayerController> actionEnd; // Action that runs once the player finishes using the ability.

    public WeaponType(string name, UnityEngine.Sprite icon, float damage, float knockback, float speed, float manaUse, Action<PlayerController> actionStart = null, Action<PlayerController> actionUpdate = null, Action<PlayerController> actionEnd = null)
    {
        this.name = name;
        this.icon = icon;

        this.damage = damage;
        this.knockback = knockback;
        this.speed = speed;
        this.manaUse = manaUse;

        this.actionStart = actionStart;
        this.actionUpdate = actionUpdate;
        this.actionEnd = actionEnd;

        WeaponTypes.allWeaponTypes.Add(this);
    }
}
