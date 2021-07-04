public class StageType
{
    public string name;
    public UnityEngine.Sprite icon;
    public UnityEngine.Tilemaps.TileBase tiles;

    public StageType(string name, UnityEngine.Sprite icon, UnityEngine.Tilemaps.TileBase tiles)
    {
        this.name = name;
        this.icon = icon;
        this.tiles = tiles;

        StageTypes.allStageTypes.Add(this);
    }
}
