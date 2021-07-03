using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class Player
{
    public int playerNum;

    public InputDevice pairedDevice;
    public InputUser inputUser;

    private static System.Collections.Generic.Dictionary<InputDevice, Player> players = new System.Collections.Generic.Dictionary<InputDevice, Player>();

    public Player(InputDevice pairedDevice, InputUser inputUser)
    {
        this.pairedDevice = pairedDevice;
        this.inputUser = inputUser;

        InputUser.PerformPairingWithDevice(pairedDevice, inputUser);
        players.Add(pairedDevice, this);
    }

    public static Player PlayerFromDevice(InputDevice device)
    {
        return players[device];
    }
    
    ~Player()
    {
        players.Remove(pairedDevice);
        inputUser.UnpairDevicesAndRemoveUser();
    }
}
