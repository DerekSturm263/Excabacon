public class GamemodeType
{
    public string name;
    public string description;
    public string playerCount;

    public System.Predicate<PlayerController> winCondition; // CHANGE TO SOME SORT OF SORTING THINGY.

    public GamemodeType(string name, string description, string playerCount, System.Predicate<PlayerController> winCondition)
    {
        this.name = name;
        this.description = description;
        this.playerCount = playerCount;

        this.winCondition = winCondition;

        GamemodeTypes.allGamemodeTypes.Add(this);
    }
}
