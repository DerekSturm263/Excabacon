public class GamemodeType
{
    public string name;
    public string description;
    public string playerCount;

    public System.Comparison<PlayerController> winnerSorter;
    public System.Predicate<GameController> gameEndCondition;

    public GamemodeType(string name, string description, string playerCount, System.Comparison<PlayerController> winnerSorter, System.Predicate<GameController> gameEndCondition)
    {
        this.name = name;
        this.description = description;
        this.playerCount = playerCount;

        this.winnerSorter = winnerSorter;
        this.gameEndCondition = gameEndCondition;

        GamemodeTypes.allGamemodeTypes.Add(this);
    }
}
