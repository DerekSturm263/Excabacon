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

    public UnityEngine.UI.Button[] playerButtons;

    public TMPro.TMP_Text instructionLabel;

    public TMPro.TMP_Text[] playerNames = new TMPro.TMP_Text[4];
    public UnityEngine.UI.Image[] playerIcons = new UnityEngine.UI.Image[4];
    public UnityEngine.UI.Image[] weaponIcons = new UnityEngine.UI.Image[4];
    public UnityEngine.UI.Image[] abilityIcons = new UnityEngine.UI.Image[4];

    public static System.Collections.Generic.List<Player> inputPlayers = new System.Collections.Generic.List<Player>();

    private void Awake()
    {
        controls = new Controls();
        eventSystem = EventSystem.current;

        players.SetActive(false);
        stages.SetActive(false);

        controls.UI.CycleLeft.started += ctx => SwitchPlayer(Player.PlayerFromDevice(ctx.control.device), -1);
        controls.UI.CycleRight.started += ctx => SwitchPlayer(Player.PlayerFromDevice(ctx.control.device), 1);

        controls.UI.SelectPlayer.performed += ctx => SelectPlayer(Player.PlayerFromDevice(ctx.control.device));

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
                Player newPlayer = new Player(device, new InputUser());
                inputPlayers.Add(newPlayer);
                playerButtons[newPlayer.playerNum].gameObject.SetActive(true);

                break;

            case InputDeviceChange.Disconnected:

                break;

            case InputDeviceChange.Reconnected:

                break;

            case InputDeviceChange.Removed:
                playerButtons[Player.PlayerFromDevice(device).playerNum].gameObject.SetActive(false);
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

        instructionLabel.text = "Choose Your Characters";
        eventSystem.SetSelectedGameObject(players.GetComponentsInChildren<UnityEngine.UI.Button>()[0].gameObject);
    }

    public void SwitchPlayer(Player player, int direction)
    {
        if (!players.activeSelf || !playerButtons[player.playerNum].interactable)
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

    public void SelectPlayer(Player player)
    {
        if (!players.activeSelf || !playerButtons[player.playerNum].interactable)
            return;

        playerButtons[player.playerNum].interactable = false;

        GameController.gameSettings.players[player.playerNum].player = PlayerTypes.PlayerFromInt(pigTypes[player.playerNum]);
        GameController.gameSettings.players[player.playerNum].weapon = WeaponTypes.WeaponFromInt(pigTypes[player.playerNum]);
        GameController.gameSettings.players[player.playerNum].ability = AbilityTypes.AbilityFromInt(pigTypes[player.playerNum]);

        inputPlayers[player.playerNum].isReady = true;

        foreach (Player inputPlayer in inputPlayers)
        {
            if (!inputPlayer.isReady)
                return;
        }

        players.SetActive(false);
        stages.SetActive(true);

        instructionLabel.text = "Choose The Stage";
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
