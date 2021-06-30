using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public GameObject[] huds;

    public GameObject[] spawnPoints;

    private void Awake()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");

        for (int i = 0; i < Gamepad.all.Count; ++i)
        {
            huds[i].SetActive(true);

            PlayerController newPlayer = Instantiate(player).GetComponent<PlayerController>();
            newPlayer.playerNum = i;
            newPlayer.transform.position = spawnPoints[i].transform.position;
            InputUser.PerformPairingWithDevice(Gamepad.all[i], InputUser.all[i]);
        }
    }
}
