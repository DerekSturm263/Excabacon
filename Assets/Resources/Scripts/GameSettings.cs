public struct GameSettings
{
    public enum GameMode : byte
    {
        Last_Pig_Standing, Relic_King, Treasure_Hunt, Raid_Battle
    }
    public GameMode gameMode;

    public PlayerSettings[] players;
    public ItemSettings itemSettings;
    public StageType stage;

    public int stocks;
    public float matchTime;
}
