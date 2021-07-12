using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameSettings gameSettings;
    public static GameObject[] spawnPoints;

    public GameObject player;
    public GameObject[] huds;

    public System.Collections.Generic.List<PlayerController> players = new System.Collections.Generic.List<PlayerController>();
    public GameObject Terrain;

    [System.NonSerialized]
    public static ModifyTerrain TerrainInterface;

    [HideInInspector] public float timeRemaining;
    public TMPro.TMP_Text time;

    public static PlayerController[] winners;
    public System.Collections.Generic.List<PlayerController> playersLeft = new System.Collections.Generic.List<PlayerController>();

    public static GameController current;

    private void Awake()
    {
        current = this;

        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
        TerrainInterface = Terrain.GetComponent<ModifyTerrain>();
        timeRemaining = gameSettings.matchTime;

        winners = new PlayerController[gameSettings.playerCount];

        for (int i = 0; i < 4; ++i)
        {
            if (gameSettings.players[i].player != null)
            {
                huds[i].SetActive(true);

                PigSettings pigSettings = gameSettings.players[i];
                PlayerController newPlayer = Instantiate(player).GetComponent<PlayerController>();
                newPlayer.Setup(Player.GetPlayerFromIndex(i), pigSettings.player, pigSettings.weapon, pigSettings.ability, i, pigSettings.teamNum);

                players.Add(newPlayer);
                playersLeft.Add(newPlayer);
                winners[i] = newPlayer;

                CameraController.targets.Add(newPlayer.transform);
            }
            else
            {
                huds[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        timeRemaining -= Time.deltaTime;
        time.text = (int) timeRemaining / 60 + ":" + ((int) timeRemaining % 60).ToString().PadLeft(2, '0');

        if (gameSettings.gameMode.gameEndCondition.Invoke(this))
        {
            GameEnd();
        }
    }

    private void GameEnd()
    {
        System.Array.Sort(winners, gameSettings.gameMode.winnerSorter);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Results");
    }
}
