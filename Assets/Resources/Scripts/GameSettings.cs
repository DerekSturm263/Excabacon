public struct GameSettings
{
    public GamemodeType gameMode;

    public PigSettings[] players;
    public ItemSettings itemSettings;
    public StageType stage;

    public int stocks;
    public float matchTime;

    public int playerCount;

    public GameSettings(GamemodeType gameMode, PigSettings[] players, ItemSettings itemSettings, StageType stage, int stocks, float matchTime, int playerCount)
    {
        this.gameMode = gameMode;

        this.players = players;
        this.itemSettings = itemSettings;
        this.stage = stage;

        this.stocks = stocks;
        this.matchTime = matchTime;

        this.playerCount = playerCount;
    }
}
