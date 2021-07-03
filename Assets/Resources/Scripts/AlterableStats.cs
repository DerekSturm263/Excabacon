public struct AlterableStats
{
    public float currentHP;
    public float currentMana;

    public int stocks;

    public AlterableStats(float currentHP, float currentMana, int stocks)
    {
        this.currentHP = currentHP;
        this.currentMana = currentMana;

        this.stocks = stocks;
    }
}
