using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameSettings gameSettings;
    public static GameObject[] spawnPoints;

    public GameObject player;
    public GameObject[] huds;

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

                PigSettings playerSettings = gameSettings.players[i];
                PlayerController newPlayer = Instantiate(player).GetComponent<PlayerController>();
                newPlayer.Setup(Player.GetPlayerFromIndex(i), playerSettings.player, playerSettings.weapon, playerSettings.ability, i, playerSettings.teamNum);

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
    }
}
