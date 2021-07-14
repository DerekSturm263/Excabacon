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

    public TMPro.TMP_Text gamemodeName;
    public TMPro.TMP_Text gamemodeDescription;
    public TMPro.TMP_Text gamemodePlayers;

    public UnityEngine.UI.Image stageIcon;
    public TMPro.TMP_Text stageName;

    public UnityEngine.UI.Button[] playerButtons;

    public TMPro.TMP_Text[] playerNames = new TMPro.TMP_Text[4];
    public UnityEngine.UI.Image[] playerIcons = new UnityEngine.UI.Image[4];
    public UnityEngine.UI.Image[] weaponIcons = new UnityEngine.UI.Image[4];
    public UnityEngine.UI.Image[] abilityIcons = new UnityEngine.UI.Image[4];

    public static GameSetupUIController current;

    private System.Action<GameObject> onNewSelection = delegate { };
    private GameObject oldSelected;

    private void Awake()
    {
        controls = new Controls();
        Player.onPlayerChange += OpenOrCloseSpot;
        current = this;

        eventSystem = EventSystem.current;

        players.SetActive(false);
        stages.SetActive(false);

        controls.UI.Back.performed += ctx => Back(ctx.control.device);
        controls.UI.Options.performed += ctx => OpenOptions();

        controls.UI.CycleLeft.started += ctx => SwitchPlayer(ctx.control.device, -1);
        controls.UI.CycleRight.started += ctx => SwitchPlayer(ctx.control.device, 1);

        controls.UI.SelectJoin.performed += ctx => JoinOrSelectPlayer(ctx.control.device);
        controls.UI.DeselectLeave.performed += ctx => LeaveOrDeselectPlayer(ctx.control.device);

        // Default values.
        GameController.gameSettings = new GameSettings(GamemodeTypes.PvP, new PigSettings[4], new ItemSettings(ItemTypes.allItemTypes, 2f, 30f), StageTypes.PlainsOfPlay, 5, 180f, 0);

        oldSelected = eventSystem.firstSelectedGameObject;
        onNewSelection += SwitchGamemode;
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject != oldSelected)
        {
            oldSelected = eventSystem.currentSelectedGameObject;
            onNewSelection.Invoke(eventSystem.currentSelectedGameObject);
        }
    }

    private void Back(InputDevice device)
    {
        if (gamemodes.activeSelf)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        }
        else if (players.activeSelf)
        {
            players.SetActive(false);
            gamemodes.SetActive(true);

            eventSystem.SetSelectedGameObject(gamemodes.GetComponentsInChildren<UnityEngine.UI.Button>()[0].gameObject);
        }
        else
        {
            players.SetActive(true);
            stages.SetActive(false);
        }
    }

    private void OpenOptions()
    {

    }

    private void SwitchGamemode(GameObject selection)
    {
        if (!gamemodes.activeSelf && !stages.activeSelf)
            return;

        if (gamemodes.activeSelf)
        {
            try
            {
                GamemodeType newGamemode = GamemodeTypes.allGamemodeTypes[selection.transform.GetSiblingIndex()];

                gamemodeName.text = newGamemode.name;
                gamemodeDescription.text = newGamemode.description;
                gamemodePlayers.text = newGamemode.minPlayerCount + " - " + newGamemode.maxPlayerCount + " Players";
            }
            catch { }
        }
        else
        {
            try
            {
                StageType newStage = StageTypes.allStageTypes[selection.transform.GetSiblingIndex()];

                stageIcon.sprite = newStage.icon;
                stageName.text = newStage.name;
            } catch { }
        }
    }

    public void SelectGamemode(int gamemodeNum)
    {
        GameController.gameSettings.gameMode = GamemodeTypes.GamemodeFromInt(gamemodeNum);

        gamemodes.SetActive(false);
        players.SetActive(true);
        
        //eventSystem.SetSelectedGameObject(players.GetComponentsInChildren<UnityEngine.UI.Button>()[0].gameObject);
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
        ++GameController.gameSettings.playerCount;

        playerButtons[player.playerNum].transform.GetChild(0).gameObject.SetActive(false);
        playerButtons[player.playerNum].transform.GetChild(1).gameObject.SetActive(true);

        if (GameController.gameSettings.playerCount < GameController.gameSettings.gameMode.minPlayerCount)
            return;

        foreach (System.Collections.Generic.KeyValuePair<InputDevice, Player> playerVal in Player.players)
        {
            if (!playerVal.Value.isReady)
                return;
        }

        players.SetActive(false);
        stages.SetActive(true);

        Invoke(nameof(SelectStagesEventSystem), 0.05f);
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

            --GameController.gameSettings.playerCount;
        }
        else
        {
            Player.RemovePlayer(device);
        }
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

        GameController.gameSettings.itemSettings.spawnRate = spawnRate;
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
