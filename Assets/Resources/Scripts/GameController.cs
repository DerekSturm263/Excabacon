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

    private void Awake()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
        TerrainInterface = Terrain.GetComponent<ModifyTerrain>();
        timeRemaining = gameSettings.matchTime;

        for (int i = 0; i < 4; ++i)
        {
            if (gameSettings.players[i].player != null)
            {
                huds[i].SetActive(true);

                PigSettings pigSettings = gameSettings.players[i];
                PlayerController newPlayer = Instantiate(player).GetComponent<PlayerController>();
                newPlayer.Setup(Player.GetPlayerFromIndex(i), pigSettings.player, pigSettings.weapon, pigSettings.ability, i, pigSettings.teamNum);

                players.Add(newPlayer);
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

        if (timeRemaining <= 0)
        {
            GameEnd();
        }
    }

    private void GameEnd()
    {
        PlayerController winner = null;

        foreach (PlayerController p in players)
        {
            if (CheckForWin(p, gameSettings.gameMode.winCondition, ref winner))
            {
                break;
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Results");
    }

    public bool CheckForWin(PlayerController player, System.Predicate<PlayerController> winCondition, ref PlayerController winner)
    {
        if (winCondition.Invoke(player))
        {
            winner = player;
            return true;
        }

        return false;
    }
}
