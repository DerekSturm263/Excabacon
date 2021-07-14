public static class GamemodeTypes
{
    public static System.Collections.Generic.List<GamemodeType> allGamemodeTypes = new System.Collections.Generic.List<GamemodeType>();
    public static int Count
    {
        get => allGamemodeTypes.Count;
    }

    #region Winner Sorters

    public static int ComparePlayerKills(PlayerController a, PlayerController b)
    {
        return b.alterableStats.kills.CompareTo(a.alterableStats.kills);
    }

    public static int CompareRelicTime(PlayerController a, PlayerController b)
    {
        return b.alterableStats.relicTime.CompareTo(a.alterableStats.relicTime);
    }

    #endregion

    #region End Conditions

    public static bool TimeRunsOut(GameController gameController)
    {
        return gameController.timeRemaining <= 0;
    }

    public static bool OnePlayerLeft(GameController gameController)
    {
        return gameController.playersLeft.Count < 2;
    }

    public static bool PlayerWinsRelic(GameController gameController)
    {
        // Change this later to be better.
        foreach (PlayerController player in gameController.players)
        {
            if (player.relicTime > GameController.gameSettings.itemSettings.relicTime)
            {
                return true;
            }
        }

        return false;
    }

    public static bool BossDies(GameController gameController)
    {
        return !gameController.currentBoss.activeSelf; // Change to check the HP when that's added.
    }

    #endregion

    public static GamemodeType PvP = new GamemodeType("PvP", "Battle against other players in a fight to the death! The last pig standing will be declared the winner in this exciting gamemode!", 2, 4,
        winnerSorter: new System.Comparison<PlayerController>((p1, p2) => ComparePlayerKills(p1, p2)),
        gameEndCondition: new System.Predicate<GameController>((gameController) => TimeRunsOut(gameController) || OnePlayerLeft(gameController)));
    public static GamemodeType TreasureHunt = new GamemodeType("Treasure Hunt", "Compete in a race to collect the most treasure! Players dig to collect treasure, and whoever has the most at the end of the round wins!", 2, 4,
        winnerSorter: new System.Comparison<PlayerController>((p1, p2) => ComparePlayerKills(p1, p2)),
        gameEndCondition: new System.Predicate<GameController>((gameController) => TimeRunsOut(gameController) || OnePlayerLeft(gameController)));
    public static GamemodeType RelicKing = new GamemodeType("Relic King", "Gaining control of the relic for 30 seconds will make you the winner in this gamemode. With the relic, you will have heightened stats.", 2, 4,
        winnerSorter: new System.Comparison<PlayerController>((p1, p2) => CompareRelicTime(p1, p2)),
        gameEndCondition: new System.Predicate<GameController>((gameController) => TimeRunsOut(gameController) || PlayerWinsRelic(gameController)));
    public static GamemodeType BossBattle = new GamemodeType("Boss Battle", "Battle bosses alongside your teammates in this gamemode. Choose from a variety of bosses and try to beat your high scores!", 1, 4,
        winnerSorter: new System.Comparison<PlayerController>((p1, p2) => 0),
        gameEndCondition: new System.Predicate<GameController>((gameController) => TimeRunsOut(gameController) || BossDies(gameController)),
        gameStart: new System.Action<GameController>((gameController) => gameController.SpawnBoss()));

    public static GamemodeType GamemodeFromInt(int key)
    {
        return allGamemodeTypes[key];
    }
}
