public static class StageTypes
{
    private static UnityEngine.Sprite[] iconSpriteSheet = UnityEngine.Resources.LoadAll<UnityEngine.Sprite>("Spritesheets/stage_icons");
    private static UnityEngine.RuleTile[] tiles = UnityEngine.Resources.LoadAll<UnityEngine.RuleTile>("Tilemaps");

    public static System.Collections.Generic.List<StageType> allStageTypes = new System.Collections.Generic.List<StageType>();

    public static StageType PlainsOfPlay = new StageType("The Plains of Play", iconSpriteSheet[0], tiles[0]);
    public static StageType MagicMines = new StageType("The Magic Mines", iconSpriteSheet[1], tiles[0]);
    public static StageType DesertOfDespair = new StageType("The Desert of Despair", iconSpriteSheet[2], tiles[0]);
    public static StageType MagmaMarsh = new StageType("The Magma Marsh", iconSpriteSheet[3], tiles[0]);
    public static StageType FrostyForest = new StageType("The Frosty Forest", iconSpriteSheet[4], tiles[0]);

    public static StageType StageFromInt(int key)
    {
        if (key == -1)
        {
            return allStageTypes[UnityEngine.Random.Range(0, allStageTypes.Count)];
        }

        return allStageTypes[key];
    }
}
