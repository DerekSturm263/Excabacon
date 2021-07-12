public static class GamemodeTypes
{

    public static System.Collections.Generic.List<GamemodeType> allGamemodeTypes = new System.Collections.Generic.List<GamemodeType>();
    public static int Count
    {
        get => allGamemodeTypes.Count;
    }

    public static GamemodeType PvP = new GamemodeType("PvP", "Battle against other players in a fight to the death! The last pig standing will be declared the winner in this exciting gamemode!", "2 - 4 Players", new System.Predicate<PlayerController>((player) => player.alterableStats.kills > 0));
    public static GamemodeType TreasureHunt = new GamemodeType("Treasure Hunt", "Compete in a race to collect the most treasure! Players dig to collect treasure, and whoever has the most at the end of the round wins!", "2 - 4 Players", new System.Predicate<PlayerController>((player) => player.alterableStats.currentHP == player.playerClass.hp));
    public static GamemodeType RelicKing = new GamemodeType("Relic King", "Gaining control of the relic for 20 seconds will make you the winner in this gamemode. With the relic, you will have heightened stats.", "2 - 4 Players", new System.Predicate<PlayerController>((player) => player.currentItem.item == ItemTypes.Relic));
    public static GamemodeType BossBattle = new GamemodeType("Boss Battle", "Battle bosses alongside your teammates in this gamemode. Choose from a variety of bosses and try to beat your high scores!", "1 - 4 Players", new System.Predicate<PlayerController>((player) => player.weaponClass == WeaponTypes.Drill));

    public static GamemodeType GamemodeFromInt(int key)
    {
        return allGamemodeTypes[key];
    }
}
