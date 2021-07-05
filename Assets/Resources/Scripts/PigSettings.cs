public struct PigSettings
{
    public PigType player;
    public WeaponType weapon;
    public AbilityType ability;

    public int teamNum;

    public PigSettings(PigType player, WeaponType weapon, AbilityType ability, int teamNum)
    {
        this.player = player;
        this.weapon = weapon;
        this.ability = ability;

        this.teamNum = teamNum;
    }
}
