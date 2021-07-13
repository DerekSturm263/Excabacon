public class GamemodeType
{
    public string name;
    public string description;
    public int minPlayerCount;
    public int maxPlayerCount;

    public System.Comparison<PlayerController> winnerSorter;
    public System.Predicate<GameController> gameEndCondition;
    public System.Action<GameController> gameStart;

    public GamemodeType(string name, string description, int minPlayerCount, int maxPlayerCount, System.Comparison<PlayerController> winnerSorter, System.Predicate<GameController> gameEndCondition, System.Action<GameController> gameStart = null)
    {
        this.name = name;
        this.description = description;
        this.minPlayerCount = minPlayerCount;
        this.maxPlayerCount = maxPlayerCount;

        this.winnerSorter = winnerSorter;
        this.gameStart = gameStart;
        this.gameEndCondition = gameEndCondition;

        GamemodeTypes.allGamemodeTypes.Add(this);
    }
}
