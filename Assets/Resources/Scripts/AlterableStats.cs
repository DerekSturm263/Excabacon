public struct AlterableStats
{
    public float currentHP;
    public float currentMana;

    public int stocks;
    public int kills;
    public int points;
    public float relicTime;

    public AlterableStats(float currentHP, float currentMana, int stocks, int kills, int points, float relicTime)
    {
        this.currentHP = currentHP;
        this.currentMana = currentMana;

        this.stocks = stocks;
        this.kills = kills;
        this.points = points;
        this.relicTime = relicTime;
    }
}
