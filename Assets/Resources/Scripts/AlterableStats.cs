public struct AlterableStats
{
    public float currentHP;
    public float currentMana;

    public int stocks;
    public int kills;

    public AlterableStats(float currentHP, float currentMana, int stocks, int kills)
    {
        this.currentHP = currentHP;
        this.currentMana = currentMana;

        this.stocks = stocks;
        this.kills = kills;
    }
}
