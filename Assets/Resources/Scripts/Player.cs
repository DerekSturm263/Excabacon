using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Player
{
    public enum PlayerChange
    {
        Player_Added, Player_Removed
    }

    public int playerNum;
    public bool isReady;

    public InputDevice pairedDevice;
    public InputUser user;

    public static System.Collections.Generic.Dictionary<InputDevice, Player> players = new System.Collections.Generic.Dictionary<InputDevice, Player>();
    public static System.Action<Player, PlayerChange> onPlayerChange = delegate { };

    public Player(InputDevice pairedDevice)
    {
        this.playerNum = GameSetupUIController.FindOpenSpot();
        this.isReady = false;

        this.pairedDevice = pairedDevice;
        this.user = InputUser.CreateUserWithoutPairedDevices();

        InputUser.PerformPairingWithDevice(this.pairedDevice, user);
        players.Add(this.pairedDevice, this);

        UnityEngine.Debug.Log("Player " + playerNum + " has joined via " + pairedDevice.displayName);
    }

    public void Destroy()
    {
        players.Remove(pairedDevice);
        user.UnpairDevicesAndRemoveUser();

        UnityEngine.Debug.Log("Player " + playerNum + " has left");
    }

    public static Player GetPlayerFromIndex(int index)
    {
        foreach (System.Collections.Generic.KeyValuePair<InputDevice, Player> pair in players)
        {
            if (pair.Value.playerNum == index)
                return pair.Value;
        }

        return null;
    }

    public static Player GetPlayerFromDevice(InputDevice device)
    {
        return players[device];
    }

    public static void AddPlayer(InputDevice device)
    {
        Player newPlayer = new Player(device);

        onPlayerChange(newPlayer, PlayerChange.Player_Added);
    }

    public static void RemovePlayer(InputDevice device)
    {
        onPlayerChange(players[device], PlayerChange.Player_Removed);

        players[device].Destroy();
    }
}
