public struct PlayerSettings
{
    public PlayerType player;
    public WeaponType weapon;
    public AbilityType ability;

    public int teamNum;

    public PlayerSettings(PlayerType player, WeaponType weapon, AbilityType ability, int teamNum)
    {
        this.player = player;
        this.weapon = weapon;
        this.ability = ability;

        this.teamNum = teamNum;
    }
}
