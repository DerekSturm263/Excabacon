using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public GameObject[] huds;

    private void Awake()
    {
        for (int i = 0; i < Gamepad.all.Count; ++i)
        {
            huds[i].SetActive(true);

            PlayerController newPlayer = Instantiate(player).GetComponent<PlayerController>();
            newPlayer.playerNum = i;
        }
    }
}
