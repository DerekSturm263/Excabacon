using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GameSetupUIController : MonoBehaviour
{
    private Controls controls;

    private EventSystem eventSystem;

    public byte items = 0b_0000_0000;
    public int[] pigTypes = new int[4];

    public GameObject gamemodes;
    public GameObject players;
    public GameObject stages;

    public GameObject[] playerButtons;

    public TMPro.TMP_Text[] playerNames = new TMPro.TMP_Text[4];
    public UnityEngine.UI.Image[] playerIcons = new UnityEngine.UI.Image[4];
    public UnityEngine.UI.Image[] weaponIcons = new UnityEngine.UI.Image[4];
    public UnityEngine.UI.Image[] abilityIcons = new UnityEngine.UI.Image[4];

    public System.Collections.Generic.List<Player> inputPlayers = new System.Collections.Generic.List<Player>();
    public bool[] isPlayerReady = new bool[4];

    private void Awake()
    {
        controls = new Controls();
        eventSystem = EventSystem.current;

        players.SetActive(false);
        stages.SetActive(false);

        controls.UI.CycleLeft.started += ctx => SwitchPlayer(Player.PlayerFromDevice(ctx.control.device), -1);
        controls.UI.CycleRight.started += ctx => SwitchPlayer(Player.PlayerFromDevice(ctx.control.device), 1);

        // Default values.
        GameController.gameSettings = new GameSettings();
        GameController.gameSettings.players = new PlayerSettings[4];
        GameController.gameSettings.stocks = 5;
        GameController.gameSettings.matchTime = 180f;
        GameController.gameSettings.itemSettings.itemSpawnRate = 2;
        GameController.gameSettings.itemSettings.items = ItemTypes.allItemTypes;

        foreach (InputDevice device in InputSystem.devices)
        {
            ChangeDevice(device, InputDeviceChange.Added);
        }

        InputSystem.onDeviceChange += (device, change) => ChangeDevice(device, change);
    }

    public void ChangeDevice(InputDevice device, InputDeviceChange change)
    {
        Debug.Log(device.displayName + " was " + change.ToString());

        switch (change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Reconnected:
                inputPlayers.Add(new Player(device, new InputUser()));
                playerButtons[Player.PlayerFromDevice(device).playerNum].SetActive(true);
                break;

            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                playerButtons[Player.PlayerFromDevice(device).playerNum].SetActive(false);
                inputPlayers[Player.PlayerFromDevice(device).playerNum] = null;
                inputPlayers.Remove(Player.PlayerFromDevice(device));
                break;
        }
    }

    public void SelectGamemode(int gamemodeNum)
    {
        GameController.gameSettings.gameMode = (GameSettings.GameMode) gamemodeNum;

        gamemodes.SetActive(false);
        players.SetActive(true);

        eventSystem.SetSelectedGameObject(players.GetComponentsInChildren<UnityEngine.UI.Button>()[0].gameObject);
    }

    public void SwitchPlayer(Player player, int direction)
    {
        if (!players.activeSelf)
            return;

        pigTypes[player.playerNum] += direction;
        if (pigTypes[player.playerNum] < 0)
        {
            pigTypes[player.playerNum] = PlayerTypes.Count - 1;
        }
        else if (pigTypes[player.playerNum] > PlayerTypes.Count - 1)
        {
            pigTypes[player.playerNum] = 0;
        }

        playerNames[player.playerNum].text = PlayerTypes.PlayerFromInt(pigTypes[player.playerNum]).name;
        playerIcons[player.playerNum].sprite = PlayerTypes.PlayerFromInt(pigTypes[player.playerNum]).icon;
        weaponIcons[player.playerNum].sprite = WeaponTypes.WeaponFromInt(pigTypes[player.playerNum]).icon;
        abilityIcons[player.playerNum].sprite = AbilityTypes.AbilityFromInt(pigTypes[player.playerNum]).icon;
    }

    public void SelectPlayer(int playerNum)
    {
        GameController.gameSettings.players[playerNum].player = PlayerTypes.PlayerFromInt(pigTypes[playerNum]);
        GameController.gameSettings.players[playerNum].weapon = WeaponTypes.WeaponFromInt(pigTypes[playerNum]);
        GameController.gameSettings.players[playerNum].ability = AbilityTypes.AbilityFromInt(pigTypes[playerNum]);

        isPlayerReady[playerNum] = true;

        for (int i = 0; i < inputPlayers.Count; ++i)
        {
            if (!isPlayerReady[i])
                return;
        }

        players.SetActive(false);
        stages.SetActive(true);

        eventSystem.SetSelectedGameObject(stages.GetComponentsInChildren<UnityEngine.UI.Button>()[0].gameObject);
    }

    public void SelectStage(int stageNum)
    {
        GameController.gameSettings.stage = StageTypes.StageFromInt(stageNum);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MovementTest");
    }

    public void ToggleItem(byte item)
    {
        items |= item;
    }
    
    public void SelectItems(byte items, float spawnRate)
    {
        // Add items based on whether the bit is on or off.

        GameController.gameSettings.itemSettings.itemSpawnRate = spawnRate;
    }

    public void SelectStock(int newStock)
    {
        GameController.gameSettings.stocks = newStock;
    }

    public void SelectTime(float newTimeLimit)
    {
        GameController.gameSettings.matchTime = newTimeLimit;
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
