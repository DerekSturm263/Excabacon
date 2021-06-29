public static class PlayerTypes
{
    public static PlayerType PorkChops = new PlayerType(name: "Pork Chops", icon: UnityEngine.Resources.Load<UnityEngine.Sprite>("Assets/Sprites/porkChops_icon.png"), hp: 90, mana: 0, walkSpeed: 6, runSpeed: 10, undergroundSpeed: 8, jumpHeight: 15, damage: 1, knockback: 2, attackSpeed: 10);
    public static PlayerType TroughDweller = new PlayerType("Trough Dweller", UnityEngine.Resources.Load<UnityEngine.Sprite>("Assets/Sprites/troughDweller_icon.png"), 120, 0, 2, 6, 4, 2, 1.5f, 4, 4);
    public static PlayerType Porkerer = new PlayerType("Porkerer", UnityEngine.Resources.Load<UnityEngine.Sprite>("Assets/Sprites/porkerer_icon.png"), 75, 20, 4, 8, 6, 6, 0.75f, 0, 5);
    public static PlayerType RobinHog = new PlayerType("Robin Hog", UnityEngine.Resources.Load<UnityEngine.Sprite>("Assets/Sprites/robinHog_icon.png"), 80, 0, 8, 12, 10, 4, 1, 1, 0);
}
