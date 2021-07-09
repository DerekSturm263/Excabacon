using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    public static GameSetupUIController current;

    private void Awake()
    {
        controls = new Controls();
        Player.onPlayerChange += OpenOrCloseSpot;
        current = this;

        eventSystem = EventSystem.current;

        players.SetActive(false);
        stages.SetActive(false);

        controls.UI.CycleLeft.started += ctx => SwitchPlayer(ctx.control.device, -1);
        controls.UI.CycleRight.started += ctx => SwitchPlayer(ctx.control.device, 1);

        controls.UI.SelectJoin.performed += ctx => JoinOrSelectPlayer(ctx.control.device);
        controls.UI.DeselectLeave.performed += ctx => LeaveOrDeselectPlayer(ctx.control.device);

        // Default values.
        GameController.gameSettings = new GameSettings();

        GameController.gameSettings.players = new PigSettings[4];
        GameController.gameSettings.stocks = 5;
        GameController.gameSettings.matchTime = 180f;
        GameController.gameSettings.itemSettings.itemSpawnRate = 2;
        GameController.gameSettings.itemSettings.items = ItemTypes.allItemTypes;
    }

    public void OpenOrCloseSpot(Player player, Player.PlayerChange change)
    {
        switch (change)
        {
            case Player.PlayerChange.Player_Added:
                playerButtons[player.playerNum].transform.GetChild(0).gameObject.SetActive(true);
                playerButtons[player.playerNum].transform.GetChild(2).gameObject.SetActive(false);
                break;
            case Player.PlayerChange.Player_Removed:
                playerButtons[player.playerNum].transform.GetChild(0).gameObject.SetActive(false);
                playerButtons[player.playerNum].transform.GetChild(2).gameObject.SetActive(true);
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

    public void SwitchPlayer(InputDevice device, int direction)
    {
        if (!players.activeSelf || !Player.players.ContainsKey(device) || !playerButtons[Player.GetPlayerFromDevice(device).playerNum].interactable)
            return;

        Player p = Player.GetPlayerFromDevice(device);

        pigTypes[p.playerNum] += direction;

        if (pigTypes[p.playerNum] < 0)
        {
            pigTypes[p.playerNum] = PigTypes.Count - 1;
        }
        else if (pigTypes[p.playerNum] > PigTypes.Count - 1)
        {
            pigTypes[p.playerNum] = 0;
        }

        playerNames[p.playerNum].text = PigTypes.PigFromInt(pigTypes[p.playerNum]).name;
        playerIcons[p.playerNum].sprite = PigTypes.PigFromInt(pigTypes[p.playerNum]).icon;
        weaponIcons[p.playerNum].sprite = WeaponTypes.WeaponFromInt(pigTypes[p.playerNum]).icon;
        abilityIcons[p.playerNum].sprite = AbilityTypes.AbilityFromInt(pigTypes[p.playerNum]).icon;
    }

    public void JoinOrSelectPlayer(InputDevice device)
    {
        if (!players.activeSelf)
            return;

        if (Player.players.ContainsKey(device))
        {
            SelectPlayer(Player.players[device]);
        }
        else
        {
            Player.AddPlayer(device);
        }
    }

    public void LeaveOrDeselectPlayer(InputDevice device)
    {
        if (!players.activeSelf || !Player.players.ContainsKey(device))
            return;

        Player p = Player.GetPlayerFromDevice(device);

        if (p.isReady)
        {
            p.isReady = false;
            playerButtons[p.playerNum].interactable = true;

            playerButtons[p.playerNum].transform.GetChild(0).gameObject.SetActive(true);
            playerButtons[p.playerNum].transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            Player.RemovePlayer(device);
        }
    }

    public void SelectPlayer(Player player)
    {
        if (!players.activeSelf || !playerButtons[player.playerNum].interactable)
            return;

        playerButtons[player.playerNum].interactable = false;

        if (pigTypes[player.playerNum] == 0) // Random.
        {
            GameController.gameSettings.players[player.playerNum].player = PigTypes.PigFromInt(Random.Range(1, PigTypes.Count));
            GameController.gameSettings.players[player.playerNum].weapon = WeaponTypes.WeaponFromInt(Random.Range(1, WeaponTypes.Count));
            GameController.gameSettings.players[player.playerNum].ability = AbilityTypes.AbilityFromInt(Random.Range(1, AbilityTypes.Count));
        }
        else
        {
            GameController.gameSettings.players[player.playerNum].player = PigTypes.PigFromInt(pigTypes[player.playerNum]);
            GameController.gameSettings.players[player.playerNum].weapon = WeaponTypes.WeaponFromInt(pigTypes[player.playerNum]);
            GameController.gameSettings.players[player.playerNum].ability = AbilityTypes.AbilityFromInt(pigTypes[player.playerNum]);
        }

        player.isReady = true;

        playerButtons[player.playerNum].transform.GetChild(0).gameObject.SetActive(false);
        playerButtons[player.playerNum].transform.GetChild(1).gameObject.SetActive(true);

        foreach (System.Collections.Generic.KeyValuePair<InputDevice, Player> playerVal in Player.players)
        {
            if (!playerVal.Value.isReady)
                return;
        }

        players.SetActive(false);
        stages.SetActive(true);

        instructionLabel.text = "Choose The Stage";

        Invoke("SelectStagesEventSystem", 0.05f);
    }

    private void SelectStagesEventSystem()
    {
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

    public static int FindOpenSpot()
    {
        for (int i = 0; i < 4; ++i)
        {
            if (current.playerButtons[i].transform.GetChild(2).gameObject.activeSelf)
                return i;
        }

        return -1;
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
