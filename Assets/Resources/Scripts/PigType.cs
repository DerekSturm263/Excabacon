public class PigType
{
    public string name;
    public UnityEngine.Sprite icon;

    public float hp;
    public float mana;

    public float walkSpeed;
    public float runSpeed;
    public float undergroundSpeed;
    public float jumpHeight;
    public float defense;

    public float damage;
    public float knockback;
    public float attackSpeed;

    public PigType(string name, UnityEngine.Sprite icon, float hp, float mana, float walkSpeed, float runSpeed, float undergroundSpeed, float jumpHeight, float defense, float damage, float knockback, float attackSpeed)
    {
        this.name = name;
        this.icon = icon;

        this.hp = hp;
        this.mana = mana;

        this.walkSpeed = walkSpeed;
        this.runSpeed = runSpeed;
        this.undergroundSpeed = undergroundSpeed;
        this.jumpHeight = jumpHeight;
        this.defense = defense;

        this.damage = damage;
        this.knockback = knockback;
        this.attackSpeed = attackSpeed;

        PigTypes.allPlayerTypes.Add(this);
    }
}
