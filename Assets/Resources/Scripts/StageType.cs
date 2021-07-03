public class StageType
{
    public string name;
    public UnityEngine.Sprite icon;

    public StageType(string name, UnityEngine.Sprite icon)
    {
        this.name = name;
        this.icon = icon;

        StageTypes.allStageTypes.Add(this);
    }
}
