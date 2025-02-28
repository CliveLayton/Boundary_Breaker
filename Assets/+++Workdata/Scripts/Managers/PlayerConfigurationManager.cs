using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigurationManager : MonoBehaviour
{
    private List<PlayerConfiguration> playerConfigs = new List<PlayerConfiguration>();

    public static PlayerConfigurationManager Instance { get; private set; }

    public void HandlePlayerJoined(PlayerInput pi)
    {
        Debug.Log("Player Joined: " + pi.playerIndex);
        pi.transform.SetParent(transform);

        if (playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            playerConfigs.Add(new PlayerConfiguration(pi));
        }
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }
    
    public PlayerInput Input { get; set; }
    
    public int PlayerIndex { get; set; }
    
    public bool IsReady { get; set; }
}
