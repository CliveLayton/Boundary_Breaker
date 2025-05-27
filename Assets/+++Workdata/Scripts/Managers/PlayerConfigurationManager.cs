using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerConfigurationManager : MonoBehaviour
{
    [SerializeField] private GameObject player1Canvas;
    [SerializeField] private GameObject player2Canvas;
    [SerializeField] private GameObject FirstSelectedP1;
    [SerializeField] private GameObject FirstSelectedP2;

    public readonly List<PlayerConfiguration> PlayerConfigs = new List<PlayerConfiguration>();
    public static PlayerConfigurationManager Instance { get; private set; }

    public bool hasEnteredCharSelection;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void HandlePlayerJoined(PlayerInput pi)
    {
        if (PlayerConfigs.Any(config => config.Input == pi))
        {
            return;
        }
        Debug.Log("Player Joined: " + pi.playerIndex);
        pi.transform.SetParent(transform);

        MultiplayerEventSystem multiplayerEventSystem = pi.gameObject.AddComponent<MultiplayerEventSystem>();
        pi.uiInputModule = pi.gameObject.AddComponent<InputSystemUIInputModule>();
        if (!hasEnteredCharSelection)
        {
            multiplayerEventSystem.enabled = false;
            pi.uiInputModule.enabled = false;
        }

        PlayerConfigs.Add(new PlayerConfiguration(pi, multiplayerEventSystem, pi.uiInputModule));

        if (pi.playerIndex == 0)
        {
            multiplayerEventSystem.SetSelectedGameObject(null);
            multiplayerEventSystem.SetSelectedGameObject(FirstSelectedP1);
            multiplayerEventSystem.playerRoot = player1Canvas;
        }
        else if (pi.playerIndex == 1)
        {
            multiplayerEventSystem.SetSelectedGameObject(null);
            multiplayerEventSystem.SetSelectedGameObject(FirstSelectedP2);
            multiplayerEventSystem.playerRoot = player2Canvas;
        }
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi, MultiplayerEventSystem mes, InputSystemUIInputModule uIModule)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
        PlayerEvent = mes;
        UIInputModule = uIModule;
    }
    
    public PlayerInput Input { get; set; }
    
    public MultiplayerEventSystem PlayerEvent { get; set; }
    
    public InputSystemUIInputModule UIInputModule { get; set; }
    
    public int PlayerIndex { get; set; }

    public bool IsReady { get; set; }
}
